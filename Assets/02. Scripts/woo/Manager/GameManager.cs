using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerController player;
    private bool isBossEndTime;

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

    public void BossTimeEnd()
    { 
        isBossEndTime = true;
        player.isDie = true;
        GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>().RetryPannelOn();
    }

    public void BossRetry()
    {
        Debug.Log("보스전 Retry");
        FadeManager.Instance.FadeOutSceneChange(SceneSwitch.Instance.GetCurrentScene());
        StartCoroutine(BossRetryCo());
    }

    IEnumerator BossRetryCo()
    {
        yield return new WaitForSeconds(1f);
        player.InitPlayer_ToBossScene();
    }


    /*--------------- Delete ----------------*/
    // 씬 이동시 삭제 필요 (지금은 리로드때문에 삭제 안함 일단)
    //void OnEnable()
    //{
    //    SceneManager.sceneUnloaded += OnSceneUnloaded;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneUnloaded -= OnSceneUnloaded;
    //}

    //private void OnSceneUnloaded(Scene scene)
    //{
    //    if (scene.name == "09_Boss")
    //    {
    //        Destroy(player.gameObject);
    //        Destroy(this.gameObject);
    //        Debug.Log("09_Boss 씬 언로드, 플레이어와 게임매니저 파괴");
    //    }
    //}
}
