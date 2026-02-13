using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static BuildingManager;

public class PackageScript : MonoBehaviour
{

    [Header("References")]
    public StorageManager _storageManager;
    public BuildingManager _buildingManager;
    public Transform _PackageHolder;
    [SerializeField] private LayerMask packageLayer;
    [SerializeField] private TextMeshPro _packageText;

    [Header("Package Info: Assign either an Item or a Building, not both.")]
    public Item packageItem;
    public string packageBuilding = "";

    [Header("Package Settings, if item is building this does not matter - only set packageAmount")]
    public int packageAmount = 1;
    public int maxPackageAmount = 1;

    private bool _isBuilding;
    private bool _isHoldingPackage = false;
    public bool isInStorage = false;

    private Rigidbody _rb;
    private Collider _collider;

    private void Awake()
    {
        maxPackageAmount = packageAmount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        if (packageBuilding != "")
        {
            _isBuilding = true;
        }
        if (packageItem != null)
        {
            _isBuilding = false;
        }
        if(packageBuilding != "" && packageItem != null)
        {
            Debug.LogWarning("PackageScript has both Building and Item assigned. Please assign only one.");
            gameObject.GetComponent<PackageScript>().enabled = false;
        }

        if (_isBuilding)
        {
            _packageText.text = packageBuilding;
        }
        else
        {
            Updatetext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isHoldingPackage)
        {
            gameObject.transform.position = _PackageHolder.position;
            gameObject.transform.rotation = _PackageHolder.rotation;
            if (Input.GetKeyDown(KeyCode.F))
            {
                GivePackage();
                if (!isInStorage)
                {
                    _isHoldingPackage = false;
                    OnDrop();
                }
            }
        }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    TakePackage();
                }
        }
    }

    private void GivePackage()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit packageHit, 2f, packageLayer))
        {
            GameObject hitObject = packageHit.collider.gameObject;

            if (hitObject.TryGetComponent<StorageShelfSpawn>(out var storageShelfSpawn))
            {
                storageShelfSpawn.InputStorageBox(this);
            }
        }
    }

    private void TakePackage()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f, packageLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.TryGetComponent<StorageShelfSpawn>(out var storageShelfSpawn))
            {
                storageShelfSpawn.GetStorageBox();
            }
        }
    }


    public void OnPickup()
    {
        if (_isBuilding)
        {
            _buildingManager._currentBuilding = packageBuilding;
            _buildingManager.currentBuildState = BuildState.Building;
            _storageManager.managerEnabled = false;
        }
        else
        {
            _storageManager.activePackageScript = this;
            _storageManager.item = packageItem;
            _buildingManager.currentBuildState = BuildState.Idle;
            _storageManager.managerEnabled = true;
        }
        _rb.isKinematic = true;
        _collider.enabled = false;

        _isHoldingPackage = true;
    }

    public void OnDrop()
    {
        _isHoldingPackage = false;

        _buildingManager.StopBuilding();
        _buildingManager.currentBuildState = BuildState.Idle;
        _storageManager.managerEnabled = false;

        _storageManager.item = null;

        _rb.isKinematic = false;
        _collider.enabled = true;
    }

    public void Updatetext()
    {
        if (!_isBuilding)
        {
            _packageText.text = packageAmount + " / " + maxPackageAmount;
        }
    }
}
