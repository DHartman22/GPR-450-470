using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObject : MonoBehaviour
{
    public float killTime;
    public float activeTime;
    public bool hit;
    public float damage;
    public AudioSource fireSound;
    public bool played;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!hit && collision.gameObject.tag == "Enemy")
        {
            hit = true;
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
        activeTime += Time.deltaTime;

        if(activeTime > killTime)
        {
            Destroy(gameObject);
            return;
        }

        if(activeTime > killTime/3 && !played)
        {
            fireSound.Play();
            played = true;
        }
    }
}
