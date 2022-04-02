using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Hitbox_Script : MonoBehaviour
{
    public List<GameObject> enemyObjects = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "enemy") enemyObjects.Add(col.gameObject);
    }

    void OnTriggerExit2D(Collider2D col){
        if(enemyObjects.Find(x => x == col.gameObject)) enemyObjects.Remove(col.gameObject);
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
