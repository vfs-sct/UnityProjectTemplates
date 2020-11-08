using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private float _dampTime = 0.1f;
    
    private Animator _animator;
    private CharacterMovement _characterMovement;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterMovement = GetComponentInParent<CharacterMovement>();
    }

    private void Update()
    {
        Vector3 localMoveInput = _characterMovement.LocalMoveInput;
        Vector3 relativeVelocity = _characterMovement.transform.InverseTransformDirection(_characterMovement.Velocity.Flatten());
        float forwardSign = Mathf.Sign(localMoveInput.z);
        float strafeSign = Mathf.Sign(localMoveInput.x);
        float forward = Mathf.Min(Mathf.Abs(localMoveInput.z), Mathf.Abs(relativeVelocity.z / _characterMovement.MovementAttributes.Speed));
        float strafe = Mathf.Min(Mathf.Abs(localMoveInput.x), Mathf.Abs(relativeVelocity.x / _characterMovement.MovementAttributes.Speed));
        _animator.SetFloat("Forward", forward * forwardSign, _dampTime, Time.deltaTime);
        _animator.SetFloat("Strafe", strafe * strafeSign, _dampTime, Time.deltaTime);
        _animator.SetBool("IsGrounded", _characterMovement.IsGrounded);
    }
}