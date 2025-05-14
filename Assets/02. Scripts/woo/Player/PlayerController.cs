using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("[State]")]
    [SerializeField] public PlayerState state; 
    [SerializeField] public PlayerWayState wayState;    
    public BaseState curState;                    // state class
    public BaseState[] stateArr;                  // state class array
    public enum PlayerState       // state class array 접근, 관리용 enum
    {
        Idle, Walk, Hide, Thrrow, Hit, Die
    }

    public enum PlayerWayState { LeftUp, LeftDown, RightUp, RightDown }   // animation 방향 처리 state

    [Header("Player Stat")]
    public int curHp;
    public int initHp;
    public float curSpeed;
    public float initSpeed;

    [Header("Hit Data")]
    public float hitDurationTime;                       // 그림힐데 AI Hit 유지시간
    public float addictionDurationTime;                 // 난쟁이 AI 중독 유지시간
    public float speedDownRate;                         // 난쟁이랑 닿았을 때 감속율
    [SerializeField] private Color addictionColor;      // 난쟁이 hit color
    private Color originColor;

    [Header("Hide Data")]
    [SerializeField] private float invisibleDuration;
    public float invisibleTargetAlpha;
    public float originAlpha;
    private Coroutine invisibleCo;

    [Header("Apple Thrrow Data")]
    public Sprite appleOnUpSprite;
    public Sprite appleOnDownSprite;
    public int    lineSegmentCount;       // 포물선의 점 개수
    public float  lineBetweenPoints;      // 포물선 간격
    public float  throwPower = 5f;        // 던지는 힘 (속도)
    public GameObject applePrefab;        // 사과 프리팹
    public Transform  appleSpawnPoint;    // 사과 생성 위치

    [Header("Bright Data")]
    public float brightDurationTime;

    [Header("Hourglass Data")]
    public float speedUpRate;
    public float hourglassDurationTime;

    [Header("Player State Flags")]
    public bool isDie;
    public bool isHit;
    public bool isAddiction;
    public bool isInHideZone;
    public bool isHide;                               // ### AI 분들 이거 get해서 쓰세여
    public bool isBright;
    public bool isHourglass;
    public bool isThrrow;

    // controll
    [HideInInspector] public float moveX;
    [HideInInspector] public float moveY;
    [HideInInspector] public int lastDirX = 1;        // right : 1, left : -1 (체크용으로 인스펙터 잠깐 빼둠)
    [HideInInspector] public int lastDirY = 1;        // up : 1, down : -1 (체크용으로 인스펙터 잠깐 빼둠)

    // item interation
    private FiledItem curFiledItem = null;

    // Components
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator ani;
    [HideInInspector] public FlashLight flashLight;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public Inventory inventory;

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
    public KeyCode Item1Key = KeyCode.Alpha1;
    public KeyCode Item2Key = KeyCode.Alpha2;
    public KeyCode Item3Key = KeyCode.Alpha3;


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
        lineRenderer = GetComponent<LineRenderer>();
        inventory = GetComponent<Inventory>();

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

            // Item PickUp
            if (isInteractionKey && curFiledItem != null)
            {
                PickUpItem(curFiledItem); 
            }

            // Item 사용 -> 스킬
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                string itemName = inventory.GetIndexItemName(1);
                if (itemName == null)
                {
                    Debug.Log("1번 슬롯에 아이템이 없습니다");
                    return;
                }
                SkillInvocation(itemName);
                inventory.RemoveItem(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                string itemName = inventory.GetIndexItemName(2);
                if (itemName == null)
                {
                    Debug.Log("1번 슬롯에 아이템이 없습니다");
                    return;
                }
                SkillInvocation(itemName);
                inventory.RemoveItem(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                string itemName = inventory.GetIndexItemName(3);
                if (itemName == null)
                {
                    Debug.Log("1번 슬롯에 아이템이 없습니다");
                    return;
                }
                SkillInvocation(itemName);
                inventory.RemoveItem(3);
            }

            // Test (아이템 스킬 사용)     // TODO :: 인벤토리 연계
            //if(Input.GetKeyDown(KeyCode.Alpha1)) ChangeState(PlayerState.Thrrow);
            //if (Input.GetKeyDown(KeyCode.Alpha2)) BrightSkill();
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    HourGlassSkill();
            //    Invoke(nameof(ReturnHourGalss), hourglassDurationTime);
            //}

            // Test (Attack)
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
        stateArr[(int)PlayerState.Thrrow] = new Thrrow(this);
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
        isHideKey = Input.GetKey(hideKey);

        isInteractionKey = Input.GetKeyDown(interationKey);
        isItem1Key = Input.GetKeyDown(Item1Key);
        isItem2Key = Input.GetKeyDown(Item2Key);
        isItem3Key = Input.GetKeyDown(Item3Key);
    }

    /// Plater Init
    public void PlayerInit()
    {
        // player stat init
        curHp = initHp;
        curSpeed = initSpeed;
        isDie = false;
        isHit = false;

        // player controll init
        moveX = 0;
        moveY = 0;
        lastDirX = 1;

        originColor = sr.color;

        // UI
        PlayerUIHandler.Instance.UpdateHpUI(curHp);

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


    /*----------------- Item Interation ------------------------*/


    // Pick Up Filed Item
    private void PickUpItem(FiledItem item)
    {
        if (inventory.IsInventoryFull())
        {
            Debug.Log("인벤토리가 꽉 찼습니다.");
            return;
        }
        else
        {
            inventory.AddItem(item.name);
            curFiledItem = null;
            item.InterationUIOff();
            Destroy(item.gameObject);
        }  
    }


    /*------------------------- Skill -------------------------------*/
    /// Skill 발동
    private void SkillInvocation(string skill)
    {
        switch (skill)
        {
            case "Apple_Item":
                ChangeState(PlayerState.Thrrow);
                break;
            case "Bright_Item":
                BrightSkill();
                break;
            case "Hourglass_Item":
                HourGlassSkill();
                Invoke(nameof(ReturnHourGalss), hourglassDurationTime);
                break;
            default:
                break;
        }
    }

    /// 1. Apple Thrrow 유인 (state)
    public void AppleThrrow()
    {
        Debug.Log("사과 던짐");
        Vector2 start = appleSpawnPoint.position;
        Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distance = Vector2.Distance(start, end);
        float height = Mathf.Max(1f, distance / 2f);

        StartCoroutine(MoveAppleParabola(start, end, height, 0.5f)); // duration은 0.5초 정도
    }

    private IEnumerator MoveAppleParabola(Vector2 start, Vector2 end, float height, float duration)
    {
        float time = 0f;

        GameObject apple = Instantiate(applePrefab, start, Quaternion.identity);

        while (time < duration)
        {
            float t = time / duration;

            Vector2 pos = Vector2.Lerp(start, end, t);
            pos.y += 4 * height * t * (1 - t); // 포물선 공식과 동일

            apple.transform.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        apple.transform.position = end;

        // 여기서 충돌 처리나 터지는 효과 등 추가 가능
    }

    /// 2. Bright Skill 밝기
    public void BrightSkill()
    {
        flashLight.Brightness();
    }


    /// 3. HourGalss Skill 이속 버프
    public void HourGlassSkill()
    {
        isHourglass = true;
        curSpeed = initSpeed * speedUpRate;
    }

    /// 이속 원상복귀
    public void ReturnHourGalss()
    {
        curSpeed = initSpeed;
        isHourglass = false;
    }

    /// Hide 투명화 (state)
    public void HideInvisible(float targetAlpha)
    {
        // 중복 실행 방지
        if (invisibleCo != null)
            StopCoroutine(invisibleCo);

        invisibleCo = StartCoroutine(FadeInvisible(targetAlpha));
    }

    IEnumerator FadeInvisible(float targetAlpha)
    {
        float startAlpha = sr.color.a;
        float time = 0f;
        Color color = sr.color;

        while (time < invisibleDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / invisibleDuration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            sr.color = color;
            yield return null;
        }

        // last set
        color.a = targetAlpha;
        sr.color = color;
        invisibleCo = null;
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
                // 중독
                Addiction();
                Invoke(nameof(ReturnAddiction), addictionDurationTime);
            }
        }

        // 마녀
        if (aiTag == "K")
        {
            // Hit중일때는 피격을 받지 않음 (Hit State에서 관리)
            if (!isHit)
            {
                curHp --;
                PlayerUIHandler.Instance.UpdateHpUI(curHp);

                if (curHp <= 0)
                {
                    curHp = 0;
                    ChangeState(PlayerState.Die);
                }
                else
                {
                    ChangeState(PlayerState.Hit);
                }
            }
        } 
    }

    // 난쟁이 Hit => 중독 (감속)
    private void Addiction()
    {
        curSpeed = initSpeed * speedDownRate;
        sr.color = addictionColor;
        isAddiction = true;
    }

    /// 중독 원상복귀
    private void ReturnAddiction()
    {
        curSpeed = initSpeed;
        sr.color = originColor;
        isAddiction = false;
    }

    /// TODO :: Die 로직 처리
    public void Die()
    {
        
    }

    

    /*------------------------- Collision -------------------------------*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // hide zone
        if (collision.gameObject.CompareTag("WallHideZone")){
            isInHideZone = true;
        }

        // item interation
        if (collision.CompareTag("Item"))
        {
            curFiledItem = collision.GetComponent<FiledItem>();
            curFiledItem.InterationUIOn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // hide zone
        if (collision.gameObject.CompareTag("WallHideZone"))
        {
            isInHideZone = false;
        }

        // item interation
        if (collision.CompareTag("Item"))
        {
            if (curFiledItem != null)
            {
                curFiledItem.InterationUIOff();
                curFiledItem = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }
}
