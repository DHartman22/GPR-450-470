using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField]
    float yMovementScale = 1f;

    [SerializeField]
    float xMovementScale = 1f;

    [SerializeField]
    float speed = 1f;

    public Transform feet;

    public float playerDragRadius;
    public float playerStopRadius = 1f;
    public Vector2 direction;
    private Rigidbody2D rgd;
    public Animator anim;
    private List<SpriteRenderer> spriteRenderers;
    public bool moving;
    public bool dashing;
    public bool shooting;
    public GameObject projectile;
    public float maxTimeBetweenShots;
    public float timeSinceLastShot;
    public Transform rightShotOrigin;
    public Transform leftShotOrigin;

    public bool scratching;
    public bool dragging;
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

    public float dragFrequency;
    public GameObject fireObj;
    public float timeSinceLastFire;

    public float shakeFrequency;
    public GameObject shakeHitbox;
    public float timeSinceLastShake;
    public Transform worldToShake;
    public Vector3 originalWorldPos;
    public float yDiff;
    public bool shakeUp;
    public float timeSinceLastScreenShake;
    public float timeBetweenScreenShakes;

    public float dashTime;
    public float dashMovingTime;
    public float dashTimeElapsed;
    public float dashPauseTime;
    public float dashEndLag;
    public float dashSpeed;
    
    public void Move(Vector2 target)
    {
        if (immobile)
            return;

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
                anim.SetBool("FacingRight", true);
            }
            else
            {
                spriteRenderers[i].flipX = false;
                anim.SetBool("FacingRight", false);
            }
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

    public void StartShooting(Vector2 target)
    {
        if(!immobile)
        {
            shooting = true;
            immobile = true;
            //Shoot(target);
        }
    }

    public void Shoot(Vector2 target)
    {
        if(!shooting)
        {
            StartShooting(target);
        }
        timeSinceLastShot = 0;
        GameObject newProj;
        if(direction.x > 0)
        {
            newProj = Instantiate(projectile, rightShotOrigin.position, Quaternion.identity);
            direction = (target - (Vector2)rightShotOrigin.position).normalized;
        }
        else
        {
            newProj = Instantiate(projectile, leftShotOrigin.position, Quaternion.identity);
            direction = (target - (Vector2)leftShotOrigin.position).normalized;
        }


        newProj.GetComponent<Rigidbody2D>().velocity = (target - (Vector2)transform.position).normalized * newProj.GetComponent<ProjectileScript>().speed;
    }

    public void CheckShootingState()
    {
        if(timeSinceLastShot > maxTimeBetweenShots)
        {
            shooting = false;
            immobile = false;
        }
        else
        {
            timeSinceLastShot += Time.deltaTime;
        }
    }

    public void StartDragging()
    {
        if(!immobile)
        {
            dragging = true;
            immobile = true;
        }
    }

    public void DragFire(Vector2 target)
    {
        if(!dragging)
        {
            StartDragging();
        }
        timeSinceLastFire += Time.deltaTime;
        if(timeSinceLastFire > dragFrequency)
        {
            timeSinceLastFire = 0;
            GameObject fire;
            fire = Instantiate(fireObj, target, Quaternion.identity);
        }
    }
    public void EndDrag()
    {
        immobile = false;
        dragging = false;
    }

    public void StartScratching()
    {
        if (!immobile)
        {
            scratching = true;
            immobile = true;
        }
    }

    public void Scratch(Vector2 target)
    {
        if (!scratching)
        {
            StartScratching();
        }
        timeSinceLastShake += Time.deltaTime;
        if (timeSinceLastShake > shakeFrequency)
        {
            
            timeSinceLastShake = 0;

            GameObject shake;
            shake = Instantiate(shakeHitbox, target, Quaternion.identity);
        }
    }
    public void EndScratch()
    {
        immobile = false;
        scratching = false;
        worldToShake.position = originalWorldPos;
    }

    void UpdateAnimBools()
    {
        anim.SetBool("Moving", moving);
        anim.SetBool("Dash", dashing);
        anim.SetBool("Slash", slashing);
        anim.SetBool("Shooting", shooting);
        anim.SetBool("Scratching", scratching);
        anim.SetBool("Dragging", dragging);
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
        CheckDirection(direction);

        if (shooting)
        {
            CheckShootingState();
        }
        if(dashing)
        {
            Dash();
        }
        if(slashing)
        {
            Slash();
        }
        if(scratching) //theres a bug where scratching doesnt stop
        {
            timeSinceLastScreenShake += Time.deltaTime;
            if (shakeUp)
            {
                worldToShake.transform.position = new Vector3(worldToShake.position.x, worldToShake.position.y + yDiff, worldToShake.position.z);
                shakeUp = false;
            }
            else
            {
                worldToShake.transform.position = new Vector3(worldToShake.position.x, worldToShake.position.y - yDiff, worldToShake.position.z);
                shakeUp = true;
            }
        }
        UpdateAnimBools();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerDragRadius);
    }
}
