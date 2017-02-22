using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;
using Raider.Game.Saves;
using UnityEngine.Networking;

namespace Raider.Game.Player
{

    public class PlayerAnimationController : NetworkBehaviour
    {
        #region NetworkAnimator Replacement

        //ANIMATOR PROPERTIES
        //These are set by the local player using the properties below and synced automatically.
        [SyncVar(hook = "OnVerticalSpeedSync")] float verticalSpeed;
        [SyncVar(hook = "OnHorizontalSpeedSync")] float horizontalSpeed;
        [SyncVar(hook = "OnRunningSync")] bool running;
        [SyncVar(hook = "OnJumpingSync")] bool jumping;
    
        //ANIMATOR ACCESSORS
        //For the local player, these update the animator, then the properties above, to sync the changes.
        //For the other players, these only update the animator.
        float VerticalSpeed
        {
            set
            {
                if (playerAnimator != null)
                    playerAnimator.SetFloat("verticalSpeed", value);
                if (weaponAnimator != null)
                    weaponAnimator.SetFloat("verticalSpeed", value);

                if(isLocalPlayer)
                    verticalSpeed = value;
            }
        }

        float HorizontalSpeed
        {
            set
            {
                if (playerAnimator != null)
                    playerAnimator.SetFloat("horizontalSpeed", value);
                if (weaponAnimator != null)
                    weaponAnimator.SetFloat("horizontalSpeed", value);

                if (isLocalPlayer)
                    horizontalSpeed = value;
            }
        }

        bool Running
        {
            set
            {
                if (playerAnimator != null)
                    playerAnimator.SetBool("running", value);
                if (weaponAnimator != null)
                    weaponAnimator.SetBool("running", value);

                if (isLocalPlayer)
                    running = value;
            }
        }

        bool Jumping
        {
            set
            {
                if (playerAnimator != null)
                    playerAnimator.SetBool("jumping", value);
                if (weaponAnimator != null)
                    weaponAnimator.SetBool("jumping", value);

                if (isLocalPlayer)
                    jumping = value;
            }
        }

        //SYNCVAR HANDLERS
        //These update the animators when new values are recieved.
        //There's no need for the other players to even store these values for now,
        //So at the moment they're sent straight to the animator.
        void OnVerticalSpeedSync(float value)
        {
            if (!isLocalPlayer)
            {
                //verticalSpeed = value; Not Needed Yet
                VerticalSpeed = value;
            }
        }

        void OnHorizontalSpeedSync(float value)
        {
            if (!isLocalPlayer)
                HorizontalSpeed = value;
        }

        void OnRunningSync(bool value)
        {
            if (!isLocalPlayer)
                Running = value;
        }

        void OnJumpingSync(bool value)
        {
            if (!isLocalPlayer)
                Jumping = value;
        }

        #endregion

        //A couple of properties to simplify references...
        private Animator playerAnimator { get { return PlayerData.localPlayerData.playerModelAnimator; } }
        private Animator weaponAnimator { get { return PlayerData.localPlayerData.weaponModelAnimator; } }

        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer)
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
                    if (Input.GetButton("Run") && Input.GetAxis("Vertical") > 0.25)
                        Running = true;
                    else
                        Running = false;
                }
                else
                    StopAnimations();
            }
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