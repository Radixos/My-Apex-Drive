using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************//
// TEMPORARY SCRIPT //
//******************//

public class TestDrive : MonoBehaviour
{

    public bool isAI;

    private float AIMoveSpeed;
    private float playerMoveSpeed;

    public float lifeTime;
    private float timer;

    private Rigidbody myRigidbody;


    // Start is called before the first frame update
    void Start()
    {
        AIMoveSpeed = 3.0f;
        playerMoveSpeed = 7.0f;

        timer = 0.0f;

        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        myRigidbody.velocity = Vector3.zero;

        if(isAI)
        {
            if(timer <= lifeTime)
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, AIMoveSpeed);

                timer += Time.deltaTime;
            }
            
        }
        else
        {
            if(Input.GetKey(KeyCode.W))
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, playerMoveSpeed);
            }
            else if(Input.GetKey(KeyCode.S))
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, -playerMoveSpeed);
            }
        }
    }
}
