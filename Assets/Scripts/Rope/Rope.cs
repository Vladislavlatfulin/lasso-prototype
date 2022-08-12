using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Rope
{
    public class Rope : MonoBehaviour
    {
        public event Action OnMoveComplete;  

        private List<GameObject> _nodePositions = new List<GameObject>();
        private List<SpringJoint2D> _nodeJoint = new List<SpringJoint2D>();
        private PolygonCollider2D _polygonCollider;
        private EdgeCollider2D _edgeCollider;
        private Pencil _pencil;
        private LineRenderer _lineRenderer;
        private Rigidbody2D _rb;
        private bool _isRopeSetted;
        private bool _startSqueezeRope;
        
        private float _requireLenght;
        private float _currentLenght;
        private float CurrentLenght
        {
            get => _currentLenght;
            set
            {
                if (value <= _requireLenght)
                {
                    _startSqueezeRope = false;
                    MoveToStartPosition();
                }

                _currentLenght = value;
            }
        }

        public void SetRope(List<GameObject> nodePositions, LineRenderer lineRenderer, Pencil pencil)
        {
            _nodePositions = nodePositions;
            _pencil = pencil;
            _lineRenderer = lineRenderer;
            _isRopeSetted = true;
            
            for (int i = 0; i < _nodePositions.Count; i++)
            {
                if (i == 0 || i == _nodePositions.Count - 1) continue;
                _nodeJoint.Add(_nodePositions[i].GetComponent<SpringJoint2D>());
            }
        }

        private void SetLineCollider()
        {
            _polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            _edgeCollider.edgeRadius = 0.05f;
            
            SetEdgePoints();
            SetPolygonPoints();
        }

        private void SetPolygonPoints()
        {
            _polygonCollider.points = _nodePositions
                .Select(node => transform.InverseTransformPoint(node.transform.position))
                .Select(dummy => (Vector2)dummy).ToArray();
        }

        private void SetEdgePoints()
        {
            _edgeCollider.points = _nodePositions
                .Select(node => transform.InverseTransformPoint(node.transform.position))
                .Select(dummy => (Vector2)dummy).ToArray();
        }

        public void PinRope()
        {
            for (int i = 0; i < _nodePositions.Count; i++)
            {
                var nodeJoint = _nodePositions[i].GetComponent<SpringJoint2D>();
                if (i > 0)
                    nodeJoint.connectedBody = _nodePositions[i - 1].GetComponent<Rigidbody2D>();
            }

            SetBodyTypeNodes();
        }

        private void SetBodyTypeNodes()
        {
            for (int i = 0; i < _nodePositions.Count; i++)
            {
                if (i == 0 || i == _nodePositions.Count - 1)
                    _nodePositions[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                else
                    _nodePositions[i].GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        public void SwipeMove(Vector2 position)
        {
            transform.DOMove(position, 2f).OnComplete(OnSwapComplete);
        }
        
        private void OnSwapComplete()
        {
            SqueezeRope();
            SetLineCollider();
            OnMoveComplete?.Invoke();
        }
        
        private void SqueezeRope()
        {
            _startSqueezeRope = true;
        }

        public void MoveToStartPosition()
        {
            var worldPosition = _pencil.GetPencilPosition();
            transform.DOMove(worldPosition, 3);
        }
        
        private void Update()
        {
            if(!_isRopeSetted) return;
            
            for (int i = 0; i < _nodePositions.Count; i++)
            {
                _lineRenderer.SetPosition(i, transform.InverseTransformPoint(_nodePositions[i].transform.position));
            }

            if (_polygonCollider && _edgeCollider) {
                SetPolygonPoints();
                SetEdgePoints();
            }

            if (_startSqueezeRope)
            {
                for (var index = 1; index < _nodeJoint.Count - 1; index++)
                {
                    var joint = _nodeJoint[index];
                    if (joint.distance <= 0.06f)
                    {
                        continue;
                    }

                    joint.autoConfigureDistance = false;
                    joint.distance -= 0.06f * Time.deltaTime;
                }
                CurrentLenght = GetRopeLenght();
            }
        }

        private float GetRopeLenght()
        {
            if(_nodeJoint.Count < 2)
                return 0f;
            
            var length = 0f;
             var start = _nodePositions[0];
            
            for(var i = 1; i < _nodePositions.Count; i++)
            {
                var end = _nodePositions[i];
                length += Vector2.Distance(start.transform.position, end.transform.position);
                start = end;
            }

            return length;
        }

        public Ball[] CheckBallsInLasso(List<Ball> balls)
        {
            return balls.Where(ball => _polygonCollider.OverlapPoint(ball.transform.position)).ToArray();
        }

        public void SetupRopeLenght(float requireLenght)
        {
            _requireLenght = (GetRopeLenght() + requireLenght) / 1.5f;
        }
    }
}