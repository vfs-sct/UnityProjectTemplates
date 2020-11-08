using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponAttributes _attributes;
    [SerializeField] private Transform _muzzle;

    private float _lastFireTime;
    
    public void TryFire(Vector3 aimPoint)
    {
        if (Time.timeSinceLevelLoad > _lastFireTime + _attributes.FirePeriod)
        {
            _lastFireTime = Time.timeSinceLevelLoad;
            Vector3 dir = (aimPoint - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(dir);
            Instantiate(_attributes.ProjectilePrefab, _muzzle.transform.position, rotation);
        }
    }
}
