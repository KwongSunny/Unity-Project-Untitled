using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Orb_Script : MonoBehaviour
{
    public Sprite fullOrb;
    public Sprite emptyOrb;
    public bool full = true;

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void DestroySelf(){
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(full){
            sr.sprite = fullOrb;
        }
        else sr.sprite = emptyOrb;
    }
}
