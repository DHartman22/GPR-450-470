using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceActor : MonoBehaviour
{
    [SerializeField] public float influenceStrength; //Can be positive for friendly influence or negative for enemy influence
    [SerializeField] public float influenceRadius;
    public float minRange;
    public float maxRange;
    public bool canShoot;
    public GameObject bullet;
    public float bulletDamage;
    public bool isPlayer;
    [SerializeField] float playerSpeed;
    public float timeSinceLastShot;
    public float timeBetweenShots;
    public float shotSpeed;
    public LayerMask agentLayer;

    public enum InfluenceType
    {
        Positive,
        Negative
    }
    public InfluenceType influenceType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }

    void PlayerControls()
    {
        if (isPlayer)
        {
            float xMove = Input.GetAxis("Horizontal");
            float yMove = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(xMove, yMove) * playerSpeed * Time.deltaTime);
        }
    }

    void EnemyShoot()
    {
        timeSinceLastShot += Time.deltaTime;
        if (canShoot && timeSinceLastShot > timeBetweenShots)
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, influenceRadius, agentLayer.value);
            float closestDistance = float.MaxValue;
            Collider2D closest = null;
            foreach (Collider2D obj in objects)
            {
                if (obj.tag != "Neutral")
                    continue;
                float distance = Vector3.Distance(obj.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = obj;
                }
            }
            if (closest == null)
                return;
            timeSinceLastShot = 0;
            GameObject tempBullet = Instantiate(bullet, transform.position, Quaternion.identity, null);
            tempBullet.GetComponent<Rigidbody2D>().velocity = (closest.transform.position - transform.position).normalized * shotSpeed;
            tempBullet.GetComponent<Bullet>().damage = bulletDamage;

            //shoot the closest neutral unit
        }
    }
    // Update is called once per frame
    void Update()
    {
        EnemyShoot();
        PlayerControls();
    }
}
