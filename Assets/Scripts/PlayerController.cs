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
                                                                         
    private CharacterMovement _characterMovement;

    private Vector2 _lookInput;
    private Vector2 _moveInput;
    private bool _possessed = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(_autoPossess && _target != null) Possess(_target);
    }

    public void Possess(GameObject target)
    {
        if (_target.TryGetComponent(out CharacterMovement characterMovement))
        {
            _characterMovement = characterMovement;
            _possessed = true;
            Debug.Log($"{gameObject.name} possessed {_target.name}", _target);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} failed to possess {_target.name}", _target);
            _characterMovement = null;
            _possessed = false;
        }
    }

    public void Depossess()
    {
        _characterMovement = null;
        _possessed = false;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        _characterMovement?.Jump();
    }

    private void Update()
    {
        if (_possessed)
        {
            Vector3 moveInput = Vector3.right * _moveInput.x;
            _characterMovement.SetMoveInput(moveInput);
        }
    }
}
