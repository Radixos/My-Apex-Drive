using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCollision : MonoBehaviour
{
    public SphereCarController carController;

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void OnCollisionEnter(Collision collision)
    {
        // Only check collision if the car has activated rampage
        if(collision.gameObject.CompareTag("Player") && carController.rampage.activeSelf)
        {
            Vector3 normal = Vector3.zero;
            normal = collision.contacts[0].normal;

            if (collision.gameObject.GetComponent<AbilityCollision>().carController.shield.activeSelf)
            {
                rigidBody.AddForce(normal * 10.0f, ForceMode.Impulse);
                collision.gameObject.GetComponent<AbilityCollision>().carController.shieldEffectTimer = 0.0f;
            }
            else
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-normal * 10.0f, ForceMode.Impulse);
        }

    }
}
