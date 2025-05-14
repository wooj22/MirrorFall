using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<string> items = new List<string>(3);
    [SerializeField] private int inventorySize = 3;
    [SerializeField] private bool isFull;

    [Header ("UI")]
    [SerializeField] private Transform inventory;           // 여기에 아이템 추가
    [SerializeField] private GameObject appleItemUI_Prefab;
    [SerializeField] private GameObject brightItemUI_Prefab;
    [SerializeField] private GameObject hourglassItemUI_Prefab;

    public bool IsInventoryFull() { return isFull; }

    /// 아이템 추가
    public void AddItem(string itemName)
    {
        if (isFull) return;

        if (items.Count < inventorySize)
        {
            items.Add(itemName);

            GameObject itemUI = null;
            Debug.Log(itemName);
            switch (itemName)
            {
                case "Apple_Item":
                    itemUI = Instantiate(appleItemUI_Prefab);
                    itemUI.transform.SetParent(inventory);
                    Debug.Log("Apple_Item UI 추가");
                    break;
                case "Bright_Item":
                    itemUI = Instantiate(brightItemUI_Prefab);
                    itemUI.transform.SetParent(inventory);
                    Debug.Log("Bright_Item UI 추가");
                    break;
                case "Hourglass_Item":
                    itemUI = Instantiate(hourglassItemUI_Prefab);
                    itemUI.transform.SetParent(inventory);
                    Debug.Log("Hourglass_Item UI 추가");
                    break;
                default:
                    break;
            }

            // full cheak
            if (items.Count >= inventorySize) { isFull = true; }
        }
    }

    // 인벤토리 정렬
    public void SortingInventory()
    {
        // 현재 UI 전부 제거
        foreach (Transform child in inventory)
        {
            Destroy(child.gameObject);
        }

        // UI 다시 정렬
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
