using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class EnemyChasingScript : MonoBehaviour
{
    private GameObject player;
    private float speed = 7f;
    public float distance;
    private Rigidbody2D rb;
    private CircleCollider2D box;
    public float movement;
    public Vector2 direction;
    private PlayerAnimation playerAnimation;
    [SerializeField] private LayerMask Player;
    public GameObject headCheck;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<CircleCollider2D>();
        playerAnimation = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimation>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        direction = (player.transform.position - transform.position).normalized;
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        

        transform.rotation = Quaternion.Euler(0, 0, angle-90);
        rb.velocity = direction * speed;

        Vector2 headCheckSize = new Vector2(0.8f, 0.2f);
        if (Physics2D.OverlapBox(headCheck.transform.position, headCheckSize, 0, Player)) //checks if set box overlaps with ground
        {
            
        }
        if (distance > 15) {
            float targetSpeed = 100f;
            targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 0.5f);
            float accelRate = 10f;
            float speedDif = targetSpeed - rb.velocity.x;
            float movement = speedDif * accelRate;
            rb.AddForce(movement * direction, ForceMode2D.Force);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (box.isTrigger == true)
                box.isTrigger = false;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (box.isTrigger == false)
                box.isTrigger = true;
        }
    }


}
