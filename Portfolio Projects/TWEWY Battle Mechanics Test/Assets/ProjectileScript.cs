using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float killTime = 3.5f;
    public float activeTime = 0;
    public float speed;
    public float damage = 1f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Damage to " + collision.gameObject.name);
            collision.gameObject.GetComponent<Enemy>().Damage(damage + (int)Random.Range(-2, 2));
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        activeTime += Time.deltaTime;
        if(activeTime > killTime)
        {
            Destroy(gameObject);
        }
    }
}
