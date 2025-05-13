using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.TestTools;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private float _followRange;
    [SerializeField] private float _moveRange;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _chaseSpeed;

    public Transform player;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    private float _distance;
    private float _startX;
    private float _startY;
    private float _pingpongStartTime;

    private bool _isChasing;

    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _startX = transform.position.x;
        _startY = transform.position.y;

        _isChasing = false;

        if (_animator == null)
        {
            Debug.LogError("[EnemyTest] Animator component not found. Please attach an Animator component.");
        }
    }

    void Update()
    {
        _distance = Vector2.Distance(transform.position, player.position);

        if (_followRange > _distance)
        {
            if (!_isChasing)
            {
                _isChasing = true;
            }

            ChasePlayer();
        }

        else
        {
            if (_isChasing)
            {
                _isChasing = false;

                _pingpongStartTime = Time.time;
                _startX = transform.position.x;
                _startY = transform.position.y;
            }

            MovePingPong();
        }

    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        _animator.SetFloat(Horizontal, direction.x);
        _animator.SetFloat(Vertical, direction.y);

        // 오른쪽 위
        if (direction.x > 0 && direction.y > 0)
        {
            _spriteRenderer.flipX = false;
            _animator.Play("RightTop");
        }

        // 오른쪽 아래
        else if (direction.x >= 0 && direction.y <= 0)
        {
            _spriteRenderer.flipX = false;
            _animator.Play("RightBot");
        }

        // 왼쪽 위
        else if (direction.x < 0 && direction.y > 0)
        {
            _spriteRenderer.flipX = true;
            _animator.Play("RightTop");
        }

        // 왼쪽 아래
        else if (direction.x <= 0 && direction.y <= 0)
        {
            _spriteRenderer.flipX = true;
            _animator.Play("RightBot");
        }

        transform.position = Vector2.MoveTowards(transform.position, player.position, _chaseSpeed * Time.deltaTime);

    }

    void MovePingPong()
    {
        if (_moveRange <= 0)
            return;

        float elapsed = Time.time - _pingpongStartTime;
        float xOffset = Mathf.PingPong(Time.time * _moveSpeed, _moveRange * 2) - _moveRange;
        float yOffset = Mathf.PingPong(Time.time * _moveSpeed, _moveRange * 2) - _moveRange;

        float newX = _startX + xOffset;
        float newY = _startY + yOffset;

        Vector2 newPos = new Vector2(newX, newY * 0.5f);
        Vector2 moveDir = (newPos - _rigidbody.position).normalized;

        _animator.SetFloat(Horizontal, moveDir.x);
        _animator.SetFloat(Vertical, moveDir.y);

        if (moveDir.x > 0 && moveDir.y > 0)
        {
            _spriteRenderer.flipX = false;
            _animator.Play("RightTop");
        }
        else if (moveDir.x >= 0 && moveDir.y <= 0)
        {
            _spriteRenderer.flipX = false;
            _animator.Play("RightBot");
        }
        else if (moveDir.x < 0 && moveDir.y > 0)
        {
            _spriteRenderer.flipX = true;
            _animator.Play("RightTop");
        }
        else if (moveDir.x <= 0 && moveDir.y <= 0)
        {
            _spriteRenderer.flipX = true;
            _animator.Play("RightBot");
        }

        _rigidbody.MovePosition(newPos);
    }
}
