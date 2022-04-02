using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animator_Script : MonoBehaviour
{
    string PLAYER_SWORD_IDLE = "Player_Sword_Idle";
    string PLAYER_RUN = "Player_Run";
    string PLAYER_SWORD_RUN = "Player_Sword_Run";
    string PLAYER_BOW_RUN = "Player_Bow_Run";
    string PLAYER_SWORD_RUN_ATTACK_1 = "Player_Sword_Run_Attack_1";
    string PLAYER_SWORD_RUN_ATTACK_2 = "Player_Sword_Run_Attack_2";
    string PLAYER_SWORD_RUN_ATTACK_3 = "Player_Sword_Run_Attack_3";
    string PLAYER_SWORD_ATTACK_1 = "Player_Sword_Attack_1";
    string PLAYER_SWORD_ATTACK_2 = "Player_Sword_Attack_2";
    string PLAYER_SWORD_ATTACK_3 = "Player_Sword_Attack_3";
    string PLAYER_PARRY = "Player_Parry";

    Player_Script playerScript = GameObject.Find("Player").GetComponent<Player_Script>();
    Animator animator;
    string currentState;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void ChangeState(string state){
        currentState = state;
        animator.Play(state);    
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
