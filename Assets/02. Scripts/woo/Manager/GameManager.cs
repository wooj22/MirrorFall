using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    private HashSet<int> collectedMirror = new HashSet<int>();    // mirror piece collected data
    private HashSet<string> collectedItemIds = new HashSet<string>(); // item piece collected data
    //private List<string> savedInventoryItems = null;            // inventory save data
    //private bool hasSavedInventory = false;                     // 아 이거 뭐더라 나중에 확인해봐

    private PlayerController player;
    public static GameManager Instance;         

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // 방 왔다갔다 하는거땜에 메인 BGM 실행은 여기에 둠 (나머지는 soundmanager에서 controll)
        //SoundManager2.Instance.SetBGM("BGM_InGame");
        //SoundManager2.Instance.PlayBGM();
    }

    /// 거울조각 수집
    public void CollectPiece(int pieceNum)
    {
        collectedMirror.Add(pieceNum);
    }

    /// 거울 조각 삭제 (5번 init)
    public void UnCollectPiece(int pieceNum)
    {
        collectedMirror.Remove(pieceNum);
    }

    /// 수집 여부 확인
    public bool HasCollected(int pieceNum)
    {
        return collectedMirror.Contains(pieceNum);
    }

    /// 아이템 수집 등록
    public void RegisterCollectedItem(string id)
    {
        collectedItemIds.Add(id);
    }

    /// 수집 여부 확인
    public bool HasCollectedItem(string id)
    {
        return collectedItemIds.Contains(id);
    }

    /*--------------- Boss Scene ----------------*/
    // 5번째 거울조각 모았을때
    public void BossMirrorPieceTrigger()
    {
        GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>().BossStart();
    }

    // 보스방에서 제한시간 끝났을때
    public void BossTimeEndDie()
    {
        player.ChangeState(PlayerController.PlayerState.Die);
    }

    // 보스방에서 플레이어가 죽었을 떄 (Hit, 제한시간)
    public void BossPlayerDie()
    {
        FadeManager.Instance.FadeOutSceneChange("12_GameOverBoss");
    }

    // 12_GameOverBoss 씬에서 Retry시 호출해야하는 함수
    public void BossRetry()
    {
        Debug.Log("보스전 Retry");
        FadeManager.Instance.FadeOutSceneChange("09_Boss");
    }


    /*--------------- 씬 로드시 처리 ----------------*/
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 플레이 끝
        if (scene.name == "01_Start" ||
            scene.name == "10_GameClear" || scene.name == "11_GameOver")
        {
            //SoundManager2.Instance.StopBGM();
            Destroy(player.gameObject);
            Destroy(this.gameObject);
        }

        // 게임 오버 보스 컷씬에서는 Destory X (다시 돌아올 가능성 있음)
        if(scene.name == "12_GameOverBoss") player.gameObject.SetActive(false);

        // 보스씬 로드시 save data로 player init
        if (scene.name == "09_Boss")
        {
            player.gameObject.SetActive(true);
            player.InitPlayer_ToBossScene();
        }
    }
}
