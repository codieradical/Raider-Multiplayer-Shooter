using UnityEngine;
using Raider.Game.Cameras;
using System.Collections;
using Raider.Game.Saves;
using UnityEngine.Networking;

namespace Raider.Game.Player
{

    public class WeaponAnimatorController : MonoBehaviour
    {
        #region Animator Property Accessors

        float LocalVerticalSpeed
        {
            set
            {
                weaponAnimator.SetFloat("verticalSpeed", value);
            }
        }
        float LocalHorizontalSpeed
        {
            set
            {
                weaponAnimator.SetFloat("horizontalSpeed", value);
            }
        }
        bool LocalRunning
        {
            set
            {
                weaponAnimator.SetBool("running", value);
            }
        }
        bool LocalJumping
        {
            set
            {
                weaponAnimator.SetBool("jumping", value);
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

        public Animator weaponAnimator; //Assigned in editor.
        private Animator parameterAnimator { get { return PlayerData.localPlayerData.sharedParametersAnimator; } }

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