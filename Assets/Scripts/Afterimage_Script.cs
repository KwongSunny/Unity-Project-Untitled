using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage_Script : MonoBehaviour
{

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void DecreaseAlpha(){
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.02f);
        
    }

    void DeleteSelf(){
        if(sr.color.a <= 0){
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseAlpha();
        DeleteSelf();
    }
}
