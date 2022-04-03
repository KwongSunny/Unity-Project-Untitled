using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : Unit_Script
{
    //Queue handler for uninteruptable actions (DASH, ATTACK, ABILITY, PARRY, SLIDE)
    Queue<string> priorityQueue = new Queue<string>();

    float horizontalForce;
    bool onGround;
    bool onWall;
    public bool isRunning;
    public bool isAttacking;
    public bool isParrying;
    public bool isSliding;
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
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 23.5f;
    public float slideForce = 14f;
    public int maxJumps = 1;
    public int jumps = 1; //gets reset in Groundcheck_Script
    public int attackCount = 0;
    float parryCooldown = 0.75f;
    float dashCooldown = 1.2f;
    float gravity = 6;

    float timeOfLastDash;
    float timeOfLastParry;
    float timeOfLastAttack;
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
    string PLAYER_SWORD_RUN_ATTACK_1 = "Player_Sword_Run_Attack_1";
    string PLAYER_SWORD_RUN_ATTACK_2 = "Player_Sword_Run_Attack_2";
    string PLAYER_SWORD_RUN_ATTACK_3 = "Player_Sword_Run_Attack_3";
    string PLAYER_SWORD_ATTACK_1 = "Player_Sword_Attack_1";
    string PLAYER_SWORD_ATTACK_2 = "Player_Sword_Attack_2";
    string PLAYER_SWORD_ATTACK_3 = "Player_Sword_Attack_3";
    string PLAYER_PARRY = "Player_Parry";
    string PLAYER_SLIDE = "Player_Slide";
    string PLAYER_WALL_SLIDE = "Player_Wall_Slide";
    string PLAYER_JUMP_VERTICAL = "Player_Jump_Vertical";
    string PLAYER_JUMP_DIAGONAL = "Player_Jump_Diagonal";
    string PLAYER_FALL = "Player_Fall";

    string ATTACK = "attack";
    string PARRY = "parry";
    string SLIDE = "slide";
    string DASH = "dash";

    string RIGHT = "right";
    string LEFT = "left";

    KeyCode ATTACK_KEY = KeyCode.C;
    KeyCode PARRY_KEY = KeyCode.V;
    KeyCode SLIDE_KEY = KeyCode.DownArrow;
    KeyCode DASH_KEY = KeyCode.X;

    /*
    PLAYER ACTION STATES PRIORITY
        [STATE]
            [States that can interupt it]
        -------------------------------------------------------------------
        DASH    [noloop]
            -
            
        ATTACK  [noloop]
            -
            
        ABILITY [noloop]
            -
            
        PARRY   [noloop]
            -

        SLIDE   [noloop]
            -

        WALL SLIDE
            JUMP	
            
        JUMP
            ATTACK - DASH - ABILITY
            
        HEAL
            IDLE - RUN - JUMP - DASH - ATTACK - ABILITY - PARRY
            
        IDLE
            RUN - JUMP - DASH - ATTACK - ABILITY - HEAL - PARRY

        RUN
            IDLE - JUMP - DASH - ATTACK - ABILITY - HEAL - PARRY - SLIDE
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

    void Face(string direction)
    {
        if (direction == RIGHT)
        {
            facingRight = true;
            sr.flipX = false;
            hitboxes.transform.localScale = new Vector3(1, hitboxes.transform.localScale.y, hitboxes.transform.localScale.z);
        }
        else
        {
            facingRight = false;
            sr.flipX = true;
            hitboxes.transform.localScale = new Vector3(-1, hitboxes.transform.localScale.y, hitboxes.transform.localScale.z);
        }
    }

    void Move()
    {
        if (!isFlinching && !isDashing && !isParrying && !isSliding && !isWallJumping && !isWallJumping)
        {
            if ((Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)))
            {
                horizontalForce = 0;
            }
            else horizontalForce = Input.GetAxisRaw("Horizontal");

            //Flip character
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
        if (isRunning && !isSliding)
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
    }

    void Slide()
    {
        isSliding = true;
        ChangeAnimatorState(PLAYER_SLIDE);

        rb2d.velocity = Vector2.zero;
        if (facingRight) rb2d.velocity = new Vector2(22, 0);
        else rb2d.velocity = new Vector2(-22, 0);

        //Not Ideal but, it's fine for now
        boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y / 2);
        boxCollider.offset = new Vector2(boxCollider.offset.x, (boxCollider.offset.y - (boxCollider.size.y / 2)));

        //TODO Need to prevent getting stuck in tight spaces after slide is done (persist slide)
    }

    void SlideHandler(bool force = false)
    {
        if (isSliding || force)
        {

            if (isWallSliding || Time.time - timeOfLastStateChange > animator.GetCurrentAnimatorStateInfo(0).length - 0.02f)
            {

                finishedAnimationState = true;
                boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y * 2);
                boxCollider.offset = new Vector2(boxCollider.offset.x, (boxCollider.offset.y + (boxCollider.size.y / 4)));
                isSliding = false;
            }
        }
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && jumps > 0 && !isSliding && !isWallJumping)
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
            if (Time.time - timeOfLastStateChange > animator.GetCurrentAnimatorStateInfo(0).length)
                isParrying = false;
        }
    }

    void Attack()
    {
        // lastWeapon = "sword";
        attackCount = (attackCount % 3) + 1;
        timeOfLastAttack = Time.time;

        if (isRunning) ChangeAnimatorState($"Player_Sword_Run_Attack_{attackCount}");
        else ChangeAnimatorState($"Player_Sword_Attack_{attackCount}");

        List<GameObject> enemies = hitboxes.transform.Find($"sword hb {attackCount}").GetComponent<Attack_Hitbox_Script>().enemyObjects;
        foreach (GameObject enemy in enemies)
        {
            Unit_Script enemyScript = enemy.GetComponent<Unit_Script>();
            enemyScript.RecieveDamage(damage);
        }
    }

    void Dash()
    {
        if (!isParrying && (Time.time - timeOfLastDash > dashCooldown))
        {
            isDashing = true;
            timeOfLastDash = Time.time;

            rb2d.gravityScale = 0;

            // animator.enabled = false;
            // sr.sprite = dashSprite;
            // if (isDashing)
            // {
            string direction;
            float FORCE = 900f;

            rb2d.velocity = Vector2.zero;
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow)) rb2d.AddForce(new Vector2(-FORCE, -FORCE));
            else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow)) rb2d.AddForce(new Vector2(FORCE, -FORCE));
            else if (Input.GetKey(KeyCode.DownArrow)) rb2d.AddForce(new Vector2(0, -FORCE));
            else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow)) rb2d.AddForce(new Vector2(-(FORCE * .7f), FORCE * .7f));
            else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow)) rb2d.AddForce(new Vector2(FORCE * .7f, FORCE * .7f));
            else if (Input.GetKey(KeyCode.UpArrow)) rb2d.AddForce(new Vector2(0, FORCE));
            else if (facingRight) rb2d.AddForce(new Vector2(FORCE, 0));
            else rb2d.AddForce(new Vector2(-(FORCE), 0));
            // }
        }
    }

    void DashHandler()
    {
        // if (isDashing)
        // {

        //     string direction;
        //     float FORCE = 800f;

        //     rb2d.velocity = Vector2.zero;
        //     if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow)) rb2d.AddForce(new Vector2(-FORCE, -FORCE));
        //     else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow)) rb2d.AddForce(new Vector2(FORCE, -FORCE));
        //     else if (Input.GetKey(KeyCode.DownArrow)) rb2d.AddForce(new Vector2(0, -FORCE));
        //     else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow)) rb2d.AddForce(new Vector2(-FORCE, FORCE));
        //     else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow)) rb2d.AddForce(new Vector2(FORCE, FORCE));
        //     else if (Input.GetKey(KeyCode.UpArrow)) rb2d.AddForce(new Vector2(0, FORCE));
        //     else if (facingRight) rb2d.AddForce(new Vector2(FORCE, 0));
        //     else rb2d.AddForce(new Vector2(-FORCE, 0));
        // }

        if (isDashing && Time.time - timeOfLastDash > 0.12f)
        {
            rb2d.gravityScale = gravity;
            // rb2d.velocity = Vector2.zero;
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
            if (Input.GetKeyDown(ATTACK_KEY)) priorityQueue.Enqueue(ATTACK);
            if (Input.GetKeyDown(PARRY_KEY)) priorityQueue.Enqueue(PARRY);
            if (Input.GetKeyDown(SLIDE_KEY))
            {
                if (!isSliding && onGround && isRunning) priorityQueue.Enqueue(SLIDE);
            }
            if (Input.GetKeyDown(DASH_KEY))
                if (Time.time - timeOfLastDash > dashCooldown) priorityQueue.Enqueue(DASH);
        }
    }

    void DequeuePriorityAction()
    {
        if (priorityQueue.Count > 0 && finishedAnimationState)
        {
            finishedAnimationState = false;
            if (priorityQueue.Peek() == ATTACK)
            {
                Attack();
            }
            if (priorityQueue.Peek() == PARRY)
            {
                Parry();
            }
            if (priorityQueue.Peek() == SLIDE)
            {
                Slide();
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
        if (onGround && !isFlinching && !isDashing && !isParrying && !isSliding && rb2d.velocity.magnitude != 0)
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
        base.Update();

        Move();

        CheckRunning();
        CheckGrounded();
        CheckWall();
        ChangeWeapon();
        // CreateSpriteTrail();
        DequeuePriorityAction();
        EnqueuePriorityAction();

        AnimationHandler();
        SlideHandler();
        ParryHandler();

        WallSlide();
        WallSlideHandler();

        WallJumpHandler();

        if (!isDashing && !isSliding)
            //rb2d.velocity = new Vector2(Vector2.ClampMagnitude(rb2d.velocity, maxSpeed).x, rb2d.velocity.y);

            //reset attackCount after idle
            if (Time.time - timeOfLastAttack > 0.4f) attackCount = 0;

    }

    void FixedUpdate()
    {
        DashHandler();
        MoveHandler();
        Jump();

        if (crushCollisions.Count == 2) Destroy(gameObject);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground" && crushCollisions.Find(it => it == "ground") == null)
        {
            Debug.Log("adding ground collision");
            crushCollisions.Add(collision.gameObject.tag);
        }
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            Debug.Log("rEMOVING ground collision");
            crushCollisions.Remove(collision.gameObject.tag);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.tag);
        if (collider.gameObject.tag == "Crush" && crushCollisions.Find(it => it == "Crush") == null)
        {
            Debug.Log("Adding crush collision");
            crushCollisions.Add(collider.gameObject.tag);
        }

    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Crush")
        {
            Debug.Log("rEMOVING crush collision");
            crushCollisions.Remove(collider.gameObject.tag);
        }
    }
}

