using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerController player;

    private HashSet<int> collectedMirror = new HashSet<int>();  // mirror piece collected data
    private List<string> savedInventoryItems = null;    // inventory save data
    private bool hasSavedInventory = false;

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

    /// 수집 여부 확인
    public bool HasCollected(int pieceNum)
    {
        return collectedMirror.Contains(pieceNum);
    }

    /*--------------- Boss Scene ----------------*/
    public void BossPlayerDie()
    {
        GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>().RetryPannelOn();
    }

    public void BossRetry()
    {
        Debug.Log("보스전 Retry");
        FadeManager.Instance.FadeOutSceneChange(SceneSwitch.Instance.GetCurrentScene());
        
        //StartCoroutine(BossRetryCo());
    }

    //IEnumerator BossRetryCo()
    //{
    //    yield return new WaitForSeconds(1.2f);        // 임시임
    //    player.InitPlayer_ToBossScene();
    //    Debug.Log("Init Timing cheak");
    //}

    // 보스씬이 리로드되었을 때 플레리어 Init
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
        if (scene.name == "09_Boss")
        {
            player.InitPlayer_ToBossScene();
        }
    }
}
