using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    private HashSet<int> collectedMirror = new HashSet<int>();  // mirror piece collected data
    private List<string> savedInventoryItems = null;            // inventory save data
    private bool hasSavedInventory = false; // 아 이거 뭐더라 나중에 확인해봐

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

    /*--------------- Boss Scene ----------------*/
    public void BossTimeEndDie()
    {
        player.ChangeState(PlayerController.PlayerState.Die);
    }

    public void BossPlayerDie()
    {
        GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>().RetryPannelOn();
    }

    public void BossRetry()
    {
        Debug.Log("보스전 Retry");
        FadeManager.Instance.FadeOutSceneChange(SceneSwitch.Instance.GetCurrentScene());
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
            Destroy(player.gameObject);
            Destroy(this.gameObject);
        }

        // 보스씬 로드시 save data로 player init
        if (scene.name == "09_Boss") player.InitPlayer_ToBossScene();
    }
}
