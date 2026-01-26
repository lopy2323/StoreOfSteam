using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public float width;
    public float height;
    public float depth;

    [SerializeField]public GameObject itemPrefab;

}
