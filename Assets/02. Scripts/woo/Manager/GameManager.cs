using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerController _player;
    private HashSet<int> collectedMirror = new HashSet<int>();      // HashSet<> : ����Ʈ�� ����ѵ� �� ���� �÷���
    public bool isBossEndTime;
    
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
        _player = GameObject.FindWithTag("Palyer").GetComponent<PlayerController>();
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

    /*--------------- Boss ----------------*/
    public void BossPlayerDie()
    {
        GameObject.Find("SceneManager").GetComponent<BossSceneManager>().RetryPannelOn();
    }

    public void BossTimeEnd() 
    { 
        isBossEndTime = true;
        _player.isDie = true;
        GameObject.Find("SceneManager").GetComponent<BossSceneManager>().RetryPannelOn();
    }
}
