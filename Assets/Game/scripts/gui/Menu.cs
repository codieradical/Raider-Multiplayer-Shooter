using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour {

    private Animator animator;
    private CanvasGroup canvasGroup;

    public bool IsOpen
    {
        get { return animator.GetBool("IsOpen"); }
        set { animator.SetBool("IsOpen", value); }
    }

    public void Awake()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = rectTransform.offsetMin = Vector2.zero;
    }

    public void Update()
    {
        //if the current animation is not called open disable interaction.
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
        }
        //else allow it.
        else
        {
            canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
        }
    }
}
