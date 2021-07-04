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
        //called from resettrackscript/eliminationscript/racemanager (endround) to get player winner information?
        //(if player winner is true, perhaps?)

        //case switch player is (colour/plaayer number)

        switch (winningPlayer.Player.PlayerID)
        {
            case 1:
                //victoryTuk.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TukSprites/Tuk" + (InsertFunColourHere));
                break;
        }

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

        //if (InsertCorrectContextHere)
        //{
        StartCoroutine("Toggles");
        //}
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
