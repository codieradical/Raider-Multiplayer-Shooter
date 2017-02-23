using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;
using Raider.Game.Saves;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        #region Animator Property Accessors

        float LocalVerticalSpeed
        {
            set
            {
                playerAnimator.SetFloat("verticalSpeed", value);
            }
        }
        float LocalHorizontalSpeed
        {
            set
            {
                playerAnimator.SetFloat("horizontalSpeed", value);
            }
        }
        bool LocalRunning
        {
            set
            {
                playerAnimator.SetBool("running", value);
            }
        }
        bool LocalJumping
        {
            set
            {
                playerAnimator.SetBool("jumping", value);
            }
        }

        float SharedVerticalSpeed
        {
            get
            {
                return parameterAnimator.GetFloat("verticalSpeed");
            }
        }
        float SharedHorizontalSpeed
        {
            get
            {
                return parameterAnimator.GetFloat("horizontalSpeed");
            }
        }
        bool SharedRunning
        {
            get
            {
                return parameterAnimator.GetBool("running");
            }
        }
        bool SharedJumping
        {
            get
            {
                return parameterAnimator.GetBool("jumping");
            }
        }


        #endregion

        public Animator playerAnimator; //Assigned in editor.
        private Animator parameterAnimator { get { return this.transform.root.GetComponent<Animator>(); } }

        void Update()
        {
            //Grab parameters from the shared animator.
            LocalVerticalSpeed = SharedVerticalSpeed;
            LocalHorizontalSpeed = SharedHorizontalSpeed;
            LocalRunning = SharedRunning;
            LocalJumping = SharedJumping;
        }
    }
}