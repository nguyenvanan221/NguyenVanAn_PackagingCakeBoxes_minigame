using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDataSetting", menuName = "ItemDataSetting")]
public class ItemDataSetting : ScriptableObject
{
    public List<ItemData> ItemDatas;

    public ItemData GetItemDataByType(ItemType _itemType)
    {
        ItemData _itemData = new ItemData();
        foreach (var itemData in ItemDatas)
        {
            if (itemData.ItemType == _itemType)
            {
                _itemData = (ItemData) itemData;
            }
        }
        return _itemData;
    }
}

[Serializable]
public class ItemData
{
    public ItemType ItemType;
    public Sprite itemSprite;
}


