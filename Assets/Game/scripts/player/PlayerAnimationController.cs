using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;
using Raider.Game.Saves;

namespace Raider.Game.Player
{

    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField]
        public Animator animator { get { return GetComponent<PlayerData>().animator; } }

        // Update is called once per frame
        void Update()
        {
            //Only update the animator of the camera controller allows movement.
            if (!CameraModeController.ControllerInstance.preventMovement)
            {
                animator.SetFloat("verticalSpeed", Input.GetAxis("Vertical"));
                animator.SetFloat("horizontalSpeed", Input.GetAxis("Horizontal"));

                //if (Input.GetButton("Jump"))
                //{
                //    attachedAnimator.SetBool("jumping", true);
                //    Invoke("StopJumping", 0.1f);
                //}

                if (Input.GetButton("Run") && Input.GetAxis("Vertical") > 0.25)
                {
                    animator.SetBool("running", true);
                }
                else
                {
                    animator.SetBool("running", false);
                }
        }
            else
            {
                StopAnimations();
    }
}

        public void StopAnimations()
        {
            animator.SetFloat("verticalSpeed", 0f);
            animator.SetFloat("horizontalSpeed", 0f);
            animator.SetBool("running", false);
            animator.SetBool("jumping", false);
        }

        void StopJumping()
        {
            animator.SetBool("jumping", false);
        }

        public static void UpdateAnimationController(PlayerData playerData, CameraModeController.CameraModes perspective)
        {
            //playerData.animator.runtimeAnimatorController = PlayerResourceReferences.instance.animatorControllers.GetControllerByPerspective(perspective);
            //playerData.animator.avatar = PlayerResourceReferences.instance.raceGraphics.GetAvatarByRace(playerData.character.Race);
        }
    }
}