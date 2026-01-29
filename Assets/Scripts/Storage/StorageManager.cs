using UnityEngine;

public class StorageManager : MonoBehaviour
{
    private bool isStorageOpen = true;

    [SerializeField] private LayerMask Storagelayer;
    [SerializeField] private float repeatInterval = 0.2f;

    public Item item;

    private float _nextFireTime;

    public PackageScript activePackageScript;

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
        if (isStorageOpen == true)
        {
            if (Input.GetKey(KeyCode.Mouse1) && Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + repeatInterval;
                HandleStorageClickRight();
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
                if (activePackageScript.packageAmount == 0) return;
                activePackageScript.packageAmount--;
                activePackageScript.Updatetext();

                shelfSpawn.SpawnItem(item);
            }
        }
    }

    private void HandleStorageClickRight()
    {
        if (activePackageScript.packageAmount >= activePackageScript.maxPackageAmount) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f, Storagelayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<ShelfSpawn>(out var shelfSpawn))
            {
                shelfSpawn.RemoveItem(item, OnRemoveItem);
            }
        }
    }

    private void OnRemoveItem(bool _gotItemBack)
    {
        Debug.Log("Item removed: " + item);
        activePackageScript.packageAmount++;
        activePackageScript.Updatetext();
    }
}
