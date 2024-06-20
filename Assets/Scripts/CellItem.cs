using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CellItem : MonoBehaviour
{
    [SerializeField] private ItemDataSetting itemDataSetting;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public ItemType ItemType { get; set; }
    
    public int RowIndex { get; private set; }
    public int ColumnIndex { get; private set; }

    public void SetItem(ItemType itemType)
    {
        this.ItemType = itemType;
        SetView();
    }

    public void SetCoordinates(int x, int y)
    {
        ColumnIndex = x;
        RowIndex = y;
    }

    public void SetView()
    {
        var itemData = itemDataSetting.GetItemDataByType(ItemType);
        spriteRenderer.sprite = itemData.itemSprite;
    }

    public void Move(CellItem finalItem)
    {
        transform.position = Vector3.MoveTowards(transform.position, finalItem.transform.position, 2f);
    }
}
