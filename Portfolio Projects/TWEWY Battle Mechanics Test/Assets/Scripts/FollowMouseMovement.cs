using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseMovement : MonoBehaviour
{
    [SerializeField]
    float yMovementScale = 1f;

    [SerializeField]
    float xMovementScale = 1f;

    [SerializeField]
    float speed = 1f;

    public float playerDragRadius;
    public float playerStopRadius = 1f;
    public Vector2 direction;
    private Rigidbody2D rgd;
    public Animator anim;
    private SpriteRenderer spriteRenderer;
    public bool moving;
    public bool dashing;
    public float dashVelocityRequirement;
    public float distanceVelocityRequirement;
    private CursorManager cursorManager;

    public float dashTime;
    public float dashMovingTime;
    public float dashTimeElapsed;
    public float dashPauseTime;
    public float dashEndLag;
    public float dashSpeed;
    
    public void Move(Vector2 target)
    {
        moving = true;
        direction = (target - (Vector2)transform.position).normalized;
        if(Vector2.Distance(transform.position, target) < playerStopRadius && !dashing)
        {
            rgd.velocity = Vector2.zero;
        }
        else if(!dashing)
        {
            rgd.velocity = direction * speed;
            rgd.velocity *= new Vector2(xMovementScale, yMovementScale);
        }
    }

    public void EndMove()
    {
        moving = false;
        dashing = false;
        rgd.velocity = Vector2.zero;
    }

    void CheckDirection(Vector2 dir)
    {
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void CheckDash()
    {
        if(cursorManager.velocity > dashVelocityRequirement && moving)
        {
            dashing = true;
            
        }
    }



    void Dash()
    {
        dashTimeElapsed += Time.deltaTime;
        if (dashTimeElapsed > dashPauseTime + dashMovingTime + dashEndLag)
        {
            dashing = false;
            dashTimeElapsed = 0;
            return;
        }
        if(dashTimeElapsed > dashPauseTime + dashMovingTime)
        {
            rgd.velocity = Vector2.zero;
            return;
        }
        if(dashTimeElapsed > dashPauseTime && dashTimeElapsed < dashTime + dashPauseTime)
        {
            if (Vector2.Distance(transform.position, cursorManager.GetLatestPosition()) < playerStopRadius)
            {
                rgd.velocity = Vector2.zero;
            }
            else
            {
                rgd.velocity = direction * (dashSpeed);
                rgd.velocity *= new Vector2(xMovementScale, yMovementScale);
            }
            return;
        }
        if(dashTimeElapsed < dashPauseTime)
        {
            rgd.velocity = Vector2.zero;
            return;
        }

    }

    void UpdateAnimBools()
    {
        anim.SetBool("Moving", moving);
        anim.SetBool("Dash", dashing);
    }

    // Start is called before the first frame update
    void Start()
    {
        rgd = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cursorManager = FindObjectOfType<CursorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDash();
        if(moving)
        {
            CheckDirection(direction);
        }
        if(dashing)
        {
            Dash();
        }
        UpdateAnimBools();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerDragRadius);
    }
}
