using UnityEngine;

public class StorageManager : MonoBehaviour
{
    private bool isStorageOpen = true;

    [SerializeField] private LayerMask Storagelayer;
    [SerializeField] private float repeatInterval = 0.2f;

    [SerializeField]public Item item;

    private float _nextFireTime;

    void Update()
    {
        if (isStorageOpen == true)
        {
            if (Input.GetKey(KeyCode.Mouse0) && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + repeatInterval;
                HandleStorageClick();
            }
        }
    }

    private void HandleStorageClick()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f, Storagelayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<ShelfSpawn>(out var shelfSpawn))
            {
                shelfSpawn.SpawnItem(item);
            }
        }
    }
}
