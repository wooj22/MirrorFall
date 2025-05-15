using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HashSet<int> collectedPieces = new HashSet<int>();      // HashSet<> : ����Ʈ�� ����ѵ� �� ���� �÷���
    public static GameManager Instance;
    public bool isBossEndTime;
    private PlayerController player;

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
        collectedPieces.Add(pieceNum);
    }

    /// ���� ���� Ȯ��
    public bool HasCollected(int pieceNum)
    {
        return collectedPieces.Contains(pieceNum);
    }

    /*--------------- Boss ----------------*/
    public void BossPlayerDie()
    {
        // retry �г� on
        Debug.Log("retry �г� on");
    }

    public void BossTimeEnd() 
    { 
        isBossEndTime = true;
        player.isDie = true;

        // retry �г� on
        Debug.Log("retry �г� on");
    }

    public void BossRetry()
    {
        FadeManager.Instance.FadeOutSceneChange(SceneSwitch.Instance.GetCurrentScene());
    }
}
