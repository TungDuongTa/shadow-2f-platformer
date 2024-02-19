using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFollow : MonoBehaviour
{
    
    private float startPos;
    public GameObject cam;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 distance = cam.transform.position - transform.position;
        if (distance.x > 100 ) {
            transform.position = new Vector3(transform.position.x+150, transform.position.y, transform.position.z);
        }else if (distance.x < -100 ) {
            transform.position = new Vector3(transform.position.x - 150, transform.position.y, transform.position.z);
        }
        
    }
}
