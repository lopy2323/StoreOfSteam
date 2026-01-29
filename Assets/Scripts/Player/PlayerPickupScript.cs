using UnityEngine;

public class PlayerPickupScript : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickupBox();
        }
    }

    private void PickupBox()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.TryGetComponent<PackageScript>(out var packageScript))
            {
                packageScript.OnPickup();
            }
        }
    }
}
