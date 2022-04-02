using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Script : MonoBehaviour
{
    public GameObject owner;
    public int damage;

    float timeOfInstance;
    float expirationTime = 10;

    void OnTriggerEnter2D(Collider2D col){
        if(owner && col.gameObject != owner && col.gameObject.tag != "hitbox"){
            Unit_Script unitScript = col.gameObject.GetComponent<Unit_Script>();
            if(unitScript){
                unitScript.RecieveDamage(damage);
            } 
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeOfInstance = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeOfInstance > expirationTime) Destroy(gameObject);
    }
}
