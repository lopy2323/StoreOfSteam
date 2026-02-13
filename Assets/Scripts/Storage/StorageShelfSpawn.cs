using UnityEngine;

public class StorageShelfSpawn : MonoBehaviour
{
    private PackageScript _currentPackageScript;
    public void InputStorageBox(PackageScript packageScript)
    {
        if (_currentPackageScript != null) return;
        packageScript.isInStorage = true;
        packageScript.OnDrop();
        packageScript.transform.position = transform.position;
        packageScript.transform.rotation = transform.rotation;
        packageScript.GetComponent<Rigidbody>().isKinematic = true;
        packageScript.GetComponent<Collider>().enabled = false;
        _currentPackageScript = packageScript;

    }

    public void GetStorageBox()
    {
        if (_currentPackageScript == null) return;
        _currentPackageScript.isInStorage = false;
        _currentPackageScript.OnPickup();
        _currentPackageScript = null;
    }

}
