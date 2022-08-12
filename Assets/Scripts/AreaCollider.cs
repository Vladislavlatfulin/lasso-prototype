using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AreaCollider : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private BoxCollider _boxCollider;
    [Inject] private Pencil _pencil;
    private void Start()
    {
        var worldSize = _pencil.GetWorldPencilRect();
        _boxCollider.size = new Vector3(worldSize.width * 2, worldSize.height * 2, 0);
        var center = _boxCollider.center;
        var boxSize = _boxCollider.size;
        
        var topLeftCorner = new Vector2(center.x - boxSize.x / 2, center.y + boxSize.y / 2);
        var topRightCorner = new Vector2(center.x + boxSize.x / 2, center.y + boxSize.y / 2);
        var lowerLeftCorner = new Vector2(center.x - boxSize.x / 2, center.y - boxSize.y / 2);
        var lowerRightCorner = new Vector2(center.x + boxSize.x/ 2, center.y - boxSize.y / 2);
        
        _lineRenderer.positionCount = 4;
        _lineRenderer.SetPosition(0, topLeftCorner);
        _lineRenderer.SetPosition(1, topRightCorner);
        _lineRenderer.SetPosition(2, lowerRightCorner);
        _lineRenderer.SetPosition(3, lowerLeftCorner);
        
        gameObject.SetActive(false);
    }
}
