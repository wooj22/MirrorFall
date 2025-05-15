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

    /// �ſ����� ����
    public void CollectPiece(int pieceNum)
    {
        collectedMirror.Add(pieceNum);
    }

    /// ���� ���� Ȯ��
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
        Debug.Log("������ Retry");
        FadeManager.Instance.FadeOutSceneChange(SceneSwitch.Instance.GetCurrentScene());
        StartCoroutine(BossRetryCo());
    }

    IEnumerator BossRetryCo()
    {
        yield return new WaitForSeconds(1f);
        player.InitPlayer_ToBossScene();
    }


    /*--------------- Delete ----------------*/
    // �� �̵��� ���� �ʿ� (������ ���ε嶧���� ���� ���� �ϴ�)
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
    //        Debug.Log("09_Boss �� ��ε�, �÷��̾�� ���ӸŴ��� �ı�");
    //    }
    //}
}
