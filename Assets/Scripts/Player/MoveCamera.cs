using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    public float cameraOffsetY;
    void Update()
    {
        transform.position = _cameraTransform.position + new Vector3(0,cameraOffsetY,0);
    }
}
