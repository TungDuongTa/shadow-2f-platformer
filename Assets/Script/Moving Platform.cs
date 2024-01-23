using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Vector3 targetPos;
    public float speed ;
    PlayerMovement player;
    Rigidbody2D rb;
    Vector3 moveDirection;
    Rigidbody2D playerRb;
    public GameObject ways;
    public Transform[] wayPoint;
    int pointIndex;
    int pointCount;
    int direction = 1;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rb=GetComponent<Rigidbody2D>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        wayPoint = new Transform[ways.transform.childCount];
        for (int i = 0; i < ways.gameObject.transform.childCount; i++) {
            wayPoint[i]= ways.transform.GetChild(i).gameObject.transform;
        }
    }
    void Start()
    {
        pointIndex = 1;
        pointCount = wayPoint.Length;
        targetPos = wayPoint[1].transform.position;
        DirectionCalculate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, targetPos) < 0.05f) {
            NextPoint();
        }
       
        
    }
    void NextPoint() {
        transform.position = targetPos;
        if (pointIndex == pointCount - 1) {
            direction = -1;
        }
        if (pointIndex == 0) { 
            direction =1;
        }
        pointIndex += direction;
        targetPos = wayPoint[pointIndex].transform.position;
        DirectionCalculate();
    }
    private void OnDrawGizmos()
    {
        
    }
    
    private void FixedUpdate()
    {
        rb.velocity = moveDirection * speed;
    }
    private void DirectionCalculate() {
        moveDirection = (targetPos - transform.position).normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.isOnPlatForm = true;
            player.platformRb = rb;
           

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.isOnPlatForm = false;
           
        }
    }
}
