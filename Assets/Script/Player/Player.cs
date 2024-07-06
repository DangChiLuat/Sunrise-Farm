using System.Collections;
using UnityEngine;

public class Player : Entity
{

    [Header("Attack details")]
    public Vector2[] attackMovement;
    public static Player instance;
    [Header("Input System")]
    [SerializeField] private GameInput gameInput;
    public bool canMove = true;
    public bool isBusy { get; private set; }
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public Vector2 dashDir { get; private set; }

    public Vector2 lastFacingDir { get; private set; } = Vector2.down;

    public SkillManager skill{  get; private set; }







    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
        dashState = new PlayerDashState(this, stateMachine, "Dash");

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
        {

            dashUsageTimer = dashCooldown;

            Vector2 inputVector = gameInput.GetMovementVectorNormalized();
            if (inputVector != Vector2.zero)
            {
                dashDir = inputVector;
                if (inputVector.x == 1 || inputVector.x == -1 || inputVector.y == 1 || inputVector.y == -1)
                {

                    anim.SetFloat("DashX", inputVector.x);
                    anim.SetFloat("DashY", inputVector.y);

                }
            }
            else
            {
                dashDir = lastFacingDir; // Default to facing direction if no input
                if (lastFacingDir.x == 1 || lastFacingDir.x == -1 || lastFacingDir.y == 1 || lastFacingDir.y == -1)
                {

                    anim.SetFloat("DashX", lastFacingDir.x);
                    anim.SetFloat("DashY", lastFacingDir.y);

                }
            }
            stateMachine.ChangeState(dashState);
        }
    }

    public bool HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        return inputVector != Vector2.zero;
    }
    public void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        if (rb != null)
        {
            rb.velocity = new Vector3(inputVector.x, inputVector.y, 0f) * moveSpeed;
            anim.SetFloat("MoveX", rb.velocity.x);
            anim.SetFloat("MoveY", rb.velocity.y);

            if (inputVector != Vector2.zero)
            {
                lastFacingDir = inputVector;
            }
        }
        else
        {
            Debug.LogError("Rigidbody component is missing on the player.");
        }
        if (inputVector.x == 1 || inputVector.x == -1 || inputVector.y == 1 || inputVector.y == -1)
        {
            if (canMove)
            {
                anim.SetFloat("LastMoveX", inputVector.x);
                anim.SetFloat("LastMoveY", inputVector.y);
                anim.SetFloat("AttackX", inputVector.x);
                anim.SetFloat("AttackY", inputVector.y);
            }

        }
    }

    /*
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameInput gameInput;
    public Rigidbody2D rb;
    public Animator Aim;
    public static Player instance;

    public string areaTransitonName;

    public bool canMove = true;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (canMove)
        {
            HandleMovement();

        }
        else
        {
            HandleInteractions();
        }
        Attack();
        DontAttack();

    }
    public void HandleInteractions()
    {
        Vector2 inputVvector = gameInput.GetMovementVectorNormalized();
    }
    public void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0f);
        //float moveDistance = moveSpeed * Time.deltaTime;
        //transform.position += moveDir * moveDistance;
        rb.velocity = new Vector3(inputVector.x, inputVector.y, 0f) * moveSpeed;
        Aim.SetFloat("MoveX", rb.velocity.x);
        Aim.SetFloat("MoveY", rb.velocity.y);
    }

    public void Attack()
    {

            if (Input.GetKey(KeyCode.K))
            {
                Debug.Log("click");
                Aim.SetBool("Attack",true);
            }


    }
    public void DontAttack()
    {

        if (Input.GetKey(KeyCode.M))
        {
            Debug.Log("click");
            Aim.SetBool("Attack", false);
        }


    }
    */
}
