using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush_Monster_Script : Unit_Script
{
    public bool isFlinching;
    public bool canAttack;

    public GameObject bulletProjectile;

    float attackCooldown = 1;
    float timeOfLastAttack;

    string BUSH_IDLE = "Bush_Monster_Idle";
    string BUSH_ATTACK = "Bush_Monster_Attack";
    string BUSH_FLINCH = "Bush_Monster_Flinch";

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        maxHp = 6;
        hp = maxHp;
        damage = 1;
    }

    void Attack(){
        if(!isFlinching && canAttack){
            timeOfLastAttack = Time.time;
            GameObject thorn = Instantiate(bulletProjectile, new Vector3(transform.position.x, transform.position.y, transform.position.z+1), Quaternion.identity);
            Projectile_Script thornScript = thorn.GetComponent<Projectile_Script>();
            thornScript.owner = gameObject;
            thornScript.damage = damage;
            thorn.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
        }
    }

    public override void RecieveDamage(int damage){
        hp -= damage;
        if(hp > 0){
            isFlinching = true;
            ChangeAnimatorState(BUSH_FLINCH);
        }
    }

    void CheckHealth(){
        if(hp <= 0){
            //play death animation
            //after death animation plays, increase transparency incrementally
            Destroy(gameObject);
        }
    }

    void CheckCanAttack(){
        canAttack = Time.time - timeOfLastAttack > attackCooldown;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        Attack();
        CheckCanAttack();
        CheckHealth();
    }
}
