using UnityEngine;

public class PlayerPickupScript : MonoBehaviour
{

    private MeshRenderer _oldHighlightedPackageRenderer;
    private float timer = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickupBox();
        }

        if (timer >= 0.1f)
        {
            timer = 0f;
            HighlightBox();
        }
        timer += Time.deltaTime;

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

    private void HighlightBox()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.TryGetComponent<PackageScript>(out var packageScript))
            {
                MeshRenderer packageRenderer = hitObject.GetComponentInChildren<MeshRenderer>();
                if (packageRenderer == null) return;
                Debug.Log("Looking at package: " + packageRenderer);

                packageRenderer.enabled = true;
                if (packageRenderer != _oldHighlightedPackageRenderer && _oldHighlightedPackageRenderer != null)
                {
                    _oldHighlightedPackageRenderer.enabled = false;
                }
                _oldHighlightedPackageRenderer = packageRenderer;

            }
        }
        else
        {
            if (_oldHighlightedPackageRenderer != null)
            {
                _oldHighlightedPackageRenderer.enabled = false;
                _oldHighlightedPackageRenderer = null;
            }
        }
    }
}
