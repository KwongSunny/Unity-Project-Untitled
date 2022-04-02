using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Script : MonoBehaviour
{
    public int maxHp;
    public int hp;
    public int damage;

    protected Rigidbody2D rb2d;
    protected SpriteRenderer sr;

    //ANIMATION CONTROLLER
    protected Animator animator;
    protected string currentState;
    protected float timeOfLastStateChange;
    public bool finishedAnimationState; //Does not apply to looped animations

    // Start is called before the first frame update
    protected void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        finishedAnimationState = true;
    }

    protected void ChangeAnimatorState(string state){
        if(animator){
            currentState = state;
            if(animator.GetCurrentAnimatorStateInfo(0).loop) animator.Play(state);
            else animator.Play(state, -1, 0);    
            timeOfLastStateChange = Time.time;
            
            //NEED TO FIX, REMOVE STATE SCRIPTS BECAUSE 1 SCRIPT FOR EVERY INSTANCE OF PREFAB 
        }
    }

    protected void FinishAnimationState(){
        if(animator){
            if(Time.time - timeOfLastStateChange > animator.GetCurrentAnimatorStateInfo(0).length){
                finishedAnimationState = true;
            }
        }
    }

    public virtual void RecieveDamage(int damage){
        hp -= damage;
    }

    // Update is called once per frame
    protected void Update()
    {
        FinishAnimationState();
    }
}
