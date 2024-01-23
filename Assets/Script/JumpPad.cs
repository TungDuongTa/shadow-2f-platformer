using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private float bounceForce = 30f;
    private Animator animator;
    private PlayerMovement player;
    private Rigidbody2D playerRb;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player.isFalling && player.isJumpCut)
            {
                playerRb.AddForce(Vector2.up * bounceForce*1.4f, ForceMode2D.Impulse);
                //playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Max(playerRb.velocity.y, bounceForce));
                animator.SetBool("Pump", true);
                player.isFalling = false;
                
            }
            else if (player.isFalling) {
                playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
                //playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Max(playerRb.velocity.y, bounceForce));
                animator.SetBool("Pump", true);
                player.isFalling = false;
                
            }

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
                animator.SetBool("Pump", false);
                
            }

        }
    }




