using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private bool isFull;

    [Header ("UI")]
    [SerializeField] private Transform inventory;           // ���⿡ ������ �߰�
    [SerializeField] private GameObject appleItemUI_Prefab;
    [SerializeField] private GameObject brightItemUI_Prefab;
    [SerializeField] private GameObject hourglassItemUI_Prefab;

    // �κ��丮 �� �� �˻�
    public bool IsInventoryFull()
    {
        return isFull;
    }

    // ������ �߰�
    public void AddItem(string itemName)
    {
        
    }

    // �κ��丮 ����
    public void SortingInventory()
    {

    }
}
