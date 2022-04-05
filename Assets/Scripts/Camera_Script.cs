using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Script : MonoBehaviour
{

    //pixels per unit: 20
    //Camera size: 4.5 - 320:180
    //Camera size: 6.75 - 480:270
    public float X_BOUND;


    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    void TrackPlayer()
    {

        float nextX = transform.position.x;
        if (X_BOUND != 0)
        {
            if (player.transform.position.x < 0 && player.transform.position.x <= X_BOUND)
                nextX = player.transform.position.x;
            if (player.transform.position.x < 0 && player.transform.position.x - 6.75 > X_BOUND)
            {
                nextX = player.transform.position.x;
                X_BOUND = 0;
            }

            if (player.transform.position.x > 0 && player.transform.position.x + 6.75 < X_BOUND)
            {
                nextX = player.transform.position.x;
                X_BOUND = 0;

            }
            // else if (position.x > 0 && player.transform.position.x < X_BOUND)
        }
        else nextX = player.transform.position.x;

        // set a min/max for x/y depending on bounds


        Vector3 desiredPosition = new Vector3(nextX, player.transform.position.y, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
        transform.position = smoothedPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        TrackPlayer();
    }
}
