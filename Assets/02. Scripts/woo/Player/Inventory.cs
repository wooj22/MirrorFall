using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<string> items = new List<string>(3);
    [SerializeField] private int inventorySize = 3;
    [SerializeField] private bool isFull;

    [Header ("UI")]
    [SerializeField] private Transform inventory;
    [SerializeField] private GameObject appleItemUI_Prefab;
    [SerializeField] private GameObject brightItemUI_Prefab;
    [SerializeField] private GameObject hourglassItemUI_Prefab;

    public bool IsInventoryFull() { return isFull; }

    /// Add Item
    public void AddItem(string itemName)
    {
        if (items.Count < inventorySize)
        {
            items.Add(itemName);

            GameObject itemUI = null;
            switch (itemName)
            {
                case "Apple_Item":
                    itemUI = Instantiate(appleItemUI_Prefab);
                    itemUI.transform.SetParent(inventory);
                    break;
                case "Bright_Item":
                    itemUI = Instantiate(brightItemUI_Prefab);
                    itemUI.transform.SetParent(inventory);
                    break;
                case "Hourglass_Item":
                    itemUI = Instantiate(hourglassItemUI_Prefab);
                    itemUI.transform.SetParent(inventory);
                    break;
                default:
                    break;
            }

            // full cheak
            if (items.Count >= inventorySize) { isFull = true; }
        }
    }

    /// Remove Item (사용한 아이템)
    public void RemoveItem(int index)
    {
        int listIndex = index - 1;
        if (listIndex < 0 || listIndex >= items.Count)
            return;

        items.RemoveAt(listIndex);
        isFull = items.Count >= inventorySize;
        SortingInventory();
    }

    /// Item name Getter
    public string GetIndexItemName(int index)
    {
        int listIndex = index - 1;
        if (listIndex >= 0 && listIndex < items.Count)
        {
            return items[listIndex];
        }
        return null;
    }

    /// Inventory Sort
    public void SortingInventory()
    {
        // 전부 제거
        foreach (Transform child in inventory)
        {
            Destroy(child.gameObject);
        }

        // 다시 정렬
        foreach (string itemName in items)
        {
            GameObject itemUI = null;

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
    }
}
