using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable 649

public class PlayerController : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _autoPossess = true;
    
    [Header("Cameras")]
    [SerializeField] private CinemachineFreeLook _cinemachineCamera;

    [Header("Aim")] 
    [SerializeField] private LayerMask _aimMask = 1 << 3;
    [SerializeField] private float _aimDistance = 200f;
    
    private Camera _mainCamera;
    private CharacterMovement _characterMovement;
    private Weapon _weapon;

    private Vector2 _lookInput;
    private Vector2 _moveInput;
    private bool _isFiring = false;
    private bool _possessed = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _mainCamera = Camera.main;
        if(_autoPossess && _target != null) Possess(_target);
    }

    public void Possess(GameObject target)
    {
        _characterMovement = _target.GetComponent<CharacterMovement>();
        _weapon = _target.GetComponentInChildren<Weapon>();
        
        if (_characterMovement != null && _weapon != null)
        {
            _possessed = true;
            Debug.Log($"{gameObject.name} possessed {_target.name}", _target);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} failed to possess {_target.name}", _target);
            Depossess();
        }
    }

    public void Depossess()
    {
        _characterMovement = null;
        _weapon = null;
        _possessed = false;
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        _characterMovement?.Jump();
    }

    public void OnFire(InputValue value)
    {
        _isFiring = value.Get<float>() > 0.5f;
    }

    private void Update()
    {
        if (_cinemachineCamera != null)
        {
            _cinemachineCamera.m_XAxis.m_InputAxisValue = _lookInput.x;
            _cinemachineCamera.m_YAxis.m_InputAxisValue = _lookInput.y;
        }

        if (_possessed)
        {
            Vector3 up = Vector3.up;
            Vector3 right = _mainCamera.transform.right;
            Vector3 forward = Vector3.Cross(right, up);
            Vector3 moveInput = forward * _moveInput.y + right * _moveInput.x;
        
            _characterMovement.SetMoveInput(moveInput);
            _characterMovement.SetLookDirection(_mainCamera.transform.forward);

            Vector3 aimPoint = _mainCamera.transform.position + _mainCamera.transform.forward * _aimDistance;
            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, _aimDistance, _aimMask))
            {
                aimPoint = hit.point;
            }
            Debug.DrawLine(_mainCamera.transform.position, aimPoint, Color.red);
            if(_isFiring) _weapon.TryFire(aimPoint);
        }
    }
}
