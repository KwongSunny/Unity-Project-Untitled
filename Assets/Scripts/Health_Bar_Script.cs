using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Bar_Script : MonoBehaviour
{

    public GameObject healthOrb;

    public List<GameObject> healthOrbs = new List<GameObject>();

    Vector3 currentPosition;
    Vector3 positionOffset;

    Player_Script playerScript;

    int lastHp;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        Vector3 orbDimensions = healthOrb.GetComponent<Renderer>().bounds.size;
        positionOffset = new Vector2(orbDimensions.x, 0);

        playerScript = GameObject.Find("Player").GetComponent<Player_Script>();
        for(int i = 0; i < playerScript.maxHp; i++){
            AddHp();
        }
    }

    void AddHp(){

        GameObject orb = Instantiate(healthOrb, currentPosition, Quaternion.identity, gameObject.transform);
        healthOrbs.Add(orb);

        currentPosition += positionOffset;
    }

    void RemoveHp(){
        if(healthOrbs.Count > 0){
            Destroy((GameObject)healthOrbs[healthOrbs.Count-1]);
            healthOrbs.RemoveAt(healthOrbs.Count-1);
        }

    }

    void UpdateHealth(){
        if(playerScript.hp != lastHp){
            for(int i = 0; i < playerScript.maxHp; i++){
                if(i < playerScript.hp) healthOrbs[i].GetComponent<Health_Orb_Script>().full = true;
                else healthOrbs[i].GetComponent<Health_Orb_Script>().full = false;
            }

            lastHp = playerScript.hp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
    }
}
