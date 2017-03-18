using UnityEngine;

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
                return ParameterAnimator.GetFloat("verticalSpeed");
            }
        }
        float SharedHorizontalSpeed
        {
            get
            {
                return ParameterAnimator.GetFloat("horizontalSpeed");
            }
        }
        bool SharedRunning
        {
            get
            {
                return ParameterAnimator.GetBool("running");
            }
        }
        bool SharedJumping
        {
            get
            {
                return ParameterAnimator.GetBool("jumping");
            }
        }


        #endregion

        public Animator weaponAnimator; //Assigned in editor.
        private Animator ParameterAnimator { get { return PlayerData.localPlayerData.sharedParametersAnimator; } }

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