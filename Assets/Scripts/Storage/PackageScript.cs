using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PackageScript : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private StorageManager _storageManager;
    [SerializeField] private BuildingManager _buildingManager;
    [SerializeField] private Transform _PackageHolder;
    [SerializeField] private TextMeshPro _packageText;

    [Header("Package Info: Assign either an Item or a Building, not both.")]
    [SerializeField] private Item _item;
    [SerializeField] private string _building = "Please insert building name here";

    [Header("Package Settings, if item is building this does not matter - only set packageAmount")]
    public int packageAmount = 1;
    public int maxPackageAmount = 1;

    private bool _isBuilding;
    private bool _isHoldingPackage = false;

    private Rigidbody _rb;
    private Collider _collider;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxPackageAmount = packageAmount;

        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        if (_building != "")
        {
            _isBuilding = true;
        }
        if (_item != null)
        {
            _isBuilding = false;
        }
        if(_building != "" && _item != null)
        {
            Debug.LogWarning("PackageScript has both Building and Item assigned. Please assign only one.");
            gameObject.GetComponent<PackageScript>().enabled = false;
        }

        if (_isBuilding)
        {
            _packageText.text = _building;
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
                _isHoldingPackage = false;
                OnDrop();
            }
        }
    }


    public void OnPickup()
    {
        if (_isBuilding)
        {
            _buildingManager._currentBuilding = _building;
            _buildingManager.enabled = true;
            _storageManager.enabled = false;
        }
        else
        {
            _storageManager.activePackageScript = this;
            _storageManager.item = _item;
            _buildingManager.enabled = false;
            _storageManager.enabled = true;
        }
        _rb.isKinematic = true;
        _collider.enabled = false;

        _isHoldingPackage = true;
    }

    private void OnDrop()
    {
        _buildingManager.StopBuilding();
        _buildingManager.enabled = false;
        _storageManager.enabled = false;

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
