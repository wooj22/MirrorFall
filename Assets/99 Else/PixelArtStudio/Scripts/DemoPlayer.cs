using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DemoPlayer : MonoBehaviour
{
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly float LoopDuration = 2f;

    private Coroutine _loopDelayRoutine;
    private Vector2 _playerRotation = Vector2.zero;
    private Animator _animator;
    private AnimationClip[] _animationClips;
    private int _clipIndex;
    private string _enabledParam = String.Empty;
    private bool _initialized;

    private Rigidbody2D _rb;

    public Transform player;
    [SerializeField] private float _followRange = 5f;
    public float moveSpeed = 0.5f;
    private float _distance;
    public float moveRange = 3f;
    private float _startX;
    private float _startY;

    void Start()
    {
        _animator = FindObjectOfType<Animator>();

        if (_animator == null)
        {
            Debug.LogError("[DemoPlayer.cs] Can't find 'Animator' component in scene." +
                           " Add character prefab with 'Animator' component attached to it in the scene");
            return;
        }

        _rb = GetComponent<Rigidbody2D>();

        LoadAnimations();

        _initialized = true;
        _startX = transform.position.x;
        _startY = transform.position.y;
    }

    void Update()
    {
        if(!_initialized)
            return;

        if (Input.GetKey(KeyCode.W))
            _playerRotation.y = 1;
        if (Input.GetKey(KeyCode.A))
            _playerRotation.x = -1;
        if (Input.GetKey(KeyCode.S))
            _playerRotation.y = -1;
        if (Input.GetKey(KeyCode.D))
            _playerRotation.x = 1;

        _distance = Vector2.Distance(transform.position, player.position);

        if (_followRange > _distance)
        {
            ChasePlayer();
        }
        else
        {
            MovePingPong();
        }
    }

    void MovePingPong()
    {
        if (moveRange <= 0)
            return;

        moveSpeed = 0.5f;

        float xOffset = Mathf.PingPong(Time.time * moveSpeed, moveRange * 2) - moveRange;
        float yOffset = Mathf.PingPong(Time.time * moveSpeed, moveRange * 2) - moveRange;
        float newX = _startX + xOffset;
        float newY = _startY + yOffset;

        _rb.MovePosition(new Vector2(newX, newY * 0.5f));
    }

    private void LoadAnimations()
    {
        var clips = _animator.runtimeAnimatorController.animationClips;

        _animationClips = clips.GroupBy(clip => clip.name)
            .Select(item => item.First())
            .Where(clip => !clip.name.Contains("Loop") && !clip.name.Contains("End")).ToArray();
    }

    void ChasePlayer()
    {
        moveSpeed = 2f;

        Vector2 direction = (player.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }
}
