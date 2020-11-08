using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponAttributes : ScriptableObject
{
    public GameObject ProjectilePrefab;
    public float RoundsPerMinute = 180f;
    public float FirePeriod => 60f / RoundsPerMinute;
}
