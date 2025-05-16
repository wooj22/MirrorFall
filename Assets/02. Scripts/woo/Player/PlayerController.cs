using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] public PlayerState state; 
    [SerializeField] public PlayerWayState wayState;    
    [HideInInspector] public BaseState curState;               // state class
    [HideInInspector] public BaseState[] stateArr;             // state class array
    public enum PlayerState       // state class array ����, ������ enum
    {
        Idle, Walk, Hide, Thrrow, Hit, Die
    }

    public enum PlayerWayState { LeftUp, LeftDown, RightUp, RightDown }   // animation ���� ó�� state

    [Header("Player Stat")]
    [SerializeField] public int curHp;
    [SerializeField] private int initHp;
    [SerializeField] public float curSpeed;
    [SerializeField] private float initSpeed;

    [Header("Mirror Piece Collection")]
    [SerializeField] private int curPieceCount;
    [SerializeField] private bool piece1;
    [SerializeField] private bool piece2;
    [SerializeField] private bool piece3;
    [SerializeField] private bool piece4;
    [SerializeField] private bool piece5;

    [Header("Player State Flags")]
    public bool isDie;
    public bool isHit;
    public bool isAddiction;
    public bool isInHideZone;
    public bool isHide;                               // ### AI �е� �̰� get�ؼ� ������
    public bool isBright;
    public bool isHourglass;
    public bool isThrrow;

    [Header("Hit Data")]
    public float hitDurationTime;                       // �׸����� AI Hit �����ð�
    public float addictionDurationTime;                 // ������ AI �ߵ� �����ð�
    public float speedDownRate;                         // �����̶� ����� �� ������
    [SerializeField] private Color addictionColor;      // ������ hit color
    private Color originColor;

    [Header("Hide Data")]
    [SerializeField] private float invisibleDuration;
    public float invisibleTargetAlpha;
    public float originAlpha;
    private Coroutine invisibleCo;

    [Header("Apple Thrrow Data")]
    public Sprite appleOnUpSprite;
    public Sprite appleOnDownSprite;
    public int    lineSegmentCount;       // �������� �� ����
    public float  lineBetweenPoints;      // ������ ����
    public float  throwPower = 5f;        // ������ �� (�ӵ�)
    public GameObject applePrefab;        // ��� ������
    public Transform  appleSpawnPoint;    // ��� ���� ��ġ

    [Header("Bright Data")]
    public float brightDurationTime;

    [Header("Hourglass Data")]
    public float speedUpRate;
    public float hourglassDurationTime;

    [Header("Boss Scene Save Data")]
    [SerializeField] private int saveBossHp;
    [SerializeField] private int saveBossAngleIndex;
    [SerializeField] private List<string> saveBossInventoryItems;

    // controll
    [HideInInspector] public float moveX;
    [HideInInspector] public float moveY;
    [HideInInspector] public int lastDirX = 1;        // right : 1, left : -1 (üũ������ �ν����� ��� ����)
    [HideInInspector] public int lastDirY = 1;        // up : 1, down : -1 (üũ������ �ν����� ��� ����)

    // interation
    private FiledItem curFiledItem = null;
    private Door curDoor = null;
    private WarpMirror curWarpMirror = null;    
    private MirrorPiece curMirrorPiece = null;

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
            // player default update
            KeyInputUpdate();
            MoveInputUpdate();
            WayUpdate();

            // interation & pickup
            ItemInputCheak();
            SkillInputCheak();
            DoorInputCheak();
            WarpMirrorInputCheak();
            MirrorPieceInputCheak();

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
        // states ���
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

    /// ## ������ Retry�� ���� ���� ##
    public void SavePlayerData_ToBossScene()
    {
        saveBossHp = 1;
        saveBossAngleIndex = flashLight.GetCurIndex();
        saveBossInventoryItems = inventory.GetInventoryData();
        Debug.Log("SavePlayerData_ToBossScene");
    }

    /// ## TODO :: ������ Retry�� ���� ���� ##
    public void InitPlayer_ToBossScene()
    {
        isDie = false;
        ChangeState(PlayerState.Idle);

        curHp = saveBossHp;
        flashLight.SetCurIndex(saveBossAngleIndex);
        inventory.SetInventoryDate(saveBossInventoryItems);

        PlayerUIHandler.Instance.UpdateHpUI(curHp);
        Debug.Log("InitPlayer_ToBossScene");
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

    /*----------------- Interation ------------------------*/
    /// Door Input Cheak
    private void DoorInputCheak()
    {
        if (isInteractionKey && curDoor != null)
        {
            string nextSceneName = curDoor.DoorGetNextScene();
            if (nextSceneName == null) Debug.Log("���� �� �̸� �ȳ־��� ������");
            else DoorInteraction(nextSceneName);
        }
    }

    /// Door Interation - Scene Change
    private void DoorInteraction(string sceneName)
    {
        if (curPieceCount == 4)
        {
            if(SceneSwitch.Instance.GetCurrentScene() == "09_Boss")
                FadeManager.Instance.FadeOutSceneChange("10_GameClear");
            else
            {
                SavePlayerData_ToBossScene();       // ������ ���� data save
                FadeManager.Instance.FadeOutSceneChange("09_Boss");
            }
        }
        else FadeManager.Instance.FadeOutSceneChange(sceneName);
        curDoor = null;
    }

    /// Warp Mirror Input Cheak
    private void WarpMirrorInputCheak()
    {
        if (isInteractionKey && curWarpMirror != null)
        {
            string warpSceneName = curWarpMirror.WarpGetNextScene();
            if (warpSceneName == null) return;
            else WarpMirrorSkill(warpSceneName);
        }
    }

    /*----------------- Pick Up ------------------------*/
    /// Item Input Cheak
    private void ItemInputCheak()
    {
        // Item PickUp
        if (isInteractionKey && curFiledItem != null)
        {
            PickUpItem(curFiledItem);
        }
    }


    /// Pick Up Filed Item
    private void PickUpItem(FiledItem item)
    {
        if (inventory.IsInventoryFull())
        {
            Debug.Log("�κ��丮�� �� á���ϴ�.");
            return;
        }
        else
        {
            inventory.AddItem(item.name);
            curFiledItem = null;
            item.InteractionUIOff();
            Destroy(item.gameObject);
        }  
    }

    /// Mirror Piece Input Cheak
    private void MirrorPieceInputCheak()
    {
        if(isInteractionKey && curMirrorPiece != null)
        {
            PickUpMirrorPiece(curMirrorPiece);
        }
    }

    /// Pick Up Mirror Piece
    private void PickUpMirrorPiece(MirrorPiece piece)
    {
        int pieceNum = curMirrorPiece.GetPieceNum();
        switch (pieceNum)
        {
            case 1: piece1 = true; curPieceCount++; break;
            case 2: piece2 = true; curPieceCount++; break;
            case 3: piece3 = true; curPieceCount++; break;
            case 4: piece4 = true; curPieceCount++; break;
            case 5:
                {
                    piece5 = true; 
                    curPieceCount++; 
                    // ������ AI, �̵���� ���� Ʈ���� 
                    break;
                }
            default: break;
        }

        GameManager.Instance.CollectPiece(pieceNum);    // game data

        curMirrorPiece = null;
        piece.InteratcionUIOff();
        Destroy(piece.gameObject);
    }

    /*------------------------- Skill -------------------------------*/
    /// Skill Input Cheak
    public void SkillInputCheak()
    {
        // Item ��� -> ��ų
        if (isItem1Key)
        {
            string itemName = inventory.GetIndexItemName(1);
            if (itemName == null)
            {
                Debug.Log("1�� ���Կ� �������� �����ϴ�");
                return;
            }
            SkillInvocation(itemName);
            inventory.RemoveItem(1);
        }

        if (isItem2Key)
        {
            string itemName = inventory.GetIndexItemName(2);
            if (itemName == null)
            {
                Debug.Log("2�� ���Կ� �������� �����ϴ�");
                return;
            }
            SkillInvocation(itemName);
            inventory.RemoveItem(2);
        }

        if (isItem3Key)
        {
            string itemName = inventory.GetIndexItemName(3);
            if (itemName == null)
            {
                Debug.Log("3�� ���Կ� �������� �����ϴ�");
                return;
            }
            SkillInvocation(itemName);
            inventory.RemoveItem(3);
        }
    }

    /// Skill �ߵ�
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

    /// 1. Apple Thrrow ���� (state)
    public void AppleThrrowSkill()
    {
        Debug.Log("��� ����");
        Vector2 start = appleSpawnPoint.position;
        Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distance = Vector2.Distance(start, end);
        float height = Mathf.Max(1f, distance / 2f);

        StartCoroutine(MoveAppleParabola(start, end, height, 0.5f)); // duration�� 0.5�� ����
    }

    private IEnumerator MoveAppleParabola(Vector2 start, Vector2 end, float height, float duration)
    {
        float time = 0f;

        GameObject apple = Instantiate(applePrefab, start, Quaternion.identity);
        Apple appleController = apple.GetComponent<Apple>();

        while (time < duration && !appleController.isGround)
        {
            float t = time / duration;

            Vector2 pos = Vector2.Lerp(start, end, t);
            pos.y += 4 * height * t * (1 - t);        // ������ ����

            apple.transform.position = pos;

            time += Time.deltaTime;
            yield return null;
        }

        //apple.transform.position = end;
    }

    /// 2. Bright Skill ���
    public void BrightSkill()
    {
        flashLight.Brightness();
    }


    /// 3. HourGalss Skill �̼� ����
    public void HourGlassSkill()
    {
        isHourglass = true;
        curSpeed = initSpeed * speedUpRate;
    }

    /// �̼� ���󺹱�
    public void ReturnHourGalss()
    {
        curSpeed = initSpeed;
        isHourglass = false;
    }

    /// 4. �ſ� ���� ��ų
    private void WarpMirrorSkill(string warpSceneName)
    {
        if (curPieceCount == 4)
        {
            if (SceneSwitch.Instance.GetCurrentScene() == "09_Boss")
                FadeManager.Instance.FadeOutSceneChange("10_GameClear");
            else
            {
                SavePlayerData_ToBossScene();       // ������ ���� data save
                FadeManager.Instance.FadeOutSceneChange("09_Boss");
            }
        }
        else FadeManager.Instance.FadeOutSceneChange(warpSceneName);
        curWarpMirror = null;

        // TODO :: �ش� ���� �ſ� ������ �̵� !!! ����� �������� @@@
    }

    /// 5. Hide ����ȭ (state)
    public void HideInvisible(float targetAlpha)
    {
        // �ߺ� ���� ����
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

        // ������
        if (aiTag == "L")
        {
            if (!isHit)
            {
                // �ߵ�
                Addiction();
                Invoke(nameof(ReturnAddiction), addictionDurationTime);
            }
        }

        // ����
        if (aiTag == "K")
        {
            // Hit���϶��� �ǰ��� ���� ���� (Hit State���� ����)
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

    // ������ Hit => �ߵ� (����)
    private void Addiction()
    {
        curSpeed = initSpeed * speedDownRate;
        sr.color = addictionColor;
        isAddiction = true;
    }

    /// �ߵ� ���󺹱�
    private void ReturnAddiction()
    {
        curSpeed = initSpeed;
        sr.color = originColor;
        isAddiction = false;
    }

    /// TODO :: Die ���� ó��
    public void Die()
    {
        // ������ �״� 1�� Ʈ����
        if(SceneSwitch.Instance.GetCurrentScene() == "09_Boss")
        {
            GameManager.Instance.BossPlayerDie();
        }
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
            curFiledItem.InteractionUIOn();
        }

        // door interation
        if (collision.CompareTag("Door"))
        {
            curDoor = collision.GetComponent<Door>();
            curDoor.InterationUIOn();
        }

        // warp mirror interation
        if (collision.CompareTag("WarpMirror"))
        {
            curWarpMirror = collision.GetComponent<WarpMirror>();
            curWarpMirror.InterationUIOn();
        }

        // mirror piece interation
        if (collision.CompareTag("MirrorPiece"))
        {
            curMirrorPiece = collision.GetComponent<MirrorPiece>();
            curMirrorPiece.InteractionUIOn();
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
                curFiledItem.InteractionUIOff();
                curFiledItem = null;
            }
        }

        // door interation
        if (collision.CompareTag("Door"))
        {
            if (curDoor != null)
            {
                curDoor.InterationUIOff();
                curDoor = null;
            }
        }

        // warp mirror interation
        if (collision.CompareTag("WarpMirror"))
        {
            if (curWarpMirror != null)
            {
                curWarpMirror.InterationUIOff();
                curWarpMirror = null;
            }
        }

        // mirror piece interation
        if (collision.CompareTag("MirrorPiece"))
        {
            if (curMirrorPiece != null)
            {
                curMirrorPiece.InteratcionUIOff();
                curMirrorPiece = null;
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
