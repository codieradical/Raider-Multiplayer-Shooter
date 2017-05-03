using Raider.Game.Cameras;
using UnityEngine;
using UnityEngine.Networking;

namespace Raider.Game.Player
{
    // Original script retrieved from the Unity Community wiki,
    // "FPSWalkerEnhanced" by Eric Haines (Eric5h5)
    // The script has been heavily modified for Raider.
    // Originally retrieved over 6 months ago, http://wiki.unity3d.com/index.php?title=FPSWalkerEnhanced&redirect=no
    // Last accessed 13/12/16.

    [RequireComponent(typeof(CharacterController))]
    public class MovementController : NetworkBehaviour
    {
        public MovementAndRotationSettings movSettings = new MovementAndRotationSettings();
        public JumpingAndFallingSettings jumpSettings = new JumpingAndFallingSettings();

		public bool canMove = true;

        [System.Serializable]
        public class MovementAndRotationSettings
        {
            public float walkSpeed = 6f;
            public float runSpeed = 10f;
            // Slows down diagonal movement a little to make it as fast as straight movement.
            public bool limitDiagonalSpeed = true;
            public bool slideOnSlopes = true;
            public float slideSpeed = 12;
            public float antiBumpFactor = .75f;
        }

        [System.Serializable]
        public class JumpingAndFallingSettings
        {
            public float jumpVelocity = 9f;
            public float gravity = 20f;
            public int jumpCount = 3;
            public float jumpCooldownSeconds = 0.25f;
            public float fallDamageThreshold = 10f;
            public bool airControl = true;
        }

        private CharacterController characterController;
        Vector3 moveDirection = Vector3.zero;
        bool grounded;
        int remainingJumps;
        float jumpCooldownTimeStamp;
        private Transform myTransform;
        private float fallStartLevel;
        private float rayDistance;
        private Vector3 contactPoint;
        private bool playerControl = false;
        float speed;
        private RaycastHit hit;
        float fallingFrom;
        bool falling;
        float slideLimit;

        // Use this for initialization
        void Start()
        {
            remainingJumps = jumpSettings.jumpCount;
            characterController = GetComponent<CharacterController>();
            rayDistance = characterController.height * .5f + characterController.radius;
            speed = movSettings.walkSpeed;
            myTransform = transform;
            slideLimit = characterController.slopeLimit - .1f;
        }

        void FixedUpdate()
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

			if (!canMove)
			{
				inputX = 0;
				inputY = 0;
			}

            float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && movSettings.limitDiagonalSpeed) ? .7071f : 1.0f;
            CameraController cameraController = CameraModeController.singleton.GetCameraController();

            if (grounded)
            {
                bool sliding = false;

                //Raycast to check for a slope to slide on.
                if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                        sliding = true;
                }
                //check from the collider.
                else
                {
                    Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                        sliding = true;
                }

                //If the player fell from a high place, damage.
                //Also reset jumps.
                if (falling)
                {
                    falling = false;
                    remainingJumps = jumpSettings.jumpCount;
                    if (myTransform.position.y < fallStartLevel - jumpSettings.fallDamageThreshold)
                        FallingDamageAlert(fallStartLevel - myTransform.position.y);
                }

                //If run key is down runSpeed, else walkSpeed.
                speed = Input.GetButton("Run") ? movSettings.runSpeed : movSettings.walkSpeed;

                // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
                if ((sliding && movSettings.slideOnSlopes) || (movSettings.slideOnSlopes && hit.collider.tag == "Slide"))
                {
                    Vector3 hitNormal = hit.normal;
                    moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                    moveDirection *= movSettings.slideSpeed;
                    playerControl = false;
                }
                // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
                else
                {
                    moveDirection = new Vector3(inputX * inputModifyFactor, -movSettings.antiBumpFactor, inputY * inputModifyFactor);
                    moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                    playerControl = true;
                }

                // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
                if (Input.GetButton("Jump") && remainingJumps > 0 && jumpCooldownTimeStamp <= Time.time)
                {
                    remainingJumps--;
                    moveDirection.y = jumpSettings.jumpVelocity;
                    jumpCooldownTimeStamp = Time.time + jumpSettings.jumpCooldownSeconds;
                }
            }
            else
            {
                // If we stepped over a cliff or something, set the height at which we started falling
                if (!falling)
                {
                    falling = true;
                    fallStartLevel = myTransform.position.y;
                }

                if (Input.GetButton("Jump") && remainingJumps > 0 && jumpCooldownTimeStamp <= Time.time)
                {
                    remainingJumps--;
                    moveDirection.y = jumpSettings.jumpVelocity;
                    jumpCooldownTimeStamp = Time.time + jumpSettings.jumpCooldownSeconds;
                    falling = false;
                }

                // If air control is allowed, check movement but don't touch the y component
                if (jumpSettings.airControl && playerControl)
                {
                    moveDirection.x = inputX * speed * inputModifyFactor;
                    moveDirection.z = inputY * speed * inputModifyFactor;
                    moveDirection = myTransform.TransformDirection(moveDirection);
                }
            }
            // Apply gravity
            moveDirection.y -= jumpSettings.gravity * Time.deltaTime;

            //If the character is moving, center the camera and update rotation

            if (cameraController is ThirdPersonCameraController && ((ThirdPersonCameraController)cameraController).overrideWalking)
            {
                //Cast camController to ThirdPersonCamController.
                ThirdPersonCameraController thirdPersonCamController = (ThirdPersonCameraController)cameraController;

                if (inputX != 0 || inputY != 0)
                {
                    if (!thirdPersonCamController.walking)
                    {
                        //Make the player face the camera.
                        gameObject.transform.eulerAngles = new Vector3(0, cameraController.camPoint.gameObject.transform.eulerAngles.y, 0);
                    }
                    thirdPersonCamController.walking = true;
                    thirdPersonCamController.CenterCamPointAxisY();
                }
                else
                {
                    thirdPersonCamController.walking = false;
                }
            }

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            if (cameraController != null)
            {
                if (!cameraController.preventMovement)
                {
                    grounded = (characterController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
                }
            }
        }

        void Update()
        {
            // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
            // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
            if (grounded && Input.GetButtonDown("Run"))
                speed = (speed == movSettings.walkSpeed ? movSettings.runSpeed : movSettings.walkSpeed);
        }

        // Store point that we're in contact with for use in FixedUpdate if needed
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            contactPoint = hit.point;
        }

        // If falling damage occured, this is the place to do something about it. You can make the player
        // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
        void FallingDamageAlert(float fallDistance)
        {
            print("Ouch! Fell " + fallDistance + " units!");
        }
    }
}