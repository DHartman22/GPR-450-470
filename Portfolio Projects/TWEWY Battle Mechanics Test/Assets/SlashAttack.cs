using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    public float damage;
    public float timeSinceLastHit;
    public float timeBetweenHits;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && timeSinceLastHit > timeBetweenHits)
        {
            timeSinceLastHit = 0;
            Debug.Log("Damage to " + collision.gameObject.name);
            collision.gameObject.GetComponent<Enemy>().Damage(damage + (int)Random.Range(-2, 2));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastHit += Time.deltaTime;
    }
}
