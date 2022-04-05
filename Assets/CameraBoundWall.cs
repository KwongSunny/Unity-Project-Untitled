using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundWall : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnWillRenderObject()
    {
        GameObject.Find("Main Camera").GetComponent<Camera_Script>().X_BOUND = transform.position.x;
    }
}
