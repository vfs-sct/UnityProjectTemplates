using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class POVCamera : MonoBehaviour
{
    private Camera _mainCamera;
    private Camera _camera;
    private bool _matching;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _mainCamera = Camera.main;
        _matching = _mainCamera != null;
        if (_matching)
        {
            UniversalAdditionalCameraData cameraData = _mainCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(_camera);
        }
    }

    private void LateUpdate()
    {
        if (_matching)
        {
            transform.position = _mainCamera.transform.position;
            transform.rotation = _mainCamera.transform.rotation;
        }
    }
}
