using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<string> items = new List<string>();
    [SerializeField] private int inventorySize = 3;
    [SerializeField] private bool isFull;

    [Header("UI")]
    [SerializeField] private Transform inventory;
    [SerializeField] private GameObject appleItemUI_Prefab;
    [SerializeField] private GameObject brightItemUI_Prefab;
    [SerializeField] private GameObject hourglassItemUI_Prefab;
    [SerializeField] private GameObject null_Prefab;  // 빈 칸용

    // 리스트대로 만들다가 아이템 쓸때 인벤토리 로직 바뀌어서 코드 정리 못함...ㅎ
    private void Awake()
    {
        for (int i = items.Count; i < inventorySize; i++)
        {
            items.Add(null);
        }
    }

    public bool IsInventoryFull() => isFull;

    /// Add Item
    public void AddItem(string itemName)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemName;
                SortingInventory();
                isFull = !items.Contains(null);
                return;
            }
        }
    }

    /// Remove Item
    public void RemoveItem(int index)
    {
        int listIndex = index - 1;
        if (listIndex >= 0 && listIndex < inventorySize)
        {
            items[listIndex] = null;
            isFull = !items.Contains(null);
            SortingInventory();
        }
    }

    /// Get Item name
    public string GetIndexItemName(int index)
    {
        int listIndex = index - 1;
        if (listIndex >= 0 && listIndex < inventorySize)
        {
            return items[listIndex];
        }
        return null;
    }

    /// Update UI
    public void SortingInventory()
    {
        // 기존 UI 전부 제거
        foreach (Transform child in inventory)
        {
            Destroy(child.gameObject);
        }

        // 다시 생성
        for (int i = 0; i < inventorySize; i++)
        {
            string itemName = items[i];
            GameObject itemUI = null;

            if (itemName == null)
            {
                itemUI = Instantiate(null_Prefab, inventory); // 빈 칸
            }
            else
            {
                switch (itemName)
                {
                    case "Apple_Item":
                        itemUI = Instantiate(appleItemUI_Prefab, inventory);
                        break;
                    case "Bright_Item":
                        itemUI = Instantiate(brightItemUI_Prefab, inventory);
                        break;
                    case "Hourglass_Item":
                        itemUI = Instantiate(hourglassItemUI_Prefab, inventory);
                        break;
                }
            }

            if (itemUI != null)
            {
                itemUI.transform.SetSiblingIndex(i);
            }
        }
    }

    /// Save inventory
    public List<string> GetInventoryData()
    {
        return new List<string>(items);
    }

    /// Load inventory
    public void SetInventoryDate(List<string> initItems)
    {
        items.Clear();
        items.AddRange(initItems);

        // 보장: 크기 맞추기
        while (items.Count < inventorySize)
            items.Add(null);

        isFull = !items.Contains(null);
        SortingInventory();
    }
}
