using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    [SerializeField]private GameObject _mpcPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _endPoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnNpc();
        }
    }
    private void SpawnNpc()
    {
        var npc = Instantiate(_mpcPrefab, _spawnPoint.position, Quaternion.identity);
        NPC npcSript = npc.GetComponent<NPC>();
        npcSript.endpoint = _endPoint;
    }
}
