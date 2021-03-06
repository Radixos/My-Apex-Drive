using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************//
// TEMPORARY SCRIPT BUT
// HAS SOME PERMANENT CODE
//******************//

public class TestDrive : MonoBehaviour
{

    public bool isAI;

    private float AIMoveSpeed;
    private float playerMoveSpeed;

    public float lifeTime;
    private float timer;

    private Rigidbody myRigidbody;

    public GameObject shield;
    public Image powerMeter;
    private float powerAmount;

    private float shieldDelay;
    private float shieldTimer;


    // Start is called before the first frame update
    void Start()
    {
        AIMoveSpeed = 3.0f;
        playerMoveSpeed = 7.0f;

        timer = 0.0f;
        powerAmount = 1.0f;

        shieldDelay = 0.7f;
        shieldTimer = shieldDelay;

        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if(isAI)
        {
            //if(timer <= lifeTime)
            //{
            if (shieldTimer >= shieldDelay)
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, AIMoveSpeed);
            }
            else shieldTimer += Time.deltaTime;
                

                //timer += Time.deltaTime;
            //}
            
        }
        else
        {
            shield.SetActive(false);
            myRigidbody.velocity = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, playerMoveSpeed);
            }
            else if(Input.GetKey(KeyCode.S))
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, -playerMoveSpeed);
            }

            if(powerAmount > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    shield.SetActive(true);

                    powerAmount -= Time.deltaTime * 0.5f;

                    powerMeter.fillAmount = powerAmount;
                }
            }
            

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAI)
        {

            if (other.gameObject.name == "Shield")
            {
                myRigidbody.AddForce(-transform.forward * 10.0f);
                shieldTimer = 0.0f;
            }
        }
    }
}
