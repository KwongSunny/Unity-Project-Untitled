using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Select_Script : MonoBehaviour
{
    public GameObject Character1;
    public GameObject Character2;
    public GameObject Character3;
    public GameObject tempCharacter;

    public float currentTime;
    float swapCooldown = 5f;
    public float timeOfLastSwap;

    // Start is called before the first frame update
    void Start()
    {
        timeOfLastSwap = -swapCooldown;
    }

    // void SwapCharacter(){
    //     //change current character
    //     if(Time.time - timeOfLastSwap > swapCooldown){
    //         if((Input.GetKey(KeyCode.Alpha1)) || (Input.GetKey(KeyCode.Alpha2)) || (Input.GetKey(KeyCode.Alpha3))){
    //             GameObject oldCharacter = currentCharacter;
    //             timeOfLastSwap = Time.time;
    //             Vector2 currentVelocity = Vector2.zero;

    //             if(currentCharacter) currentVelocity = currentCharacter.GetComponent<Rigidbody2D>().velocity;



    //             // if(Input.GetKey(KeyCode.Alpha1)){
    //             //     currentCharacter = Character1;
    //             // }
    //             // if(Input.GetKey(KeyCode.Alpha2)){
    //             //     currentCharacter = Character2;
    //             // }
    //             // if(Input.GetKey(KeyCode.Alpha3)){
    //             //     currentCharacter = Character3;
    //             // }


    //             tempCharacter = Character1;
    //             if(Input.GetKey(KeyCode.Alpha1) && Character2){
    //                 Character1 = Character2;
    //                 Character2 = tempCharacter;
    //             }
    //             if(Input.GetKey(KeyCode.Alpha1) && Character3){
    //                 Character1 = Character3;
    //                 Character3 = tempCharacter;
    //             }

    //             currentCharacter.SetActive(true);
    //             if(oldCharacter){
    //                 oldCharacter.SetActive(false);

    //                 //swap positions, inactive characters will be placed offscreen somewhere
    //                 Vector3 oldPosition = currentCharacter.transform.position;
    //                 currentCharacter.transform.position = oldCharacter.transform.position;
    //                 oldCharacter.transform.position = oldPosition;

    //                 //set facing direction
    //                 if(oldCharacter.GetComponent<Player_Script>().facingRight) 
    //                     currentCharacter.GetComponent<Player_Script>().Face("right");
    //                 else 
    //                     currentCharacter.GetComponent<Player_Script>().Face("left");

    //                 //preserve velocity
    //                     currentCharacter.GetComponent<Rigidbody2D>().velocity = currentVelocity;
    //             }
    //         }
    //     }
    // }

    void SwapCharacter2(){
        if(Time.time - timeOfLastSwap > swapCooldown){


            tempCharacter = Character1;
            if(Input.GetKeyDown(KeyCode.Alpha1) && Character2 != tempCharacter){
                Character1 = Character2;
                Character2 = tempCharacter;
            }
            if(Input.GetKeyDown(KeyCode.Alpha2) && Character3 != tempCharacter){
                Character1 = Character3;
                Character3 = tempCharacter;
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.time;
        SwapCharacter2();
    }
}
