using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallcheck_Script : MonoBehaviour
{

    public bool onWall;
    Player_Script playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponentInParent<Player_Script>();
    }

    private void OnTriggerStay2D(Collider2D collision){
        
        if(collision.gameObject.tag == "ground"){
            onWall = true;
            playerScript.jumps = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.tag == "ground"){
            onWall = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerScript.facingRight){
            transform.localScale = new Vector3(1, 1, 1);
        }
        else transform.localScale = new Vector3(-1, 1, 1);
    }
}
