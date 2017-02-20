using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;
using Raider.Game.Saves;

namespace Raider.Game.Player
{

    public class PlayerAnimationController : MonoBehaviour
    {
        //A couple of properties to simplify references...
        private Animator playerAnimator { get { return PlayerData.localPlayerData.playerModelAnimator; } }
        private Animator weaponAnimator { get { return PlayerData.localPlayerData.weaponModelAnimator; } }

        // Update is called once per frame
        void Update()
        {

            //Only update the animator of the camera controller allows movement.
            if (!CameraModeController.ControllerInstance.preventMovement)
            {
                //Give all of the animators the movement speed on both axis.
                playerAnimator.SetFloat("verticalSpeed", Input.GetAxis("Vertical"));
                playerAnimator.SetFloat("horizontalSpeed", Input.GetAxis("Horizontal"));
                if (weaponAnimator != null)
                {
                    weaponAnimator.SetFloat("verticalSpeed", Input.GetAxis("Vertical"));
                    weaponAnimator.SetFloat("horizontalSpeed", Input.GetAxis("Horizontal"));
                }

                //if (Input.GetButton("Jump"))
                //{
                //    attachedAnimator.SetBool("jumping", true);
                //    Invoke("StopJumping", 0.1f);
                //}

                //Tell all of the animators that the player is running.
                if (Input.GetButton("Run") && Input.GetAxis("Vertical") > 0.25)
                {
                    playerAnimator.SetBool("running", true);
                    if (weaponAnimator != null)
                        weaponAnimator.SetBool("running", true);
                }
                else
                {
                    playerAnimator.SetBool("running", false);
                    if (weaponAnimator != null)
                        weaponAnimator.SetBool("running", false);
                }
            }
            else
            {
                StopAnimations();
            }
        }

        public void StopAnimations()
        {
            playerAnimator.SetFloat("verticalSpeed", 0f);
            playerAnimator.SetFloat("horizontalSpeed", 0f);
            playerAnimator.SetBool("running", false);
            playerAnimator.SetBool("jumping", false);
            if (weaponAnimator != null)
            {
                weaponAnimator.SetFloat("verticalSpeed", 0f);
                weaponAnimator.SetFloat("horizontalSpeed", 0f);
                weaponAnimator.SetBool("running", false);
                weaponAnimator.SetBool("jumping", false);
            }
        }

        void StopJumping()
        {
            playerAnimator.SetBool("jumping", false);
            if (weaponAnimator != null)
            {
                weaponAnimator.SetBool("jumping", false);
            }
        }
    }
}