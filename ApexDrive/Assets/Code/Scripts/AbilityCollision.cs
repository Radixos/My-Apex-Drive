    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCollision : MonoBehaviour
{
    public Abilities carAbilities;

    public bool stunned;

    private float stunTimer;

    // Start is called before the first frame update
    void Start()
    {
        stunTimer = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        if (stunned)
        {
            if (stunTimer <= 0)
            {
                stunned = false;
                stunTimer = 0.75f;
            }
            else
                stunTimer -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        // Only check collision if the car has activated rampage
        if (collision.gameObject.CompareTag("Player") && carAbilities.rampage.activeSelf)
        {
            //Debug.Log(collision.gameObject.name);

            Vector3 normal = Vector3.zero;
            normal = collision.contacts[0].normal;

            if (collision.gameObject.GetComponent<AbilityCollision>().carAbilities.shield.activeSelf)
            {
                GetComponent<Rigidbody>().AddForce(normal * 3000, ForceMode.Impulse);
                stunned = true;
            }
            else
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-normal * 3000.0f, ForceMode.Impulse);
                collision.gameObject.GetComponent<AbilityCollision>().stunned = true;
            }

        }

    }
}
