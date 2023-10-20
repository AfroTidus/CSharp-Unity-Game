using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector2 boxExtents;
    Rigidbody2D rigidBody;
    Animator animator;
    public float speed = 3.0f;
    bool goingleft = true;
    int maxhits = 1;
    int hits;
    // Start is called before the first frame update
    void Start()
    {
        hits = maxhits;
        rigidBody = GetComponent<Rigidbody2D>();
        boxExtents = GetComponent<BoxCollider2D>().bounds.extents;
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (rigidBody.velocity.x * transform.localScale.x < 0.0f)
            transform.localScale = new Vector3(-transform.localScale.x,
           transform.localScale.y, transform.localScale.z);

        float Xspeed = Mathf.Abs(rigidBody.velocity.x);
        //animator.SetFloat("Xspeed", Xspeed);

        float YSpeed = rigidBody.velocity.y;
        //animator.SetFloat("Yspeed", YSpeed);
    }

    public void Damage(int damage)
    {
        hits -= damage;

        if(hits <= 0)
        {
            Die();
        }
    }

    void Die() 
    {
        animator.SetTrigger("Dead");

        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    void FixedUpdate()
    {
        //rigidBody.velocity = new Vector2(-1 * speed, rigidBody.velocity.y);
        Vector2 bottom = new Vector2(transform.position.x, transform.position.y - boxExtents.y);
        Vector2 hitBoxSize = new Vector2(boxExtents.x * 2.0f, 0.05f);

        if (goingleft == true)
        {
            rigidBody.velocity = new Vector2(-1 * speed, rigidBody.velocity.y);
        }

        if (goingleft == false)
        {
            rigidBody.velocity = new Vector2(1 * speed, rigidBody.velocity.y); ;
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Barrier")
        {
            goingleft = !goingleft;
        }
    }
}
