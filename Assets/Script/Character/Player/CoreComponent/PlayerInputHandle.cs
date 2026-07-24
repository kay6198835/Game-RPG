using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : CoreComponent<Core>
{
    #region Attribute
    public float starTime;
    public enum SkillState
    {
        Start,
        Cast,
        Do,
    }
    public enum SkillType
    {
        Special,
        Ability
    }
    public enum DisadvantageState
    {
        TakeDamaged,
    }
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;

    [Header("Direction by Keyboard")]
    [SerializeField] private Vector2 directionKeyboardVector;
    [SerializeField] private float angleKeyboardDirection;
    [SerializeField] private int directionKeyboard;

    [Header("Direction by Externality")]
    [SerializeField] private Vector2 directionExternalityVector;
    [SerializeField] private float angleExternalityDirection;
    [SerializeField] private int directionExternality;

    [Header("Direction by Mouse")]
    [SerializeField] private Vector2 directionMouseVector;
    [SerializeField] private int directionMouse;
    [SerializeField] private float angleMouseDirection;
    [SerializeField] private float angleRotationPlayer;



    [Header("Bool Value")]
    [SerializeField] private bool isAttack;
    [SerializeField] public bool BufferIsAttack { get; private set; } = false;
    [SerializeField] private bool isSkill;
    [SerializeField] private bool isDisadvantage;
    [SerializeField] private bool isTakeDamage;
    [SerializeField] private bool isEquip_Unequip = false;
    [SerializeField] private bool isInteractor = false;

    [Header("Enum Value")]
    [SerializeField] private SkillState state;
    [SerializeField] private SkillType skill;
    [SerializeField] private DisadvantageState disadvantage;

    #region Get value 
    public Vector2 MoveVector { get => moveVector; }
    public Vector2 MouseVector { get => mouseVector; }
    public Vector2 DirectionMouseVector { get => directionMouseVector; }
    public int DirectionMouse { get => directionMouse; }
    public float AngleRotationPlayer { get => angleRotationPlayer; }
    public float AngleLookDirection { get => angleMouseDirection; }
    public bool IsAttack { get => isAttack; }
    public SkillState State { get => state; }
    public SkillType Skill { get => skill; }
    public bool IsSkill { get => isSkill; }
    public PlayerInput PlayerInput { get => playerInput; }
    public bool IsDisadvantage { get => isDisadvantage; }
    public bool IsTakeDamage { get => isTakeDamage; }
    public bool IsEquip_Unequip { get => isEquip_Unequip; }
    public bool IsInteractor { get => isInteractor; }
    public Vector2 DirectionKeyboardVector { get => directionKeyboardVector; }
    public float AngleKeyboardDirection { get => angleKeyboardDirection; }
    public int DirectionKeyboard { get => directionKeyboard; }
    public Vector2 DirectionExternalityVector { get => directionExternalityVector; }
    public float AngleExternalityDirection { get => angleExternalityDirection; }
    public int DirectionExternality { get => directionExternality; }
    #endregion


    #endregion
    WeaponHolder weaponHolder;
    AbilityHolder abilityHolder;
    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
    }
    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out weaponHolder);
        Core.GetCoreComponent(out abilityHolder);

    }
    #region OnMethod
    protected void OnEnable()
    {
        playerInput.Control.Enable();
        playerInput.Control.Movement.started += OnMove;
        playerInput.Control.Movement.performed += OnMove;
        playerInput.Control.Movement.canceled += OnMove;

        playerInput.Control.MousePosition.performed += OnDirection;

        playerInput.Control.Attack.started += OnAttack;
        playerInput.Control.Attack.canceled += OnAttack;

        playerInput.Control.SkillWeapon.started += OnSkillWeapon;
        playerInput.Control.SkillWeapon.performed += OnSkillWeapon;
        playerInput.Control.SkillWeapon.canceled += OnSkillWeapon;

        playerInput.Control.Block.started += OnAbilityWeapon;
        playerInput.Control.Block.performed += OnAbilityWeapon;
        playerInput.Control.Block.canceled += OnAbilityWeapon;

        playerInput.Control.EquipUnequip.started += OnEquipUnequip;
        playerInput.Control.EquipUnequip.canceled += OnEquipUnequip;


        playerInput.Control.Interactor.started += OnInteractor;
        playerInput.Control.Interactor.canceled += OnInteractor;
    }
    protected void OnDisable()
    {
        playerInput.Control.Movement.started -= OnMove;
        playerInput.Control.Movement.performed -= OnMove;
        playerInput.Control.Movement.canceled -= OnMove;

        playerInput.Control.MousePosition.performed -= OnDirection;

        playerInput.Control.Attack.started -= OnAttack;
        playerInput.Control.Attack.canceled -= OnAttack;

        playerInput.Control.SkillWeapon.started -= OnSkillWeapon;
        playerInput.Control.SkillWeapon.performed -= OnSkillWeapon;
        playerInput.Control.SkillWeapon.canceled -= OnSkillWeapon;

        playerInput.Control.Block.started -= OnAbilityWeapon;
        playerInput.Control.Block.performed -= OnAbilityWeapon;
        playerInput.Control.Block.canceled -= OnAbilityWeapon;

        playerInput.Control.EquipUnequip.started -= OnEquipUnequip;
        playerInput.Control.EquipUnequip.canceled -= OnEquipUnequip;


        playerInput.Control.Interactor.started -= OnInteractor;
        playerInput.Control.Interactor.canceled -= OnInteractor;
        playerInput.Control.Disable();

    }
    private void OnDirection(InputAction.CallbackContext context)
    {
        mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directionMouseVector = (mouseVector - (Vector2)this.transform.position).normalized;
        AngleCalculate(directionMouseVector, ref angleMouseDirection, ref directionMouse);
        this.angleRotationPlayer = Vector2.SignedAngle(transform.right, directionMouseVector);
        this.angleRotationPlayer = (this.angleRotationPlayer + 360) % 360;
    }
    private void OnEquipUnequip(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isEquip_Unequip = true;
        }
        if (context.canceled)
        {
            isEquip_Unequip = false;
        }
    }
    private void OnInteractor(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isInteractor = true;

        }
        if (context.canceled)
        {
            isInteractor = false;
        }
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        AngleCalculateKeyboard(moveVector);
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (weaponHolder.Weapon == null) return;

        if (context.started && !BufferIsAttack)
        {
            if (StatusAnimation.StartRangeTrigger <= Core.Player.stateMachine.CurrentState.Status
            && Core.Player.stateMachine.CurrentState.Status <= StatusAnimation.EndRangeTrigger)
            {
                SetBufferAttack(true);
            }
            else if (weaponHolder.Weapon.CheckCanAttack(Core.Player))
            {
                isAttack = true;
            }
        }
        if (context.canceled)
        {
            isAttack = false;
        }
    }
    private void OnSkillWeapon(InputAction.CallbackContext context)
    {
        if (weaponHolder.Weapon == null)
        {
            return;
        }
        skill = SkillType.Special;
        if (context.started)
        {
            state = SkillState.Start;
            isSkill = true;
            weaponHolder.Weapon.SetAbility();
            abilityHolder.SetCanUseAbility(true);
        }
        else if (context.performed)
        {
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            isSkill = false;
        }
    }
    private void OnAbilityWeapon(InputAction.CallbackContext context)
    {
        if (weaponHolder.Weapon == null)
        {
            return;
        }
        skill = SkillType.Ability;
        if (context.started)
        {
            state = SkillState.Start;
            isSkill = true;
            weaponHolder.Weapon.SetAbility();
            abilityHolder.SetCanUseAbility(true);
        }
        else if (context.performed)
        {
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            isSkill = false;
        }
    }
    public void OnTakeDamage(Vector2 attackPosition)
    {
        CancelInvoke(nameof(ResetTakeDamage));
        Invoke(nameof(ResetTakeDamage), 0.1f);
        directionExternalityVector = ((attackPosition - (Vector2)this.transform.position)).normalized;
        AngleCalculateExternality(directionExternalityVector);
        isTakeDamage = true;
    }
    private void ResetTakeDamage()
    {
        isTakeDamage = false;
    }
    private void ChangeIsTakeDamage()
    {
        this.isTakeDamage = !this.isTakeDamage;
    }
    private void AngleCalculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        DirectionResolver.Calculate(directionVector, ref angle, ref direction);
    }
    public void AngleCalculateKeyboard(Vector2 directionKeyboardVector)
    {
        AngleCalculate(directionKeyboardVector, ref angleKeyboardDirection, ref directionKeyboard);
    }
    public void AngleCalculateMouse(Vector2 directionMouseVector)
    {
        AngleCalculate(directionMouseVector, ref angleMouseDirection, ref directionMouse);
    }
    public void AngleCalculateExternality(Vector2 directionExternalityVector)
    {
        AngleCalculate(directionExternalityVector, ref angleExternalityDirection, ref directionExternality);
    }
    #endregion


    #region 
    // public bool GetBufferAttack()
    // {
    //     if (bufferIsAttack)
    //     {
    //         weaponHolder.Weapon.CheckCanAttack(core.Player);
    //     }
    //     return bufferIsAttack;
    // }

    public void SetBufferAttack(bool bufferIsAttack)
    {
        this.BufferIsAttack = bufferIsAttack;
    }
    #endregion

}