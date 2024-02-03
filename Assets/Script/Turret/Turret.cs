using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


public class Turret : MonoBehaviour
{
    [SerializeField]private Transform turretRotationPoint;
    [SerializeField] private float turretRotationSpeed= 200f;
    [SerializeField] private float targetingRange = 5f;
    private Transform target;
    [SerializeField] private LayerMask enemyLayer;

    public GameObject bulletPrefab;
    public Transform bulletPos;

    private float bulletTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }
        RotateTowardsTarget();
        if (!CheckTargetInRange())
        {
            target = null;
        }
        else {
            bulletTime += Time.deltaTime;
            if (bulletTime > 1) { 
                bulletTime = 0;
                Shoot();
            }
            
        }
        
    }
    void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, bulletPos.position, quaternion.identity);
        BulletScript bullet = bulletObj.GetComponent<BulletScript>();
        bullet.SetTarget(target);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
#endif
    private void FindTarget() {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyLayer);
        if (hit.Length > 0) {
            target = hit[0].transform;
        }

    }
    private void RotateTowardsTarget() { 
        float angle = Mathf.Atan2(-(target.position.y-transform.position.y), -(target.position.x - transform.position.x)) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f,0f,angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation,targetRotation, turretRotationSpeed * Time.deltaTime);
    }
    private bool CheckTargetInRange() { 
        return Vector2.Distance(target.position,transform.position) <= targetingRange;
    }
}
