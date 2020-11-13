using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#pragma warning disable 649

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private MovementAttributes _movementAttributes;

    public MovementAttributes MovementAttributes => _movementAttributes;
    public bool IsGrounded { get; private set; } = true;
    public bool IsFudgeGrounded => Time.timeSinceLevelLoad < _lastGroundedTime + MovementAttributes.GroundedFudgeTime;
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public Vector3 MoveInput {get; private set;}
    public Vector3 LocalMoveInput {get; private set;}
    public Vector3 LookDirection {get; private set;}
    
    public bool CanMove {get; set;} = true;
    public float MoveSpeedMultiplier { get; set; } = 1f;
    public float ForcedMovement { get; set; } = 0f;
    public float TurnSpeedMultiplier { get; set; } = 1f;
    
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private float _lastGroundedTime = -Mathf.Infinity;

    public Vector3 Velocity
    {
        get => _rigidbody.velocity;
        private set => _rigidbody.velocity = value;
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.useGravity = false;
        
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
        
        LookDirection = transform.forward;
    }

    public void SetMoveInput(Vector3 input)
    {
        Vector3 flattened = input.Flatten();
        MoveInput = Vector3.ClampMagnitude(flattened, 1f);
        LocalMoveInput = transform.InverseTransformDirection(MoveInput);
    }

    public void SetLookDirection(Vector3 direction)
    {
        if(direction.magnitude < 0.05f) return;
        LookDirection = direction.Flatten().normalized;
    }

    public void Jump()
    {
        if(CanMove && IsFudgeGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(2f * -MovementAttributes.Gravity * MovementAttributes.JumpHeight);
            Velocity = new Vector3(Velocity.x, jumpVelocity, Velocity.z);
        }
    }

    public void MoveTo(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }

    public void Stop()
    {
        _navMeshAgent.ResetPath();
        SetMoveInput(Vector3.zero);
    }

    private void FixedUpdate()
    {
        Vector3 input = MoveInput;
        if (ForcedMovement > 0f) input = transform.forward * ForcedMovement;
        Vector3 right = Vector3.Cross(transform.up, input);
        Vector3 forward = Vector3.Cross(right, GroundNormal);
        Vector3 targetVelocity = forward * (MovementAttributes.Speed * MoveSpeedMultiplier);
        Vector3 velocityDiff = targetVelocity.Flatten() - Velocity.Flatten();
        float control = IsGrounded ? 1f : MovementAttributes.AirControl;
        Vector3 acceleration = velocityDiff * (MovementAttributes.Acceleration * control);
        acceleration += GroundNormal * MovementAttributes.Gravity;
        _rigidbody.AddForce(acceleration);

        if (IsGrounded || MovementAttributes.AirTurning)
        {
            Quaternion targetRotation = Quaternion.LookRotation(LookDirection);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * MovementAttributes.TurnSpeed * TurnSpeedMultiplier);
            _rigidbody.MoveRotation(rotation);
        }
    }

    private void Update()
    {
        IsGrounded = CheckGrounded();
        
        if(_navMeshAgent.hasPath)
        {
            Vector3 nextPathPoint = _navMeshAgent.path.corners[1];
            Vector3 pathDir = (nextPathPoint - transform.position).normalized;
            SetMoveInput(pathDir);
            SetLookDirection(pathDir);
        }

        _navMeshAgent.nextPosition = transform.position;

        if(!CanMove)
        {
            SetMoveInput(Vector3.zero);
            SetLookDirection(transform.forward);
        }
    }

    private bool CheckGrounded()
    {
        Vector3 start = transform.TransformPoint(MovementAttributes.GroundCheckStart);
        Vector3 end = transform.TransformPoint(MovementAttributes.GroundCheckEnd);
        Vector3 diff = end - start;
        Vector3 dir = diff.normalized;
        float distance = diff.magnitude;
        if (Physics.SphereCast(start, MovementAttributes.GroundCheckRadius, dir, out RaycastHit hit, distance, MovementAttributes.GroundMask))
        {
            bool angleValid = Vector3.Angle(Vector3.up, hit.normal) < MovementAttributes.MaxSlopeAngle;
            if (angleValid)
            {
                _lastGroundedTime = Time.timeSinceLevelLoad;
                GroundNormal = hit.normal;
                return true;
            }
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