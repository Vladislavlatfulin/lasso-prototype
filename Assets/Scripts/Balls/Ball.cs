using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private readonly Vector2Int[] _directions = { new Vector2Int(1, 0), new Vector2Int(-1, 0) };
    private Vector2 _direction;
    private Camera _camera;
    private Rigidbody2D _rb;
    private float _camWidth;
    private Transform _transform;
    private bool _isMove = true;
    private CircleCollider2D _collider;

    void Start()
    {
        _transform = GetComponent<Transform>();
        _camera = Camera.main;
        _camWidth = _camera.orthographicSize * _camera.aspect;
        _direction = _directions[Random.Range(0, _directions.Length)];
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (Mathf.Abs(_transform.position.x) >= Mathf.Abs(_camWidth) && _isMove)
        {
            var pos = transform.position;
            pos.x = _camWidth * Mathf.Sign(pos.x);
            _transform.position = pos;
            _direction.x *= -1;
        }
    }

    private void FixedUpdate()
    {
        if (_isMove)
        {
            _rb.MovePosition((Vector2)_transform.position + new Vector2(_direction.x, 0) * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        _direction.x *= -1;
    }

    public void StopMove()
    {
        _isMove = false;
    }

    public float GetLenght()
    {
        return 2 * Mathf.PI * _collider.radius;
    }
}
