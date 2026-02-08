using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject packagePrefab;

    [SerializeField] StorageManager StorageManager;
    [SerializeField] BuildingManager BuildingManager;
    [SerializeField] Transform PackageHolder;


    public void BuyBuilding()
    {
        GameObject package = Instantiate(packagePrefab, transform.position, Quaternion.identity);
        PackageScript packageScript = package.GetComponent<PackageScript>();
        packageScript._storageManager = StorageManager;
        packageScript._buildingManager = BuildingManager;
        packageScript._PackageHolder = PackageHolder;
        packageScript.maxPackageAmount = 1;
        packageScript._building = "TestShelf2";
    }


}
