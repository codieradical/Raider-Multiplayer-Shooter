using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;
using UnityEditor.Animations;
using Raider.Game.Saves;

namespace Raider.Game.Player
{

    public class PlayerAnimationController : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            //Only update the animator of the camera controller allows movement.
            if (!CameraModeController.ControllerInstance.preventMovement)
            {
                PlayerData.localPlayerData.animator.SetFloat("verticalSpeed", Input.GetAxis("Vertical"));
                PlayerData.localPlayerData.animator.SetFloat("horizontalSpeed", Input.GetAxis("Horizontal"));

                //if (Input.GetButton("Jump"))
                //{
                //    attachedAnimator.SetBool("jumping", true);
                //    Invoke("StopJumping", 0.1f);
                //}

                if (Input.GetButton("Run") && Input.GetAxis("Vertical") > 0.25)
                {
                    PlayerData.localPlayerData.animator.SetBool("running", true);
                }
                else
                {
                    PlayerData.localPlayerData.animator.SetBool("running", false);
                }
            }
            else
            {
                StopAnimations();
            }
        }

        public void StopAnimations()
        {
            PlayerData.localPlayerData.animator.SetFloat("verticalSpeed", 0f);
            PlayerData.localPlayerData.animator.SetFloat("horizontalSpeed", 0f);
            PlayerData.localPlayerData.animator.SetBool("running", false);
            PlayerData.localPlayerData.animator.SetBool("jumping", false);
        }

        void StopJumping()
        {
            PlayerData.localPlayerData.animator.SetBool("jumping", false);
        }

        static void DestroyAnimator(PlayerData playerData)
        {
            Destroy(playerData.animator);
        }

        static void SetupNewAnimator(PlayerData playerData, CameraModeController.CameraModes perspective)
        {
            playerData.animator = playerData.gameObject.AddComponent<Animator>();
            playerData.animator.runtimeAnimatorController = PlayerResourceReferences.instance.animatorControllers.GetControllerByPerspective(perspective);
            playerData.animator.avatar = PlayerResourceReferences.instance.raceGraphics.GetAvatarByRace(playerData.character.Race);
        }

        public static void RecreateAnimator(PlayerData playerData, CameraModeController.CameraModes perspective)
        {
            DestroyAnimator(playerData);
            SetupNewAnimator(playerData, perspective);
        }
    }
}