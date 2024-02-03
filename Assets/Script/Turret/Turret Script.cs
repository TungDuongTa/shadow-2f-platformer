using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public float range;
    public Transform target;
    bool detected = false;
    Vector2 direction;
    public GameObject alarmLight;
    public GameObject gun;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPos = target.position;
        direction = targetPos - (Vector2) transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction,range);
        if (hit) {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                if (!detected)
                {
                    detected = true;
                    alarmLight.GetComponent<SpriteRenderer>().color = Color.blue;

                }
            }
            else {
                if (detected)
                {
                    detected = false;
                    alarmLight.GetComponent<SpriteRenderer>().color = Color.black;
                    

                }
            }
        }
        if (detected) {
            gun.transform.up = direction;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
