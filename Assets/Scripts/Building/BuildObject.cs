using UnityEngine;

[CreateAssetMenu(fileName = "BuildObject", menuName = "Game/BuildObject")]
public class BuildObject : ScriptableObject
{
    public string Name;
    public GameObject FinalPrefab;
    public GameObject HoloPrefab;
    public float width;
    public float length;
    public float height;
}
