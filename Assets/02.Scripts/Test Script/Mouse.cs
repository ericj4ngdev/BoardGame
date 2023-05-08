using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 offset = Vector3.up * 2;

    private Vector3 screenPoint;
    private Vector3 offsetFromMouse;

    private void OnMouseDown()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue,layerMask))
        {
            transform.position = hit.point + offset;
        }
    }

    private void OnMouseDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            transform.position = hit.point + offset;
        }
    }

    private void OnMouseUp()
    {
        transform.position = new Vector3(-5f, 0.5f, 2.5f);  
    }
}