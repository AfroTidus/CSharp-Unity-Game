using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Vector2 boxExtents;
    Rigidbody2D rigidBody;
    Animator animator;
    public float speed = 5.0f;
    public float jumpForce = 8.0f;
    public float airControlForce = 10.0f;
    public float airControlMax = 1.5f;
    public Transform AttackBox;
    public float attackRange = 0.5f;
    public float attackRate = 1f;
    public float nextAttack = 0f;
    public LayerMask EnemyL;
    public AudioSource gemSound;
    public AudioSource deathSound;
    public AudioSource attackSound;
    public TextMeshProUGUI uiGemsText, uiLivesText;
    int totalGems;
    int gemsCollected;
    int totalLives;
    int currentLives;
    bool dying;
    string curLevel;
    string nextLevel;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        // get the extent of the collision box
        boxExtents = GetComponent<BoxCollider2D>().bounds.extents;
        animator = GetComponent<Animator>();
        // find out how many gems in the level
        gemsCollected = 0;
        totalGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        totalLives = 3;
        currentLives = totalLives;
        curLevel = SceneManager.GetActiveScene().name;
        if (curLevel == "Level1")
            nextLevel = "Level2";
        else if (curLevel == "Level2")
            nextLevel = "Menu";
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidBody.velocity.x * transform.localScale.x < 0.0f)
            transform.localScale = new Vector3(-transform.localScale.x,
           transform.localScale.y, transform.localScale.z);

        float Xspeed = Mathf.Abs(rigidBody.velocity.x);
        animator.SetFloat("Xspeed", Xspeed);

        float YSpeed = rigidBody.velocity.y;
        animator.SetFloat("Yspeed", YSpeed);

        string uiString = "x " + gemsCollected + "/" + totalGems;
        uiGemsText.text = uiString;

        string uiString2 = currentLives + "/" + totalLives;
        uiLivesText.text = uiString2;

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Gem")
        {
            gemSound.Play();
            Destroy(coll.gameObject);
            gemsCollected++;
        }

        if (coll.gameObject.tag == "Finish")
        {
            // hide the level end object
            coll.gameObject.SetActive(false);
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator Attack()
    {
            attackSound.Play();
            animator.SetTrigger("Attack");

        //Create hitbox when attacking from a central point
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(AttackBox.position, attackRange, EnemyL);
        //interact with all other colliders hit on the enemy layer
            foreach(Collider2D enemy in enemiesHit)
            {
            enemy.GetComponent<Enemy>().Damage(1);
            }
            yield return new WaitForSeconds(1);
    }

    // Final Death
    IEnumerator DoDeath()
    {
        // freeze the rigidbody
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        // hide the player
        // GetComponent<Renderer>().enabled = false;

        //play death animation
        deathSound.Play();
        animator.SetBool("Dead", true);

        // reload the level in 2 seconds
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(3);
    }

    // Death with lives
    IEnumerator Spawn()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        dying = true;
        deathSound.Play();
        animator.SetBool("Dead", true);
        yield return new WaitForSeconds(2);
        transform.position = new Vector3(-7.48f, -3.28f, 0f);
        animator.SetBool("Dead", false);
        dying = false;
        rigidBody.constraints = ~RigidbodyConstraints2D.FreezeAll;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator LoadNextLevel()
    {
        if (nextLevel != "Finished")
        {
            // hide the player
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(nextLevel);
        }
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "DeathZone" && dying==false)
        {
            if (currentLives == 0)
            {
                StartCoroutine(DoDeath());
            }

            else
            {
                currentLives -= 1;
                StartCoroutine(Spawn());
            }
        }
                
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        // check if we are on the ground
        Vector2 bottom =
        new Vector2(transform.position.x, transform.position.y - boxExtents.y);

        Vector2 hitBoxSize = new Vector2(boxExtents.x * 2.0f, 0.05f);

        RaycastHit2D result = Physics2D.BoxCast(bottom, hitBoxSize, 0.0f,
        new Vector3(0.0f, -1.0f), 0.0f, 1 << LayerMask.NameToLayer("Ground"));

        bool grounded = result.collider != null && result.normal.y > 0.9f;
        if (grounded)
        {
            if (Input.GetAxis("Jump") > 0.0f)
                rigidBody.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
            else
                rigidBody.velocity = new Vector2(speed * h, rigidBody.velocity.y);
        }
        else
        {
            // allow a small amount of movement in the air
            float vx = rigidBody.velocity.x;
            if (h * vx < airControlMax)
                rigidBody.AddForce(new Vector2(h * airControlForce, 0));
        }

        if(Time.time >= nextAttack)
        {
            if (Input.GetButton("Fire1") && grounded)
            {
                StartCoroutine(Attack());
                nextAttack = Time.time + 1f / attackRate;
            }
        }
    }
}
