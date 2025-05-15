using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HashSet<int> collectedPieces = new HashSet<int>();      // HashSet<> : ����Ʈ�� ����ѵ� �� ���� �÷���
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
}
