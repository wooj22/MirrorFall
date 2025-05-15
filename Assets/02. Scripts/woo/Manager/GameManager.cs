using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerController _player;
    private HashSet<int> collectedMirror = new HashSet<int>();      // HashSet<> : 리스트랑 비슷한데 더 빠른 컬렉션
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
