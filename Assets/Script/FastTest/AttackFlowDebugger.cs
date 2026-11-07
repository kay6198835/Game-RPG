using UnityEngine;

// Debug harness for "pick Champion -> attack": logs, on change only, every link between the attack
// input and the Animator "Attack" bool. Drop it on any scene object; it binds on ON_PLAYER_READY,
// so neither the Player prefab nor any existing script needs to change. Delete when done.
public class AttackFlowDebugger : MonoBehaviour
{
    [SerializeField] private string attackParam = "Attack";
    [SerializeField] private bool logEveryChange = true;
    [Header("Frame trace (stage hand-off)")]
    [Tooltip("Logs every animation event with the clip and time it came from.")]
    [SerializeField] private bool logAnimationEvents = true;
    [Tooltip("On every controller swap, dumps the frames before it and traces the frames after it.")]
    [SerializeField] private bool traceFrames = true;
    [SerializeField, Min(1)] private int traceWindow = 6;
    [Tooltip("A frame longer than this is flagged as a HITCH.")]
    [SerializeField, Min(1f)] private float hitchMs = 25f;

    private SpriteRenderer spriteRenderer;
    private AttackFlowEventProbe probe;
    private string[] ring;
    private int ringHead;
    private int ringCount;
    private int traceFramesLeft;

    private Player player;
    private Animator anim;
    private PlayerInputHandler input;
    private WeaponHolder weaponHolder;
    private int attackParamHash;

    private Snapshot last;
    private RuntimeAnimatorController lastController;
    private bool paramExists;
    private bool warnedThisEntry;

    private struct Snapshot
    {
        public string State;
        public StatusAnimation Status;
        public bool IsAttack;
        public bool Buffer;
        public bool HasWeapon;
        public bool CanAttack;
        public bool ParamValue;
        public bool AnimInAttack;

        public bool SameAs(Snapshot o) =>
            State == o.State && Status == o.Status && IsAttack == o.IsAttack && Buffer == o.Buffer
            && HasWeapon == o.HasWeapon && CanAttack == o.CanAttack && ParamValue == o.ParamValue
            && AnimInAttack == o.AnimInAttack;

        public override string ToString() =>
            $"state={State} status={Status} | input.IsAttack={IsAttack} buffer={Buffer} | " +
            $"weapon={(HasWeapon ? "yes" : "NULL")} canAttack={CanAttack} | animBool={ParamValue} animInAttackState={AnimInAttack}";
    }

    private void Awake() => attackParamHash = Animator.StringToHash(attackParam);
    private void OnEnable() => EventManager.Resgister(EventID.ON_PLAYER_READY, OnPlayerReady);
    private void OnDisable() => EventManager.UnResgister(EventID.ON_PLAYER_READY, OnPlayerReady);

    private void OnPlayerReady(object payload)
    {
        if (payload is not Component root) return;
        player = root.GetComponent<Player>();
        if (player == null) return;
        anim = player.Anim != null ? player.Anim : player.GetComponent<Animator>();
        player.Core.GetCoreComponent(out input);
        player.Core.GetCoreComponent(out weaponHolder);
        lastController = null;
        spriteRenderer = anim != null ? anim.GetComponent<SpriteRenderer>() : null;
        ring = new string[Mathf.Max(1, traceWindow)];
        ringHead = ringCount = traceFramesLeft = 0;
        if (anim != null && logAnimationEvents)
        {
            if (!anim.TryGetComponent(out probe)) probe = anim.gameObject.AddComponent<AttackFlowEventProbe>();
            probe.OnEvent = OnAnimationEvent;
        }
        Log($"bound to '{player.name}' (input={(input != null)}, weaponHolder={(weaponHolder != null)}, animator={(anim != null)}, " +
            $"sprite={(spriteRenderer != null)}, eventProbe={(probe != null)})");
    }

    private void OnDestroy()
    {
        if (probe != null) Destroy(probe);
    }

    private void OnAnimationEvent(string name, AnimationEvent e)
    {
        string clip = e.animatorClipInfo.clip != null ? e.animatorClipInfo.clip.name : "<none>";
        Log($"EVENT {name} @ {e.time:F3}s clip='{clip}' w={e.animatorClipInfo.weight:F2} | state={StateName()} " +
            $"status={player.StateMachine.CurrentState?.Status} | sprite={SpriteName()}");
    }

    // One line per frame: how long the frame was, which sprite is on screen, which clip and where in it.
    private string FrameLine()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
        string clip = clips.Length > 0 && clips[0].clip != null ? clips[0].clip.name : "<none>";
        float length = info.length > 0f ? info.length : 0f;
        float norm = info.normalizedTime;
        float dtMs = Time.deltaTime * 1000f;
        string hitch = dtMs > hitchMs ? " HITCH" : "";
        string blend = clips.Length > 1 ? $" (+{clips.Length - 1} blended)" : "";
        return $"f{Time.frameCount} dt={dtMs:F1}ms{hitch} | sprite={SpriteName()} | clip='{clip}'{blend} " +
               $"t={norm * length:F3}s n={norm:F3} | anim={(info.IsName(attackParam) ? "Attack" : "other")} " +
               $"transition={anim.IsInTransition(0)} | state={StateName()} status={player.StateMachine.CurrentState?.Status}";
    }

    private string SpriteName() =>
        spriteRenderer != null && spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "<none>";

    private void TraceFrame(bool controllerChanged)
    {
        if (!traceFrames || ring == null) return;
        string line = FrameLine();
        if (controllerChanged)
        {
            int start = (ringHead - ringCount + ring.Length) % ring.Length;
            for (int i = 0; i < ringCount; i++) Log("TRACE pre  " + ring[(start + i) % ring.Length]);
            ringCount = 0;
            traceFramesLeft = traceWindow;
        }
        if (traceFramesLeft > 0)
        {
            traceFramesLeft--;
            Log("TRACE post " + line, line.Contains(" HITCH") ? LogType.Warning : LogType.Log);
            return;
        }
        ring[ringHead] = line;
        ringHead = (ringHead + 1) % ring.Length;
        if (ringCount < ring.Length) ringCount++;
    }

    private void LateUpdate()
    {
        if (player == null || anim == null || input == null || weaponHolder == null) return;

        // Weapon.OnAttackEnter swaps runtimeAnimatorController on every stage, and SetCharacterData swaps it on pick.
        bool controllerChanged = anim.runtimeAnimatorController != lastController;
        TraceFrame(controllerChanged);
        if (controllerChanged)
        {
            lastController = anim.runtimeAnimatorController;
            paramExists = HasParam(anim, attackParamHash);
            Log($"CONTROLLER -> {Describe(lastController)} | has '{attackParam}' param: {paramExists} | animBool now={ReadParam()}"
                + $" | state={StateName()}", paramExists ? LogType.Log : LogType.Error);
        }

        Snapshot now = Capture();
        if (logEveryChange && !now.SameAs(last)) Log(now.ToString());

        if (now.IsAttack && now.State != nameof(PlayerAttackState) && last.IsAttack == false)
            Log("Attack pressed but not in PlayerAttackState. Gate (PlayerBasicState:57) = " +
                $"IsAttack({now.IsAttack}) && Weapon!=null({now.HasWeapon}) && CanAttack({now.CanAttack}); " +
                $"also blocked if Equip/Interact/ResourceReceiver input is set first (current state={now.State})",
                LogType.Warning);

        bool inAttack = now.State == nameof(PlayerAttackState);
        if (!inAttack) warnedThisEntry = false;
        else if (!now.ParamValue && !warnedThisEntry)
        {
            warnedThisEntry = true;
            Log($"In PlayerAttackState but Animator '{attackParam}' is FALSE. PlayerState.Enter set it, then " +
                $"WeaponHolder.Attack() swapped the controller to {Describe(anim.runtimeAnimatorController)} " +
                "— a swap rebinds the Animator and can reset bools. Run the context menu probe to confirm.",
                LogType.Error);
        }

        last = now;
    }

    private Snapshot Capture()
    {
        PlayerState state = player.StateMachine.CurrentState;
        return new Snapshot
        {
            State = state != null ? state.GetType().Name : "<none>",
            Status = state != null ? state.Status : StatusAnimation.None,
            IsAttack = input.IsAttack,
            Buffer = input.BufferIsAttack,
            HasWeapon = weaponHolder.Weapon != null,
            CanAttack = weaponHolder.CanAttack(),
            ParamValue = ReadParam(),
            AnimInAttack = anim.GetCurrentAnimatorStateInfo(0).IsName(attackParam),
        };
    }

    // Sets the bool, swaps to stage 0's override exactly like Weapon.OnAttackEnter, reads it back, restores.
    [ContextMenu("Probe: does the attack controller swap reset the Attack bool?")]
    private void ProbeSwap()
    {
        if (player == null || weaponHolder?.Weapon == null)
        {
            Log("Probe needs a bound player with an equipped weapon (pick a Champion first).", LogType.Warning);
            return;
        }
        AttackSO stage = weaponHolder.Weapon.Stats.GetStage(0);
        RuntimeAnimatorController original = anim.runtimeAnimatorController;
        bool originalValue = ReadParam();

        anim.SetBool(attackParamHash, true);
        bool before = ReadParam();
        anim.runtimeAnimatorController = stage.directionAttackAnimatorOV;
        bool after = ReadParam();

        anim.runtimeAnimatorController = original;
        anim.SetBool(attackParamHash, originalValue);

        Log($"PROBE: {Describe(original)} -> {Describe(stage.directionAttackAnimatorOV)} | bool before swap={before}, after swap={after} => "
            + (after ? "swap keeps the bool, look elsewhere" : "SWAP RESETS THE BOOL (root cause)"),
            after ? LogType.Log : LogType.Error);
    }

    private bool ReadParam() => paramExists && anim.GetBool(attackParamHash);

    private string StateName()
    {
        PlayerState s = player.StateMachine.CurrentState;
        return s != null ? s.GetType().Name : "<none>";
    }

    private static bool HasParam(Animator animator, int hash)
    {
        if (animator.runtimeAnimatorController == null) return false;
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.nameHash == hash) return true;
        return false;
    }

    private static string Describe(RuntimeAnimatorController c)
    {
        if (c == null) return "<null>";
        return c is AnimatorOverrideController ov
            ? $"'{ov.name}' (override of '{(ov.runtimeAnimatorController != null ? ov.runtimeAnimatorController.name : "<null>")}')"
            : $"'{c.name}'";
    }

    private void Log(string msg, LogType type = LogType.Log) =>
        Debug.unityLogger.Log(type, $"[AttackFlow f{Time.frameCount}] {msg}");
}
