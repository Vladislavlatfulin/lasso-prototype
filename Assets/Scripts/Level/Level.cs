using System.Collections.Generic;
using DG.Tweening;
using Drawing;
using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;

namespace Level
{
    public class Level : IInitializable, ILateDisposable
    {
        [Inject] private DrawingManager _drawingManager;
        [Inject] private Pencil _pencil;
        [Inject] private AreaCollider _areaCollider;
        [Inject] private LassoMovementManager _movementManager;
        [Inject] private List<Ball> _balls;
        private List<GameObject> _ropeNodes = new List<GameObject>();
        private EdgeCollider2D _lineCollider;
        private bool _startDraw;
        private DrawingObject _currentObject;
        private bool _canDraw;
        private Rope.Rope _rope;

        public void Initialize()
        {
            _drawingManager.onDrawingCompleted += HandleDrawingCompletion;
            var pointerDown = new EventTrigger.Entry();
            var pointerUp = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerDown.callback.AddListener((f) => {StartDraw(); });
            pointerUp.callback.AddListener(f=> {FinishDraw();});
            
            _pencil.gameObject.GetComponent<EventTrigger>().triggers.Add(pointerDown);
            _pencil.gameObject.GetComponent<EventTrigger>().triggers.Add(pointerUp);
        }

        public void LateDispose()
        {
            _drawingManager.onDrawingCompleted -= HandleDrawingCompletion;
        }

        private void HandleDrawingCompletion(DrawingObject obj)
        {
            _currentObject = obj;
            var lr = obj.GetComponentInChildren<LineRenderer>();
            if (!lr)
            {
                Object.Destroy(obj.gameObject);
                _canDraw = false;
                _areaCollider.gameObject.SetActive(false);
                return;
            }
            var points = new Vector3[lr.positionCount];
            lr.GetPositions(points);

            var lineObj = obj.transform.GetChild(0);
            _rope = lineObj.gameObject.AddComponent<Rope.Rope>();
            _rope.OnMoveComplete += OnRopeMoveComplete;
            
            foreach (var nodePosition in points)
            {
                var node = new GameObject();
                SetupNodeRigidBody(ref node);
                SetupNodeSpringJoint(ref node);
                
                node.transform.SetParent(lineObj.transform);
                node.transform.position = nodePosition;
                _ropeNodes.Add(node);
            }
            
            lr.loop = true;
            _rope.SetRope(_ropeNodes, lr, _pencil);
            SetupRope(_rope);
            _areaCollider.gameObject.SetActive(false);
        }

        private void OnRopeMoveComplete()
        {
            var ballsInLasso = _rope.CheckBallsInLasso(_balls);
            foreach (var ballInLasso in ballsInLasso)
            {
                ballInLasso.transform.SetParent(_rope.transform);
                ballInLasso.StopMove();
            }

            if (ballsInLasso.Length > 0)
            {
                _rope.SetupRopeLenght(ballsInLasso[0].GetLenght() * ballsInLasso.Length);   
            }
            else
            {
                DestroyRope();
                ResetLevel();
            }
        }

        private void ResetLevel()
        {
            _canDraw = false;
        }

        private void SetupRope(Rope.Rope rope)
        {
            _movementManager.SetLasso(rope);
            rope.PinRope();
            rope.gameObject.transform.DOScale(new Vector3(0.40f, 0.40f), 3);
            rope.MoveToStartPosition();
        }

        private void SetupNodeSpringJoint(ref GameObject node)
        {
            var joint = node.AddComponent<SpringJoint2D>();
            joint.autoConfigureDistance = true;
            joint.enableCollision = true;
            joint.frequency = 0f;
            joint.dampingRatio = 1f;
        }

        private void SetupNodeRigidBody(ref GameObject node)
        {
            var rb = node.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.mass = 0.2f;
            rb.drag = 1f;
            rb.gravityScale = 0.0f;
        }
        
        private void StartDraw()
        {
            if (_canDraw)
                return;
            
            _canDraw = true;
            _drawingManager.StartDrawing();
            _areaCollider.gameObject.SetActive(true);
        }

        private void FinishDraw()
        {
            if (_canDraw)
                return;

            _drawingManager.ActivationPencil();
            DestroyRope();
        }

        private void DestroyRope()
        {
            if (_currentObject != null)
            {
                _rope.OnMoveComplete -= OnRopeMoveComplete;
                Object.Destroy(_currentObject.gameObject);
                _ropeNodes.Clear();
            }
        }
    }
}