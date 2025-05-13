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
    public enum PlayerState       // state class array ����, ������ enum
    {
        Idle, Walk, Hide, Lure, Hit, Die
    }

    public enum PlayerWayState { LeftUp, LeftDown, RightUp, RightDown }   // animation ���� ó�� state

    [Header("Player Stat")]
    public int hp;
    public int maxHp;
    public float speed;
    public float initSpeed;
    public float hitTime;              // �׸����� AI Hit �����ð�
    public float hitSpeedDownTime;     // ������ AI Hit �����ð�
    public float speedDownRate;        // �����̶� ����� �� ������

    [Header("Player State Flags")]
    public bool isDie;
    public bool isHit;
    public bool isHitSpeedDown;

    [Header("Player Key Input Flags")]
    [HideInInspector] public bool isMoveLKey;
    [HideInInspector] public bool isMoveRKey;
    [HideInInspector] public bool isMoveUpKey;
    [HideInInspector] public bool isMoveDownKey;
    [HideInInspector] public bool isInteractionKey;       // ��ȣ�ۿ�, ��ų �κ��� 1ȸ���̶� ���Ŀ� ��� �����Ҷ� ���� ���ɼ� ����.
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
    [SerializeField] private Color addictionColor;      // ������ hit color
    private Color originColor;

    // controll
    [HideInInspector] public float moveX;
    [HideInInspector] public float moveY;
    [HideInInspector] public int lastDirX = 1;        // right : 1, left : -1 (üũ������ �ν����� ��� ����)
    [HideInInspector] public int lastDirY = 1;        // up : 1, down : -1 (üũ������ �ν����� ��� ����)
    
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

            // Test (������ �ӽ� �ڵ�)
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
        // states ���
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

        // ������
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

        // ����
        if (aiTag == "K")
        {
            // Hit���϶��� �ǰ��� ���� ���� (Hit State���� ����)
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

    /// Speed ���󺹱� (������ hit)
    private void InitSpeed()
    {
        speed = initSpeed;
        sr.color = originColor;
        isHitSpeedDown = false;
    }

    /// TODO :: Die ���� ó��
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
