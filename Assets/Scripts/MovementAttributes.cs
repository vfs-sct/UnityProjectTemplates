using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MovementAttributes : ScriptableObject
{
    [Header("Movement")]
    public float Speed = 5f;
    public float Acceleration = 10f;
    public float TurnSpeed = 10f;

    [Header("Airborne")] 
    public float Gravity = -20f;
    public float JumpHeight = 2f;
    public float AirControl = 0.1f;
    public bool AirTurning = false;
    
    [Header("Grounding")]
    public float GroundCheckRadius = 0.25f;
    public Vector3 GroundCheckStart = new Vector3(0f, 0.35f, 0f);
    public Vector3 GroundCheckEnd = new Vector3(0f, 0.1f, 0f);
    public float MaxSlopeAngle = 40f;
    public float GroundedFudgeTime = 0.25f;
    public LayerMask GroundMask = 1 << 0;
}
