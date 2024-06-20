using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "LevelSystem/Level")]
public class Level : ScriptableObject
{
    public int width, height;
    public List<ItemType> listItemType;
}
