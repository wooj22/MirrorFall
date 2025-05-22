using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    private HashSet<int> collectedMirror = new HashSet<int>();    // mirror piece collected data
    private HashSet<string> collectedItemIds = new HashSet<string>(); // item piece collected data
    //private List<string> savedInventoryItems = null;            // inventory save data
    //private bool hasSavedInventory = false;                     // �� �̰� ������ ���߿� Ȯ���غ�

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

        // �� �Դٰ��� �ϴ°Ŷ��� ���� BGM ������ ���⿡ �� (�������� soundmanager���� controll)
        //SoundManager2.Instance.SetBGM("BGM_InGame");
        //SoundManager2.Instance.PlayBGM();
    }

    /// �ſ����� ����
    public void CollectPiece(int pieceNum)
    {
        collectedMirror.Add(pieceNum);
    }

    /// �ſ� ���� ���� (5�� init)
    public void UnCollectPiece(int pieceNum)
    {
        collectedMirror.Remove(pieceNum);
    }

    /// ���� ���� Ȯ��
    public bool HasCollected(int pieceNum)
    {
        return collectedMirror.Contains(pieceNum);
    }

    /// ������ ���� ���
    public void RegisterCollectedItem(string id)
    {
        collectedItemIds.Add(id);
    }

    /// ���� ���� Ȯ��
    public bool HasCollectedItem(string id)
    {
        return collectedItemIds.Contains(id);
    }

    /*--------------- Boss Scene ----------------*/
    // 5��° �ſ����� �������
    public void BossMirrorPieceTrigger()
    {
        GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>().BossStart();
    }

    // �����濡�� ���ѽð� ��������
    public void BossTimeEndDie()
    {
        player.ChangeState(PlayerController.PlayerState.Die);
    }

    // �����濡�� �÷��̾ �׾��� �� (Hit, ���ѽð�)
    public void BossPlayerDie()
    {
        FadeManager.Instance.FadeOutSceneChange("12_GameOverBoss");
    }

    // 12_GameOverBoss ������ Retry�� ȣ���ؾ��ϴ� �Լ�
    public void BossRetry()
    {
        Debug.Log("������ Retry");
        FadeManager.Instance.FadeOutSceneChange("09_Boss");
    }


    /*--------------- �� �ε�� ó�� ----------------*/
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
        // �÷��� ��
        if (scene.name == "01_Start" ||
            scene.name == "10_GameClear" || scene.name == "11_GameOver")
        {
            //SoundManager2.Instance.StopBGM();
            Destroy(player.gameObject);
            Destroy(this.gameObject);
        }

        // ���� ���� ���� �ƾ������� Destory X (�ٽ� ���ƿ� ���ɼ� ����)
        if(scene.name == "12_GameOverBoss") player.gameObject.SetActive(false);

        // ������ �ε�� save data�� player init
        if (scene.name == "09_Boss")
        {
            player.gameObject.SetActive(true);
            player.InitPlayer_ToBossScene();
        }
    }
}
