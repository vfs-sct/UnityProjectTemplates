using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private float _dampTime = 0.1f;
    
    private Animator _animator;
    private CharacterMovement _characterMovement;
    private SpriteRenderer _renderer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterMovement = GetComponentInParent<CharacterMovement>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(_characterMovement.HasMoveInput) _renderer.flipX = _characterMovement.MoveInput.x < 0f;
        float speed = Mathf.Min(_characterMovement.MoveInput.magnitude, _characterMovement.Velocity.Flatten().magnitude / _characterMovement.MovementAttributes.Speed);
        _animator.SetFloat("Speed", speed, _dampTime, Time.deltaTime);
        _animator.SetBool("IsGrounded", _characterMovement.IsGrounded);
        _animator.SetFloat("VerticalVelocity", _characterMovement.Velocity.y);
    }
}