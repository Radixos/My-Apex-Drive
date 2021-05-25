using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public int index;
    public int lockedIndex = 10;
    public bool keyDown;
    [SerializeField] int maxIndex;
    public AudioSource audioSource;

    [SerializeField] MainMenu canvas;

    public bool hasTransitionedOut = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Mouse ScrollWheel") != 0) && canvas.currentlyTransitioning == false)
        {
            if (!keyDown)
            {
                menuScrolling();
                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }
    }

    private void menuScrolling()
    {
        //FOR AUDIO FRIENDS, this is the code that changes what button is being selected on the menu

        if (Input.GetAxis("Vertical") < 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (index < maxIndex)
            {
                index++;
            }
            else
            {
                index = 0;
            }
        }
        else if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (index > 0)
            {
                index--;
            }
            else
            {
                index = maxIndex;
            }
        }
    }
}
