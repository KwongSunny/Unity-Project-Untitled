using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Groundcheck_Script : MonoBehaviour
{

    public bool onGround;
    Player_Script playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponentInParent<Player_Script>();
    }

    private void OnTriggerStay2D(Collider2D collision){
        
        if(collision.gameObject.tag == "ground"){
            playerScript.jumps = playerScript.maxJumps;
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag == "ground"){
            onGround = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
