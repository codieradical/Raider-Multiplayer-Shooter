using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;

namespace Raider.Game.Player
{

    public class PlayerAnimationController : MonoBehaviour
    {

        private Animator attachedAnimator;

        // Use this for initialization
        void Start()
        {
            attachedAnimator = GetComponent<Player>().graphicsObject.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            //Only update the animator of the camera controller allows movement.
            if (!CameraModeController.ControllerInstance.preventMovement)
            {
                attachedAnimator.SetFloat("verticalSpeed", Input.GetAxis("Vertical"));
                attachedAnimator.SetFloat("horizontalSpeed", Input.GetAxis("Horizontal"));

                //if (Input.GetButton("Jump"))
                //{
                //    attachedAnimator.SetBool("jumping", true);
                //    Invoke("StopJumping", 0.1f);
                //}

                if (Input.GetButton("Run") && Input.GetAxis("Vertical") > 0.25)
                {
                    attachedAnimator.SetBool("running", true);
                }
                else
                {
                    attachedAnimator.SetBool("running", false);
                }
            }
            else
            {
                StopAnimations();
            }
        }

        void StopAnimations()
        {
            attachedAnimator.SetFloat("verticalSpeed", 0f);
            attachedAnimator.SetFloat("horizontalSpeed", 0f);
            attachedAnimator.SetBool("running", false);
        }

        void StopJumping()
        {
            attachedAnimator.SetBool("jumping", false);
        }
    }
}