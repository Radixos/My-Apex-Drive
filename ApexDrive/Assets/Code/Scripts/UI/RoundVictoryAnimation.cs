using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundVictoryAnimation : MonoBehaviour
{
    private Transform animContent;
    private Animator bannerAnim, tukAnim;

    void Start()
    {
        animContent = this.gameObject.transform;
        bannerAnim = animContent.GetChild(0).GetComponent<Animator>();
        tukAnim = animContent.GetChild(1).GetComponent<Animator>();
    }

    void Update()
    {
        
    }
}
