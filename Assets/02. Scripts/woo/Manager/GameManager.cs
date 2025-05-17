using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    private HashSet<int> collectedMirror = new HashSet<int>();  // mirror piece collected data
    private List<string> savedInventoryItems = null;            // inventory save data
    private bool hasSavedInventory = false; // �� �̰� ������ ���߿� Ȯ���غ�

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
        Debug.Log("������ Retry");
        FadeManager.Instance.FadeOutSceneChange(SceneSwitch.Instance.GetCurrentScene());
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
            Destroy(player.gameObject);
            Destroy(this.gameObject);
        }

        // ������ �ε�� save data�� player init
        if (scene.name == "09_Boss") player.InitPlayer_ToBossScene();
    }
}
