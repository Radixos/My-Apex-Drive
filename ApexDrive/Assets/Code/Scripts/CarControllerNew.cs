using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CarControllerNew : MonoBehaviour
{
    CarInputActions controls;
    Vector2 move;

    private void Awake()
    {
        controls = new CarInputActions();
        controls.Player.PowerC.performed += context => SendMessage("power c");
        controls.Player.PowerA.performed += context => SendMessage("power a");
        controls.Player.PowerB.performed += context => SendMessage("power b");
    }
    void SendMessage(string text)
    {
        Debug.Log(text);
    }
    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
