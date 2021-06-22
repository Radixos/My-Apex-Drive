using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    private GameObject victoryMenu;

    public bool inVictoryMenu = true;
    public bool currentlyTransitioning = false;
    public bool checkedState = false;

    //[SerializeField] MainMenuCamera mainCamera;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        victoryMenu = this.gameObject;
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
            checkedState = true;
        }
        menuTypeConfiguration();
    }

    private void menuTypeConfiguration()
    {
        if (inVictoryMenu)
        {
            victoryMenu.gameObject.SetActive(true);
        }
    }
}
