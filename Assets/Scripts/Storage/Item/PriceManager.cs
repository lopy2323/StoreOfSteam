using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceManager : MonoBehaviour
{
    private float _InputItemPrice;
    private string _InputItem;

    [SerializeField] private TMP_InputField itemInputField;
    [SerializeField] private TMP_InputField priceInputField;

    public void ChangeItemPrice()
    {
        _InputItem = itemInputField.text; if (!float.TryParse(priceInputField.text, out _InputItemPrice)) { Debug.LogError("Invalid price input. Please enter a valid number."); return; }
        _InputItemPrice = Mathf.Clamp(_InputItemPrice, 0.01f, 1000f);

        if (!ItemManager.instance.itemDictionary.ContainsKey(_InputItem)) return;
        ItemManager.instance.itemDictionary[_InputItem].currentPrice = _InputItemPrice;
        List<Shelf> shelfs = ItemManager.instance.GetShelfs(_InputItem);
        foreach (Shelf shelf in shelfs)
        {
            if (!shelf.storedItems.Contains(ItemManager.instance.scriptableItemDictionary[_InputItem])) return;
            foreach (ShelfSpawn shelfSpawn in shelf.StorageSpaces)
            {
                if (shelfSpawn != null && shelfSpawn.ShelfItem != null && shelfSpawn.ShelfItem.itemName == _InputItem)
                {
                    shelfSpawn.GetComponentInChildren<TextMeshPro>().text = "$" + _InputItemPrice.ToString("F2");
                }
            }
        }
    }
}
