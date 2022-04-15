using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : Unit_Script
{
    //Queue handler for uninteruptable actions (DASH, LIGHT, ABILITY, PARRY)
    Queue<string> priorityQueue = new Queue<string>();

    float horizontalForce;
    bool onGround;
    bool onWall;
    public bool isRunning;
    public bool isAttacking;
    public bool isParrying;
    public bool isJumping;
    public bool isDashing;
    public bool isFlinching;
    public bool isWallSliding;
    public bool isWallJumping;
    public List<string> crushCollisions;
    public string lastWeapon = "none";

    public bool facingRight = true;
    public float maxSpeed = 8f;
    public float speedForce = 3f;
    public float jumpForce = 22f;
    float dashForce = 900f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 23.5f;
    public int maxJumps = 1;
    public int jumps = 1; //gets reset in Groundcheck_Script
    public int attackCount = 0;
    public int maxLightAttacks = 3;
    public int heavyCount = 0;
    public int maxHeavyAttacks = 1;
    float heavyCooldown = 0.5f;
    float parryCooldown = 0.75f;
    float dashCooldown = 1.2f;
    float gravity = 6;

    public int energy = 100;
    public int maxEnergy = 100;

    
    [SerializeField] float verticalDashForce;
    [SerializeField] float horizontalDashForce;

    float timeOfLastDash;
    float timeOfLastParry;
    float timeOfLastAttack;
    float timeOfLastHeavy;
    float timeOfLastWallJump;
    float timeOfLastJump;

    public GameObject afterimageObject;
    public Sprite dashSprite;
    public Sprite afterimageSprite;

    public BoxCollider2D boxCollider;
    public GameObject hitboxes;

    string PLAYER_IDLE = "Player_Idle";
    string PLAYER_SWORD_IDLE = "Player_Sword_Idle";
    string PLAYER_RUN = "Player_Run";
    string PLAYER_SWORD_RUN = "Player_Sword_Run";
    string PLAYER_BOW_RUN = "Player_Bow_Run";
    string PLAYER_SWORD_RUN_ATTACK_1 = "Player_Light_1";
    string PLAYER_SWORD_RUN_ATTACK_2 = "Player_Light_2";
    string PLAYER_SWORD_RUN_ATTACK_3 = "Player_Light_3";
    string PLAYER_SWORD_ATTACK_1 = "Player_Light_1";
    string PLAYER_SWORD_ATTACK_2 = "Player_Light_2";
    string PLAYER_SWORD_ATTACK_3 = "Player_Light_3";
    string PLAYER_PARRY = "Player_Parry";
    string PLAYER_WALL_SLIDE = "Player_Wallslide";
    string PLAYER_JUMP_VERTICAL = "Player_Jump_Vertical";
    string PLAYER_JUMP_DIAGONAL = "Player_Jump_Diagonal";
    string PLAYER_FALL = "Player_Fall";

    string LIGHT = "light";
    string HEAVY = "heavy";
    string PARRY = "parry";
    string DASH = "dash";

    string RIGHT = "right";
    string LEFT = "left";

    KeyCode LIGHT_KEY = KeyCode.X;
    KeyCode HEAVY_KEY = KeyCode.C;
    KeyCode PARRY_KEY = KeyCode.V;
    KeyCode DASH_KEY = KeyCode.Z;

    /*
    PLAYER ACTION STATES PRIORITY
        [STATE]
            [States that can interupt it]
        -------------------------------------------------------------------
        DASH    [noloop]
            -
            
        LIGHT  [noloop]
            -
            
        ABILITY [noloop]
            -
            
        PARRY   [noloop]
            -

        WALL SLIDE
            JUMP	
            
        JUMP
            LIGHT - DASH - ABILITY
            
        HEAL
            IDLE - RUN - JUMP - DASH - LIGHT - ABILITY - PARRY
            
        IDLE
            RUN - JUMP - DASH - LIGHT - ABILITY - HEAL - PARRY

        RUN
            IDLE - JUMP - DASH - LIGHT - ABILITY - HEAL - PARRY
    */

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        boxCollider = GetComponent<BoxCollider2D>();

        rb2d.gravityScale = gravity;
        ChangeAnimatorState(PLAYER_IDLE);

        maxHp = 4;
        hp = maxHp;
        damage = 1;
        crushCollisions = new List<string>(2);
    }

    public void Face(string direction)
    {
        if (direction == RIGHT)
        {
            facingRight = true;
            sr.flipX = false;
            hitboxes.transform.localScale = new Vector3(1, hitboxes.transform.localScale.y, hitboxes.transform.localScale.z);
        }
        else if(direction == LEFT)
        {
            facingRight = false;
            sr.flipX = true;
            hitboxes.transform.localScale = new Vector3(-1, hitboxes.transform.localScale.y, hitboxes.transform.localScale.z);
        }
    }

    void Move()
    {
        if (!isFlinching && !isDashing && !isParrying && !isWallJumping && !isWallJumping)
        {
            if ((Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)))
            {
                horizontalForce = 0;
            }
            else horizontalForce = Input.GetAxisRaw("Horizontal");

            //Flip character
            if(!isAttacking)
                isRunning = true;
            if (horizontalForce > 0)
            {
                Face(RIGHT);
            }
            else if (horizontalForce < 0)
            {
                Face(LEFT);
            }
            else
            {
                isRunning = false;
            }
        }
    }

    void MoveHandler()
    {
        if (isRunning && !isAttacking)
        {
            if (isWallSliding)
            {
                if (facingRight)
                {
                    if (horizontalForce > 0) horizontalForce = 0;
                }
                else
                {
                    if (horizontalForce < 0) horizontalForce = 0;
                }
            }

            //rb2d.AddForce(new Vector2(speedForce * horizontalForce, 0), ForceMode2D.Impulse);

            //the closer player is to top speed, the less force is added, replace later on
            if (System.Math.Abs(rb2d.velocity.x) < maxSpeed * 3 / 4)
            {
                rb2d.AddForce(new Vector2(speedForce * horizontalForce, 0), ForceMode2D.Impulse);
            }
            else if (System.Math.Abs(rb2d.velocity.x) < (maxSpeed * 7) / 8)
            {
                rb2d.AddForce(new Vector2(speedForce * horizontalForce / 2, 0), ForceMode2D.Impulse);
            }
            else if (System.Math.Abs(rb2d.velocity.x) < (maxSpeed * 15) / 16)
            {
                rb2d.AddForce(new Vector2(speedForce * horizontalForce / 4, 0), ForceMode2D.Impulse);
            }
            else if (System.Math.Abs(rb2d.velocity.x) < (maxSpeed * 31) / 32)
            {
                rb2d.AddForce(new Vector2(speedForce * horizontalForce / 8, 0), ForceMode2D.Impulse);
            }

        }
        if(isAttacking) isRunning = false;
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && jumps > 0 && !isWallJumping && !isDashing)
        {

            jumps--;

            if (isWallSliding && !onGround)
            {
                WallJump();
            }
            else
            {
                rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }

    //If Walljumping and and spamming Jump, it will send player straight up
    void WallJump()
    {
        if (!isWallJumping)
        {
            timeOfLastWallJump = Time.time;
            isWallJumping = true;

            if (facingRight)
            {
                rb2d.velocity = new Vector2(-wallJumpForceX, wallJumpForceY);
                Face(LEFT);
            }
            else
            {
                rb2d.velocity = new Vector2(wallJumpForceX, wallJumpForceY);
                Face(RIGHT);
            }
        }
    }

    void WallJumpHandler()
    {
        if (isWallJumping && Time.time - timeOfLastWallJump > 0.3f)
        {
            isWallJumping = false;
        }
    }

    void Parry()
    {
        if (!isParrying && onGround && Time.time - timeOfLastParry > parryCooldown)
        {
            isParrying = true;
            timeOfLastParry = Time.time;
            ChangeAnimatorState(PLAYER_PARRY);

            //Parry Enemies
        }
    }

    void ParryHandler()
    {
        if (isParrying)
        {
            if (Time.time - timeOfLastParry > animator.GetCurrentAnimatorStateInfo(0).length)
                isParrying = false;
        }
    }

    void Light()
    {
        if(onGround){
            attackCount = (attackCount % maxLightAttacks) + 1;
            timeOfLastAttack = Time.time;
            isAttacking = true;
            rb2d.velocity = Vector2.zero;
            if(facingRight) rb2d.AddForce(Vector2.right * 200);
            else rb2d.AddForce(Vector2.left * 200);
            ChangeAnimatorState($"Player_Light_{attackCount}");

            // List<GameObject> enemies = hitboxes.transform.Find($"sword hb {attackCount}").GetComponent<Attack_Hitbox_Script>().enemyObjects;
            // foreach (GameObject enemy in enemies)
            // {
            //     Unit_Script enemyScript = enemy.GetComponent<Unit_Script>();
            //     enemyScript.RecieveDamage(damage);
            // }  
        }

    }

    //BUG: Spamming will keep the player in place even if running
    void Heavy(){
        if(onGround && Time.time - timeOfLastHeavy > heavyCooldown){
            heavyCount = (heavyCount % maxHeavyAttacks) + 1;
            timeOfLastHeavy = Time.time;
            isAttacking = true;
            rb2d.velocity = Vector2.zero;
            // if(facingRight) rb2d.velocity = Vector2.right * 3;
            // else rb2d.velocity = Vector2.left * 3;
            ChangeAnimatorState($"Player_Heavy_{heavyCount}");
        }
    }

    void AttackHandler()
    {
        if (isAttacking)
        {
            if (Time.time - timeOfLastAttack > animator.GetCurrentAnimatorStateInfo(0).length && Time.time - timeOfLastHeavy > animator.GetCurrentAnimatorStateInfo(0).length)
                isAttacking = false;
        }
    }

    void Dash()
    {
        if (!isParrying && (Time.time - timeOfLastDash > dashCooldown))
        {
            isDashing = true;
            timeOfLastDash = Time.time;

            // float FORCE = 900f;
            // rb2d.velocity = Vector2.zero;
            // if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow)) rb2d.AddForce(new Vector2(-FORCE, -FORCE));
            // else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow)) rb2d.AddForce(new Vector2(FORCE, -FORCE));
            // else if (Input.GetKey(KeyCode.DownArrow)) rb2d.AddForce(new Vector2(0, -FORCE));
            // else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow)) rb2d.AddForce(new Vector2(-(FORCE * .7f), FORCE * .7f));
            // else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow)) rb2d.AddForce(new Vector2(FORCE * .7f, FORCE * .7f));
            // else if (Input.GetKey(KeyCode.UpArrow)) rb2d.AddForce(new Vector2(0, FORCE));
            // else if (facingRight) rb2d.AddForce(new Vector2(FORCE, 0));
            // else rb2d.AddForce(new Vector2(-(FORCE), 0));
            // }

            if(Input.GetKey(KeyCode.UpArrow)) verticalDashForce = dashForce;
            else if(Input.GetKey(KeyCode.DownArrow)) verticalDashForce = -dashForce;
            else verticalDashForce = 0F;

            if(Input.GetKey(KeyCode.LeftArrow)) horizontalDashForce = -dashForce;
            else if(Input.GetKey(KeyCode.RightArrow)) horizontalDashForce = dashForce;
            else horizontalDashForce = 0F;

            if(horizontalDashForce != 0 && verticalDashForce != 0){
                horizontalDashForce *= 0.75f;
                verticalDashForce *= 0.75f;
            }

            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(horizontalDashForce, verticalDashForce));

        }
    }

    void DashHandler()
    {
        if(isDashing){

            rb2d.gravityScale = 0;
            //rb2d.velocity = new Vector2(horizontalDashForce, verticalDashForce);
        }


        if (isDashing && Time.time - timeOfLastDash > 0.12f)
        {
            rb2d.gravityScale = gravity;
            // rb2d.velocity = new Vector2()
            isDashing = false;
            // animator.enabled = true;
            finishedAnimationState = true;
        }
    }

    void CreateSpriteTrail()
    {
        if (isDashing)
        {
            GameObject spriteTrail = Instantiate(afterimageObject);
            spriteTrail.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
            if (!facingRight) spriteTrail.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    void WallSlide()
    {
        if (!isWallSliding && onWall && !onGround && !isWallJumping)
        {
            isWallSliding = true;
            rb2d.drag = 9;
            //rb2d.velocity = new Vector2(0, 0);
        }
    }

    void WallSlideHandler()
    {
        if (isWallSliding)
        {
            if (!onWall || onGround)
            {
                rb2d.drag = 1;
                isWallSliding = false;
            }
        }
    }

    void EnqueuePriorityAction()
    {
        if (priorityQueue.Count < 2)
        {
            if (Input.GetKeyDown(LIGHT_KEY)) priorityQueue.Enqueue(LIGHT);
            if (Input.GetKeyDown(HEAVY_KEY)) priorityQueue.Enqueue(HEAVY);
            if (Input.GetKeyDown(PARRY_KEY)) priorityQueue.Enqueue(PARRY);
            if (Input.GetKeyDown(DASH_KEY))
                if (Time.time - timeOfLastDash > dashCooldown) priorityQueue.Enqueue(DASH);
        }
    }

    void DequeuePriorityAction()
    {
        if (priorityQueue.Count > 0 && finishedAnimationState)
        {
            finishedAnimationState = false;
            if (priorityQueue.Peek() == LIGHT)
            {
                Light();
            }
            if (priorityQueue.Peek() == HEAVY)
            {
                Heavy();
            }
            if (priorityQueue.Peek() == PARRY)
            {
                Parry();
            }
            if (priorityQueue.Peek() == DASH)
            {
                Dash();
            }
            priorityQueue.Dequeue();
        }
    }

    void CheckRunning()
    {
        if (onGround && !isFlinching && !isDashing && !isParrying && rb2d.velocity.magnitude != 0)
        {
            isRunning = true;
        }
    }

    void CheckGrounded()
    {
        onGround = gameObject.GetComponentInChildren<Groundcheck_Script>().onGround;
        isJumping = !onGround;
    }

    void CheckWall()
    {
        onWall = gameObject.GetComponentInChildren<Wallcheck_Script>().onWall;
    }

    void ChangeWeapon()
    {
        //if idle, change weapon to none
        if (Time.time - timeOfLastAttack > 12) lastWeapon = "none";
    }

    public override void RecieveDamage(int damage)
    {
        hp -= damage;
    }

    void AnimationHandler()
    {
        if (finishedAnimationState)
        {
            if (onGround)
            {
                if (horizontalForce != 0)
                {
                    if (!isWallSliding && priorityQueue.Count == 0)
                    {
                        if (lastWeapon == "none") ChangeAnimatorState(PLAYER_RUN);
                        if (lastWeapon == "sword") ChangeAnimatorState(PLAYER_SWORD_RUN);
                        if (lastWeapon == "bow") ChangeAnimatorState(PLAYER_BOW_RUN);
                    }
                }
                else
                {
                    if (!isWallSliding && priorityQueue.Count == 0)
                    {
                        if (lastWeapon == "none") ChangeAnimatorState(PLAYER_IDLE);
                        if (lastWeapon == "sword") ChangeAnimatorState(PLAYER_SWORD_IDLE);
                        if (lastWeapon == "bow") ChangeAnimatorState(PLAYER_IDLE);
                    }
                }
            }
            //jumping or falling
            else
            {
                if (isWallSliding)
                {
                    ChangeAnimatorState(PLAYER_WALL_SLIDE);
                }
                else
                {
                    if (rb2d.velocity.y > 0)
                    {
                        if (System.Math.Abs(rb2d.velocity.x) > 2) ChangeAnimatorState(PLAYER_JUMP_DIAGONAL);
                        else ChangeAnimatorState(PLAYER_JUMP_VERTICAL);
                    }
                    else ChangeAnimatorState(PLAYER_FALL);
                }

            }
        }

    }

    // Update is called once per frame
    new void Update()
    {
        if(GameObject.Find("Main Camera").GetComponentInChildren<Player_Select_Script>().Character1 == gameObject){
            base.Update();

            Move();

            CheckRunning();
            CheckGrounded();
            CheckWall();
            ChangeWeapon();
            // CreateSpriteTrail();
            DequeuePriorityAction();
            EnqueuePriorityAction();

            AttackHandler();
            AnimationHandler();
            ParryHandler();

            WallSlide();
            WallSlideHandler();

            WallJumpHandler();

            //reset attackCount after idle
            if (Time.time - timeOfLastAttack > 0.4f) attackCount = 0;
            if (Time.time - timeOfLastHeavy > 0.4f) heavyCount = 0;
        }
    }

    void FixedUpdate()
    {
        if(GameObject.Find("Main Camera").GetComponentInChildren<Player_Select_Script>().Character1 == gameObject){
            DashHandler();
            MoveHandler();
            Jump();

            if (crushCollisions.Count == 2) Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" && crushCollisions.Find(it => it == "ground") == null)
        {
            crushCollisions.Add(collision.gameObject.tag);
        }
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            crushCollisions.Remove(collision.gameObject.tag);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Crush" && crushCollisions.Find(it => it == "Crush") == null)
        {
            crushCollisions.Add(collider.gameObject.tag);
        }

    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Crush")
        {
            crushCollisions.Remove(collider.gameObject.tag);
        }
    }
}

