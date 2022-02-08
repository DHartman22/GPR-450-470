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
    private List<SpriteRenderer> spriteRenderers;
    public bool moving;
    public bool dashing;
    public bool immobile;

    public bool slashing;
    public Vector2 slashStartPoint;
    public Vector2 slashTarget;
    public float slashTime;
    public float slashTimeElapsed;
    public float slashBoostSpeed;
    public float slashBoostStopRadius;
    public float slashBoostMaxDistance;

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
        for(int i = 0; i < spriteRenderers.Count; i++)
        {
            if (direction.x > 0)
            {
                spriteRenderers[i].flipX = true;
            }
            else
            {
                spriteRenderers[i].flipX = false;
            }
        }
    }

    void CheckDash()
    {
        if(cursorManager.velocity > dashVelocityRequirement && moving)
        {
            dashing = true;
            
        }
    }

    public void StartSlash(Vector2 target)
    {
        if (dashing || slashing || immobile)
            return;

        slashTarget = target;
        immobile = true;
        slashing = true;

        rgd.velocity = Vector2.zero;

        rgd.velocity = (Vector2)(target - (Vector2)transform.position).normalized * slashBoostSpeed;
        direction = rgd.velocity;
    }

    void Slash()
    {
        slashTimeElapsed += Time.deltaTime;
        if(Vector2.Distance(slashTarget, transform.position) < slashBoostStopRadius || 
            Vector2.Distance(slashStartPoint, transform.position) > slashBoostMaxDistance)
        {
            rgd.velocity = Vector2.zero;
        }
        if(slashTimeElapsed > slashTime)
        {
            slashing = false;
            immobile = false;
            slashTimeElapsed = 0;
            return;
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
        anim.SetBool("Slash", slashing);
    }

    // Start is called before the first frame update
    void Start()
    {
        rgd = GetComponent<Rigidbody2D>();

        spriteRenderers = new List<SpriteRenderer>();
        spriteRenderers.AddRange(GetComponents<SpriteRenderer>());
        spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

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
        if(slashing)
        {
            Slash();
        }
        UpdateAnimBools();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerDragRadius);
    }
}
