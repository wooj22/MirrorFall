using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private bool isFull;

    [Header ("UI")]
    [SerializeField] private Transform inventory;           // 여기에 아이템 추가
    [SerializeField] private GameObject appleItemUI_Prefab;
    [SerializeField] private GameObject brightItemUI_Prefab;
    [SerializeField] private GameObject hourglassItemUI_Prefab;

    // 인벤토리 꽉 참 검사
    public bool IsInventoryFull()
    {
        return isFull;
    }

    // 아이템 추가
    public void AddItem(string itemName)
    {
        
    }

    // 인벤토리 정렬
    public void SortingInventory()
    {

    }
}
