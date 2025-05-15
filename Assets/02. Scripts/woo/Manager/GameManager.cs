using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HashSet<int> collectedPieces = new HashSet<int>();      // HashSet<> : 리스트랑 비슷한데 더 빠른 컬렉션
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

    /// 거울조각 수집
    public void CollectPiece(int pieceNum)
    {
        collectedPieces.Add(pieceNum);
    }

    /// 수집 여부 확인
    public bool HasCollected(int pieceNum)
    {
        return collectedPieces.Contains(pieceNum);
    }
}
