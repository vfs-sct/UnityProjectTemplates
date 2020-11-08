﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#pragma warning disable 649

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private MovementAttributes _movementAttributes;
    [SerializeField] private Animator _art;

    public MovementAttributes MovementAttributes => _movementAttributes;
    public bool IsGrounded { get; private set; } = true;
    public bool IsFudgeGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public Vector3 MoveInput {get; private set;}
    public bool HasMoveInput { get; private set; }
    public Vector3 LookDirection {get; private set;}
    
    public bool CanMove {get; set;} = true;
    public float MoveSpeedMultiplier { get; set; } = 1f;
    public float ForcedMovement { get; set; } = 0f;
    
    private Rigidbody2D _rigidbody;
    private float _groundedFudgeTimer;

    public Vector3 Velocity
    {
        get => _rigidbody.velocity;
        set => _rigidbody.velocity = value;
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody.gravityScale = 0f;

        LookDirection = _art.transform.forward;
    }

    public void SetMoveInput(Vector3 input)
    {
        Vector3 flattened = input.Flatten();
        MoveInput = Vector3.ClampMagnitude(flattened, 1f);
        HasMoveInput = input.magnitude > 0.1f;
    }

    public void Jump()
    {
        if(CanMove && IsFudgeGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(2f * -MovementAttributes.Gravity * MovementAttributes.JumpHeight);
            Velocity = new Vector3(Velocity.x, jumpVelocity, Velocity.z);
        }
    }

    private void FixedUpdate()
    {
        float input = MoveInput.x;
        if (ForcedMovement > 0f) input = (LookDirection * ForcedMovement).x;
        float targetVelocity = (input * MovementAttributes.Speed * MoveSpeedMultiplier);
        float velocityDiff = targetVelocity - Velocity.x;
        float control = IsGrounded ? 1f : MovementAttributes.AirControl;
        Vector3 acceleration = Vector2.right * (velocityDiff * MovementAttributes.Acceleration * control);
        if (IsGrounded) acceleration += GroundNormal * MovementAttributes.Gravity;
        else acceleration += Vector3.up * MovementAttributes.Gravity;
        _rigidbody.AddForce(acceleration);
    }

    private void Update()
    {
        IsGrounded = CheckGrounded();
        if (IsGrounded) _groundedFudgeTimer = MovementAttributes.GroundedFudgeTime;
        else _groundedFudgeTimer -= Time.deltaTime;
        IsFudgeGrounded = _groundedFudgeTimer > 0f;
        
        if(!CanMove)
        {
            SetMoveInput(Vector3.zero);
        }
    }

    private bool CheckGrounded()
    {
        GroundNormal = Vector3.up;
        Vector3 start = transform.TransformPoint(MovementAttributes.GroundCheckStart);
        Vector3 end = transform.TransformPoint(MovementAttributes.GroundCheckEnd);
        Vector3 diff = end - start;
        Vector3 dir = diff.normalized;
        float distance = diff.magnitude;
        RaycastHit2D hit = Physics2D.CircleCast(start, MovementAttributes.GroundCheckRadius, dir, distance, MovementAttributes.GroundMask);
        if(hit)
        {
            GroundNormal = hit.normal;
            bool angleValid = Vector3.Angle(Vector3.up, GroundNormal) < MovementAttributes.MaxSlopeAngle;
            if (angleValid) _groundedFudgeTimer = MovementAttributes.GroundedFudgeTime;
            return angleValid;
        }

        GroundNormal = Vector3.up;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 start = transform.TransformPoint(MovementAttributes.GroundCheckStart);
        Vector3 end = transform.TransformPoint(MovementAttributes.GroundCheckEnd);
        Gizmos.DrawWireSphere(start, MovementAttributes.GroundCheckRadius);
        Gizmos.DrawWireSphere(end, MovementAttributes.GroundCheckRadius);
    }
}