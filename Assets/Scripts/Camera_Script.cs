using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Script : MonoBehaviour
{
    //pixels per unit: 20
    //Camera size: 4.5 - 320:180
    //Camera size: 6.75 - 480:270


    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    void TrackPlayer(){
        Vector3 desiredPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 0.1f);
        transform.position = smoothedPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        TrackPlayer();
    }
}
