using UnityEngine;
using UnityEngine.EventSystems;

public class Pencil : MonoBehaviour
{
    private RectTransform _rectTransform;
    private EventTrigger _eventTrigger;
    private Camera _mainCamera;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCamera = Camera.main;
    }
    
    public Rect GetWorldPencilRect() {
        Vector3[] corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }
    
    public Vector3 GetPencilPosition()
    {
        var worldPosition = GetComponent<RectTransform>().position;
        worldPosition.y -= GetWorldPencilRect().height / 3;
        worldPosition.z = -_mainCamera.transform.position.z;
        return worldPosition;
    }
}