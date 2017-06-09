using Raider.Game.Networking;
using UnityEngine;

namespace Raider.Game.Cameras
{
	/// <summary>
	/// Camera controller for watching other players in the scene.
	/// </summary>
    public class SpectatorThirdPersonCameraController : ThirdPersonCameraController
    {
        public override void Setup()
        {
            base.Setup();

            parent = NetworkGameManager.instance.Players[CameraModeController.singleton.spectatingPlayerIndex].transform;
            characterController = CameraModeController.singleton.localPlayerGameObject.GetComponent<CharacterController>();
        }

		/// <summary>
		/// Every frame:
		/// Rotate the camera
		/// Make sure the camera doesn't roll
		/// Allow the user to adjust the view distance
		/// Allow the user to switch spectating player.
		/// </summary>
        void Update()
        {
            RotateCamera();
            LockCamPointZRotation();
            UpdateCameraDistance();
			SwitchSpectatingPlayer();
        }

		/// <summary>
		/// If the up arrow key is pressed, go to the next player to spectate.
		/// If the down arrow key is pressed, go to the previous player.
		/// </summary>
		void SwitchSpectatingPlayer()
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				//If the player being spectated is the last in queue, go back to the beginning.
				if ((CameraModeController.singleton.spectatingPlayerIndex + 1) >= NetworkGameManager.instance.Players.Count)
					CameraModeController.singleton.spectatingPlayerIndex = 0;
				else //else go continue.
					CameraModeController.singleton.spectatingPlayerIndex += 1;
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				if ((CameraModeController.singleton.spectatingPlayerIndex - 1) <= -1)
					CameraModeController.singleton.spectatingPlayerIndex = NetworkGameManager.instance.Players.Count - 1;
				else
					CameraModeController.singleton.spectatingPlayerIndex -= 1;
			}

			CameraModeController.singleton.CameraParent = NetworkGameManager.instance.Players[CameraModeController.singleton.spectatingPlayerIndex].transform;
		}
	}
}
