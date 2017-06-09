using Raider.Game.Cameras;
using UnityEngine;

namespace Raider.Game.Player
{

	public class AnimationParametersController : MonoBehaviour
    {
        #region Animator Property Accessors

        float VerticalSpeed
        {
            set
            {
                if (ParameterAnimator != null)
                    ParameterAnimator.SetFloat("verticalSpeed", value);
            }
        }

        float HorizontalSpeed
        {
            set
            {
                if (ParameterAnimator != null)
                    ParameterAnimator.SetFloat("horizontalSpeed", value);
            }
        }

        bool Running
        {
            set
            {
                if (ParameterAnimator != null)
                    ParameterAnimator.SetBool("running", value);
            }
        }

        bool Jumping
        {
            set
            {
                if (ParameterAnimator != null)
                    ParameterAnimator.SetBool("jumping", value);
            }
        }

        #endregion

        //A couple of properties to simplify references...
        private Animator ParameterAnimator { get { return PlayerData.localPlayerData.sharedParametersAnimator; } }

        // Update is called once per frame
        void Update()
        {
            //Only update the animator of the camera controller allows movement.
            if (!CameraModeController.ControllerInstance.preventMovement)
            {
                //Give all of the animators the movement speed on both axis.
                VerticalSpeed = Input.GetAxis("Vertical");
                HorizontalSpeed = Input.GetAxis("Horizontal");

                //if (Input.GetButton("Jump"))
                //{
                //    Jumping = true;
                //    Invoke("StopJumping", 0.1f);
                //}

                //Tell all of the animators that the player is running.
                if (Input.GetButton("Run") && Input.GetAxis("Vertical") > 0.25 && PlayerData.localPlayerData.networkPlayerController.pickupObjective == null)
                    Running = true;
                else
                    Running = false;
            }
            else
                StopAnimations();
        }

        public void StopAnimations()
        {
            VerticalSpeed = 0f;
            HorizontalSpeed = 0f;
            Running = false;
            Jumping = false;
        }

        void StopJumping()
        {
            Jumping = false;
        }
    }
}