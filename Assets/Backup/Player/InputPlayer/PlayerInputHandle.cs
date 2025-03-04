using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
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
    [SerializeField] private NewPlayer player;
    [SerializeField] private PlayerInput playerInput;

    //[SerializeField] private Vector2 moveVector;
    //[SerializeField] private Vector2 mouseVector;

    //[Header("Direction by Keyboard")]
    //[SerializeField] private Vector2 directionKeyboardVector;
    //[SerializeField] private float angleKeyboardDirection;
    //[SerializeField] private int directionKeyboard;
    
    //[Header("Direction by Externality")]
    //[SerializeField] private Vector2 directionExternalityVector;
    //[SerializeField] private float angleExternalityDirection;
    //[SerializeField] private int directionExternality;

    //[Header("Direction by Mouse")]
    //[SerializeField] private Vector2 directionMouseVector;
    //[SerializeField] private int directionMouse;
    //[SerializeField] private float angleMouseDirection;
    //[SerializeField] private float angleRotationPlayer;



    //[Header("Bool Value")]
    //[SerializeField] private bool isAttack;
    //[SerializeField] private bool isSkill;
    //[SerializeField] private bool isDisadvantage;
    //[SerializeField] private bool isTakeDamage;
    //[SerializeField] private bool isEquip_Unequip= false;
    //[SerializeField] private bool isInteractor = false;

    [Header("Enum Value")]
    [SerializeField] private SkillState state;
    [SerializeField] private SkillType skill;
    [SerializeField] private DisadvantageState disadvantage;

    #region Get value 
    //public Vector2 MoveVector { get => moveVector; }
    //public Vector2 MouseVector { get => mouseVector; }
    //public Vector2 DirectionMouseVector { get => directionMouseVector; }
    //public int DirectionMouse { get => directionMouse; }
    //public float AngleRotationPlayer { get => angleRotationPlayer; }
    //public float AngleLookDirection { get => angleMouseDirection; }
    //public bool IsAttack { get => isAttack; }
    //public SkillState State { get => state; }
    //public SkillType Skill { get => skill; }
    //public bool IsSkill { get => isSkill; }
    //public PlayerInput PlayerInput { get => playerInput; }
    //public bool IsDisadvantage { get => isDisadvantage; }
    //public bool IsTakeDamage { get => isTakeDamage; }
    //public bool IsEquip_Unequip { get => isEquip_Unequip; }
    //public bool IsInteractor { get => isInteractor; }
    //public Vector2 DirectionKeyboardVector { get => directionKeyboardVector; }
    //public float AngleKeyboardDirection { get => angleKeyboardDirection; }
    //public int DirectionKeyboard { get => directionKeyboard; }
    //public Vector2 DirectionExternalityVector { get => directionExternalityVector; }
    //public float AngleExternalityDirection { get => angleExternalityDirection; }
    //public int DirectionExternality { get => directionExternality; }
    #endregion


    #endregion
    private void Awake()
    {
        player = GetComponentInParent<NewPlayer>();
        playerInput = new PlayerInput();
    }
    private void Start()
    {
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
        //playerInput.Control.Interactor.performed += OnInteractor;
        playerInput.Control.Interactor.canceled += OnInteractor;
    }

    #region OnMethod
    private void OnEnable()
    {
        playerInput.Control.Enable();
    }
    private void OnDisable()
    {
        playerInput.Control.Disable();
    }
    private void OnDirection(InputAction.CallbackContext context)
    {
        player.StatsBehavior.MouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player.StatsBehavior.DirectionMouseVector = (player.StatsBehavior.MouseVector - (Vector2)this.transform.position).normalized;
        AngleCalculate(player.StatsBehavior.DirectionMouseVector,ref player.StatsBehavior.AngleMouseDirection,ref player.StatsBehavior.DirectionMouse);
        player.StatsBehavior.AngleRotationPlayer = Vector2.SignedAngle(transform.right, player.StatsBehavior.DirectionMouseVector);
        player.StatsBehavior.AngleRotationPlayer = (player.StatsBehavior.AngleRotationPlayer + 360) % 360;
    }
    private void OnEquipUnequip(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.StatsBehavior.IsEquip_Unequip = true;
        }
        if(context.canceled)
        {
            player.StatsBehavior.IsEquip_Unequip = false;
        }
    }
    private void OnInteractor(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.StatsBehavior.IsInteractor = true;
            Debug.Log(" Start " + Time.time);
        }   
        if (context.canceled)
        {
            player.StatsBehavior.IsInteractor = false;
            Debug.Log("Canceled " + Time.time);
        }
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        player.StatsBehavior.MoveVector = context.ReadValue<Vector2>();
        AngleCalculateKeyboard(player.StatsBehavior.MoveVector);
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.StatsBehavior.IsAttack = true;
        }
        if (context.canceled)
        {
            player.StatsBehavior.IsAttack = false;
        }
    }
    private void OnSkillWeapon(InputAction.CallbackContext context)
    {
        if(player.Core.WeaponHolder.Weapon == null)
        {
            return;
        }
        skill = SkillType.Special;
        if (context.started)
        {
            state = SkillState.Start;
            player.StatsBehavior.IsSkill = true;
            player.Core.WeaponHolder.Weapon.SetAbility();
            player.Core.AbilityHolder.SetCanUseAbility(true);
        }
        else if (context.performed)
        {
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            player.StatsBehavior.IsSkill = false;
        }
    }
    private void OnAbilityWeapon(InputAction.CallbackContext context)
    {
        if (player.Core.WeaponHolder.Weapon == null)
        {
            return;
        }
        skill = SkillType.Ability;
        if (context.started)
        {
            state = SkillState.Start;
            player.StatsBehavior.IsSkill = true;
            player.Core.WeaponHolder.Weapon.SetAbility();
            player.Core.AbilityHolder.SetCanUseAbility(true);
        }
        else if (context.performed)
        {
            state = SkillState.Cast;
        }
        else if (context.canceled)
        {
            state = SkillState.Do;
            player.StatsBehavior.IsSkill = false;
        }
    }
    public void OnTakeDamage(Vector2 attackPosition)
    {
        ChangeIsTakeDamage();
        Invoke(nameof(ChangeIsTakeDamage), 0.2f);
        player.StatsBehavior.DirectionExternalityVector= ((attackPosition - (Vector2)this.transform.position)).normalized;
        AngleCalculateExternality(player.StatsBehavior.DirectionExternalityVector);
    }
    private void ChangeIsTakeDamage()
    {
        player.StatsBehavior.IsTakeDamage = !player.StatsBehavior.IsTakeDamage;
    }
    private void AngleCalculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle, ref direction);
    }
    protected void DirectionCaculate(float angle, ref int direction)
    {
        if ((angle > 22 && angle <= 67))
        {
            direction = 0;
        }
        else if (angle > 67 && angle <= 112)
        {
            direction = 1;
        }
        else if (angle > 112 && angle <= 157)
        {
            direction = 2;

        }
        else if (angle > 157 && angle <= 202)
        {
            direction = 3;
        }
        else if (angle > 202 && angle <= 247)
        {
            direction = 4;
        }
        else if (angle > 247 && angle <= 292)
        {
            direction = 5;
        }
        else if (angle > 292 && angle <= 337)
        {
            direction = 6;
        }
        else if (angle > 337 || angle < 22)
        {
            direction = 7;
        }
    }
    public void AngleCalculateKeyboard(Vector2 directionKeyboardVector)
    {
        AngleCalculate(directionKeyboardVector, ref player.StatsBehavior.AngleKeyboardDirection, ref player.StatsBehavior.DirectionKeyboard);
    }
    public void AngleCalculateMouse(Vector2 directionMouseVector)
    {
        AngleCalculate(directionMouseVector, ref player.StatsBehavior.AngleKeyboardDirection, ref player.StatsBehavior.DirectionKeyboard);
    }
    public void AngleCalculateExternality(Vector2 directionExternalityVector)
    {
        AngleCalculate(directionExternalityVector, ref player.StatsBehavior.AngleKeyboardDirection, ref player.StatsBehavior.DirectionKeyboard);
    }
    #endregion
}