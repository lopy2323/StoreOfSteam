using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [Header("Change these values in Editor")]
    public string itemName;
    public float width;
    public float height;
    public float depth;
    public int marketPrice;

    public GameObject itemPrefab;

}
