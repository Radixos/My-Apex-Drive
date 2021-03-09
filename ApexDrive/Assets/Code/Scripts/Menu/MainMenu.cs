using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject victoryMenu;
    public GameObject defeatMenu;

    public bool inMainMenu = true;
    public bool inOptionsMenu = false;
    public bool inVictoryMenu = false;
    public bool inDefeatMenu = false;
    public bool currentlyTransitioning = false;
    public bool checkedState = false;

    [SerializeField] MainMenuCamera mainCamera;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("GameState", 0);
    }

    // Update is called once per frame
    void Update()
    {
        float GameState = PlayerPrefs.GetFloat("GameState");
        if (!checkedState)
        {
            //victory
            if (GameState == 1)
            {
                inVictoryMenu = true;
            }
            //defeat
            else if (GameState == 2)
            {
                inDefeatMenu = true;
            }
            else
            {
                inMainMenu = true;
            }
            checkedState = true;
        }

        menuTypeConfiguration();
        cameraTransitions();

    }

    private void menuTypeConfiguration()
    {
        //in hindsight, should have been a case, but would be a bit of effort to change.. spare me
        if (inMainMenu)
        {
            mainMenu.gameObject.SetActive(true);
            optionsMenu.gameObject.SetActive(false);
            victoryMenu.gameObject.SetActive(false);
            defeatMenu.gameObject.SetActive(false);
        }
        else if (inOptionsMenu)
        {
            mainMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(true);
            victoryMenu.gameObject.SetActive(false);
            defeatMenu.gameObject.SetActive(false);
        }
        else if (inVictoryMenu)
        {
            mainMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(false);
            victoryMenu.gameObject.SetActive(true);
            defeatMenu.gameObject.SetActive(false);
        }
        else if (inDefeatMenu)
        {
            mainMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(false);
            victoryMenu.gameObject.SetActive(false);
            defeatMenu.gameObject.SetActive(true);
        }
    }

    private void cameraTransitions()
    {
        //camera transitions
        if (inOptionsMenu == true && currentlyTransitioning == true)
        {
            animator.SetBool("cameraMain", false);
            animator.SetBool("cameraOptions", true);
        }
        else if (inMainMenu == true && currentlyTransitioning == true)
        {
            animator.SetBool("cameraOptions", false);
            animator.SetBool("cameraMain", true);
        }
        else
        {
            animator.SetBool("cameraOptions", false);
            animator.SetBool("cameraMain", false);
        }
    }
}
