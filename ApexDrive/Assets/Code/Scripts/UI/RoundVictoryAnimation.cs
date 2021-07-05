using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundVictoryAnimation : MonoBehaviour
{
    private Transform animContent;
    private Animator bannerAnim, tukAnim, starsAnim;
    private GameObject victoryTuk, victoryStars;

    private float animationTimer;

    void Start()
    {
        animContent = this.gameObject.transform;
        bannerAnim = animContent.GetChild(0).GetComponent<Animator>();
        victoryStars = animContent.GetChild(1).gameObject;
        starsAnim = victoryStars.GetComponent<Animator>();
        victoryTuk = animContent.GetChild(2).gameObject;
        tukAnim = victoryTuk.GetComponent<Animator>();
    }

    public void AnimationEvent(CoreCarModule winningPlayer)
    {
        string winningColour = "";
        switch (winningPlayer.Player.PlayerID)
        {
            case 0:
                winningColour = "Blue";
                break;

            case 1:
                winningColour = "Red";
                break;

            case 2:
                winningColour = "Green";
                break;

            case 3:
                winningColour = "Yellow";
                break;
        }
        victoryTuk.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TukSprites/Tuk" + winningColour);

        int starNumber = 1;
        switch (winningPlayer.Player.RoundWins)
        {
            case 1:
                starNumber = 1;
                break;

            case 2:
                starNumber = 2;
                break;

            case 3:
                starNumber = 3;
                break;
        }
        victoryStars.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/StarSprites/Stars" + (starNumber));
        StartCoroutine("Toggles");
    }
    private IEnumerator Toggles()
    {
        bannerAnim.SetBool("WinAnimation", true);
        tukAnim.SetBool("WinAnimation", true);
        starsAnim.SetBool("WinAnimation", true);
        yield return new WaitForSeconds(3.5f);
        bannerAnim.SetBool("WinAnimation", false);
        tukAnim.SetBool("WinAnimation", false);
        starsAnim.SetBool("WinAnimation", false);
    }
}
