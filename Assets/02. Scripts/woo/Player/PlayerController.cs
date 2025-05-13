using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] public PlayerState state; 
    [SerializeField] public PlayerWayState wayState;    
    public BaseState curState;                    // state class
    public BaseState[] stateArr;                  // state class array
    public enum PlayerState       // state class array 접근, 관리용 enum
    {
        Idle, Walk, Hide, Lure, Hit, Die
    }

    public enum PlayerWayState { LeftUp, LeftDown, RightUp, RightDown }   // animation 방향 처리 state

    [Header("Player Stat")]
    public int hp;
    public int maxHp;
    public float speed;
    public float initSpeed;
    public float hitTime;              // 그림힐데 AI Hit 유지시간
    public float hitSpeedDownTime;     // 난쟁이 AI Hit 유지시간
    public float speedDownRate;        // 난쟁이랑 닿았을 때 감속율

    [Header("Player State Flags")]
    public bool isDie;
    public bool isHit;
    public bool isHitSpeedDown;

    [Header("Player Key Input Flags")]
    [HideInInspector] public bool isMoveLKey;
    [HideInInspector] public bool isMoveRKey;
    [HideInInspector] public bool isMoveUpKey;
    [HideInInspector] public bool isMoveDownKey;
    [HideInInspector] public bool isInteractionKey;       // 상호작용, 스킬 부분은 1회성이라 추후에 기능 구현할때 수정 가능성 있음.
    [HideInInspector] public bool isHideKey;
    [HideInInspector] public bool isItem1Key;
    [HideInInspector] public bool isItem2Key;
    [HideInInspector] public bool isItem3Key;

    [Header("Key Bindings")]
    public KeyCode moveL = KeyCode.A;
    public KeyCode moveR = KeyCode.D;
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode interationKey = KeyCode.F;
    public KeyCode hideKey = KeyCode.LeftShift;
    public KeyCode Item1Key = KeyCode.Alpha2;
    public KeyCode Item2Key = KeyCode.Alpha3;
    public KeyCode Item3Key = KeyCode.Alpha1;


    [Header("Controll")]
    [SerializeField] private Color addictionColor;      // 난쟁이 hit color
    private Color originColor;

    // controll
    [HideInInspector] public float moveX;
    [HideInInspector] public float moveY;
    [HideInInspector] public int lastDirX = 1;        // right : 1, left : -1 (체크용으로 인스펙터 잠깐 빼둠)
    [HideInInspector] public int lastDirY = 1;        // up : 1, down : -1 (체크용으로 인스펙터 잠깐 빼둠)
    
    // Components
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator ani;
    [HideInInspector] public FlashLight flashLight;


    /*------------------------- Function -------------------------------*/
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        AddFSM();
    }

    private void Start()
    {
        // get component
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        flashLight = GetComponent<FlashLight>();

        // player init
        PlayerInit();
    }

    private void Update()
    {
        if (!isDie)
        {
            KeyInputUpdate();
            MoveInputUpdate();
            WayUpdate();

            // Test (디버깅용 임시 코드)
            if (Input.GetKeyDown(KeyCode.K)) Hit("K");
            if (Input.GetKeyDown(KeyCode.L)) Hit("L");

            // state update logic
            curState?.ChangeStateLogic();
            curState?.UpdateLigic();
        }
    }

    /// FSM Setting
    private void AddFSM()
    {
        // states 등록
        stateArr = new BaseState[System.Enum.GetValues(typeof(PlayerState)).Length];

        stateArr[(int)PlayerState.Idle] = new Idle(this);
        stateArr[(int)PlayerState.Walk] = new Walk(this);
        stateArr[(int)PlayerState.Hide] = new Hide(this);
        stateArr[(int)PlayerState.Lure] = new Lure(this);
        stateArr[(int)PlayerState.Hit] = new Hit(this);
        stateArr[(int)PlayerState.Die] = new Die(this);
    }

    /// State Change
    public void ChangeState(PlayerState state)
    {
        curState?.Exit();
        curState = stateArr[(int)state];
        this.state = state;
        curState?.Enter();
    }

    /// Key Input Handler
    public void KeyInputUpdate()
    {
        isMoveLKey = Input.GetKey(moveL);
        isMoveRKey = Input.GetKey(moveR);
        isMoveUpKey = Input.GetKey(moveUp);
        isMoveDownKey = Input.GetKey(moveDown);

        isInteractionKey = Input.GetKeyDown(interationKey);
        isHideKey = Input.GetKeyDown(hideKey);
        isItem1Key = Input.GetKeyDown(Item1Key);
        isItem2Key = Input.GetKeyDown(Item2Key);
        isItem3Key = Input.GetKeyDown(Item3Key);
    }

    /// Plater Init
    public void PlayerInit()
    {
        // player stat init
        hp = maxHp;
        speed = initSpeed;
        isDie = false;
        isHit = false;

        // player controll init
        moveX = 0;
        moveY = 0;
        lastDirX = 1;

        originColor = sr.color;

        // UI
        PlayerUIHandler.Instance.UpdateHduUI(hp);

        // player state init
        ChangeState(PlayerState.Idle);
    }

    /// Player Move Input
    public void MoveInputUpdate()
    {
        // input
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        // last Direction save
        if (moveX != 0)
            lastDirX = moveX > 0 ? 1 : -1;

        if (moveY != 0)
            lastDirY = moveY > 0 ? 1 : -1;
    }

    /// Player Way Update (Debuging)
    public void WayUpdate()
    {
        if (lastDirX == -1 && lastDirY == 1)
            wayState = PlayerWayState.LeftUp;
        else if (lastDirX == -1 && lastDirY == -1)
            wayState = PlayerWayState.LeftDown;
        else if (lastDirX == 1 && lastDirY == 1)
            wayState = PlayerWayState.RightUp;
        else if (lastDirX == 1 && lastDirY == -1)
            wayState = PlayerWayState.RightDown;
    }


    /*------------------------- Event -------------------------------*/
    /// Hit
    public void Hit(string aiTag)
    {
        if(isDie) return;

        // 난쟁이
        if (aiTag == "L")
        {
            if (!isHit)
            {
                speed = initSpeed * speedDownRate;
                sr.color = addictionColor;
                isHitSpeedDown = true;
                Invoke(nameof(InitSpeed), hitSpeedDownTime);
            }
        }

        // 마녀
        if (aiTag == "K")
        {
            // Hit중일때는 피격을 받지 않음 (Hit State에서 관리)
            if (!isHit)
            {
                hp --;
                PlayerUIHandler.Instance.UpdateHduUI(hp);

                if (hp <= 0)
                {
                    hp = 0;
                    ChangeState(PlayerState.Die);
                }
                else
                {
                    ChangeState(PlayerState.Hit);
                }
            }
        } 
    }

    /// Speed 원상복귀 (난쟁이 hit)
    private void InitSpeed()
    {
        speed = initSpeed;
        sr.color = originColor;
        isHitSpeedDown = false;
    }

    /// TODO :: Die 로직 처리
    public void Die()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}
