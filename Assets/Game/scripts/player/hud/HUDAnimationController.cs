using UnityEngine;
using System.Collections;

public class HUDAnimationController : MonoBehaviour {

    Animator animatorInstance;

    // Use this for initialization
    void Start()
    {
        animatorInstance = gameObject.GetComponent<Animator>();
        if (animatorInstance == null)
            Debug.LogError("A Hud Widget Animation Controller is missing an Animator!");
    }

    public void HudWidgetBoot()
    {
        //Boot hud widget anim.
    }

    public void HudWidgetShutDown()
    {
        //close hud widget anim.
    }

}
