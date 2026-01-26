using UnityEngine;

public class StorageManager : MonoBehaviour
{
    bool isStorageOpen = false;
    [SerializeField]private LayerMask Storagelayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isStorageOpen == true)
        {
            while (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out RaycastHit hit,2,Storagelayer);
                GameObject obj = hit.collider.gameObject;
                if (obj != null)
                {
                    Collider col = obj.GetComponent<Collider>();
                }
            }
        }
    }
}
