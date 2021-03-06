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
    public GameObject rampage;
    public Image powerMeter;
    private float powerAmount;

    // Small delay to prevent racer from
    // moving upon shield collision
    private float shieldCollDelay;
    private float shieldCollTimer;

    private float rampageLifetime;
    private float rampageTimer;


    // Start is called before the first frame update
    void Start()
    {
        AIMoveSpeed = 3.0f;
        playerMoveSpeed = 7.0f;

        timer = 0.0f;
        powerAmount = 1.0f;

        shieldCollDelay = 0.7f;
        shieldCollTimer = shieldCollDelay;

        rampageLifetime = 3.0f;
        rampageTimer = rampageLifetime;

        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if(isAI)
        {
            if (timer <= lifeTime)
            {
                if (shieldCollTimer >= shieldCollDelay)
                {
                    myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, AIMoveSpeed);
                }
                else shieldCollTimer += Time.deltaTime;


                timer += Time.deltaTime;
            }

        }
        else
        {
            shield.SetActive(false);
            //myRigidbody.velocity = Vector3.zero;

            if(rampage.activeSelf)
            {
                if (rampageTimer >= rampageLifetime)
                    rampage.SetActive(false);
                else
                    rampageTimer += Time.deltaTime;
                
            }

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
                else if(Input.GetKeyDown(KeyCode.E) && powerAmount >= 0.5)
                {
                    rampage.SetActive(true);
                    rampageTimer = 0.0f;
                    powerAmount -= 0.5f;
                    powerMeter.fillAmount = powerAmount;
                }
                
            }
            
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (isAI)
    //    {

    //        if (other.gameObject.name == "Shield")
    //        {
    //            myRigidbody.AddForce(-transform.forward * 10.0f, ForceMode.Impulse);
    //            shieldCollTimer = 0.0f;
    //        }
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {

        if ((collision.gameObject.CompareTag("Finish") || // Finish - temp tag for AI
            collision.gameObject.CompareTag("Player")) && rampage.activeSelf)
        {

            if(rampage.activeSelf)
            {

                if (isAI)
                    timer = lifeTime + 1.0f;

                Vector3 normal = Vector3.zero;
                normal = collision.contacts[0].normal;

                if (collision.gameObject.GetComponent<TestDrive>().shield.activeSelf)
                {
                    myRigidbody.AddForce(normal * 10.0f, ForceMode.Impulse);
                    shieldCollTimer = 0.0f;
                }
                else
                {
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(-normal * 10.0f, ForceMode.Impulse);
                }
            }            

        }
    }
}