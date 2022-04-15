using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Block_Script : MonoBehaviour
{
    protected Rigidbody2D rb2d;
    public string direction = "UP";
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(0, 10f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (direction == "UP")
        {
            rb2d.velocity = new Vector2(0, -5);
        }
        else
        {
            rb2d.velocity = new Vector2(0, 5);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            if (direction == "UP")
            {
                direction = "DOWN";
            }
            else
            {
                direction = "UP";
            }

        }

    }
}
