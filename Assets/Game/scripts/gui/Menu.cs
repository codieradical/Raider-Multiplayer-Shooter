using UnityEngine;

namespace Raider.Game.GUI
{
	/// <summary>
	/// Please refactor me!!!
	/// Unused menus need to be disabled to reduce update calls on ui layout scripts.
	/// </summary>
	[RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : MonoBehaviour
    {

        private Animator animator;
        private CanvasGroup canvasGroup;

        public bool IsOpen
        {
            get { return animator.GetBool("open"); }
            set { animator.SetBool("open", value); }
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
}