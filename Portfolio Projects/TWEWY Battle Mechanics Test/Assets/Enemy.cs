using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject damageNumber;
    public AudioSource hitSound;


    public void Damage(float dmgValue)
    {
        GameObject number = Instantiate(damageNumber);
        number.GetComponent<TextMesh>().text = dmgValue.ToString();
        hitSound.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
