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

    [Header("Aim")] 
    [SerializeField] private float _aimDistance = 10f;
    [SerializeField] private float _aimOffset = 1f;
    [SerializeField] private string _gamepadControlSchemeName = "Gamepad";
    
    [Header("HUD")] 
    [SerializeField] private RectTransform _crossHair;

    private Camera _mainCamera;
    private CharacterMovement _characterMovement;
    private Weapon _weapon;

    private Vector2 _moveInput;
    private bool _isFiring = false;
    private bool _possessed = false;
    private Vector3 _aimPoint;

    private Vector2 _mousePosition;
    private Vector3 _gamepadAimDirection;
    private bool _useMouseAim = true;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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

    public void OnControlsChanged(PlayerInput input)
    {
        _useMouseAim = !input.currentControlScheme.Equals(_gamepadControlSchemeName);
    }

    public void OnMouseAim(InputValue value)
    {
        _mousePosition = value.Get<Vector2>();
    }

    public void OnStickAim(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        if (input.magnitude < 0.05f) return;
        input = input.normalized;
        _gamepadAimDirection = new Vector3(input.x, 0f, input.y);
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
        if (_possessed)
        {
            Vector3 up = Vector3.up;
            Vector3 right = _mainCamera.transform.right;
            Vector3 forward = Vector3.Cross(right, up);
            Vector3 moveInput = forward * _moveInput.y + right * _moveInput.x;
        
            _characterMovement.SetMoveInput(moveInput);

            if (_useMouseAim)
            {
                _crossHair.anchoredPosition = _mousePosition;
                Plane footPlane = new Plane(Vector3.up, _target.transform.position);
                Ray mouseRay = _mainCamera.ScreenPointToRay(_mousePosition);
                if (footPlane.Raycast(mouseRay, out float distance))
                {
                    _aimPoint = mouseRay.GetPoint(distance) - mouseRay.direction * _aimOffset;
                    Vector3 aimDir = (_aimPoint - _target.transform.position).Flatten().normalized;
                    _characterMovement.SetLookDirection(aimDir);
                }
            }
            else
            {
                _aimPoint = _target.transform.position + _gamepadAimDirection * _aimDistance + Vector3.up * _aimOffset;
                _crossHair.anchoredPosition = _mainCamera.WorldToScreenPoint(_aimPoint);
                _characterMovement.SetLookDirection(_gamepadAimDirection);
            }

            if(_isFiring) _weapon.TryFire(_aimPoint);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_weapon == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_weapon.transform.position, _aimPoint);
    }
}
