using Raider.Game.Networking;
using UnityEngine;

namespace Raider.Game.Cameras
{
    public class SpectatorThirdPersonCameraController : ThirdPersonCameraController
    {
        public override void Setup()
        {
            base.Setup();

            parent = NetworkGameManager.instance.Players[CameraModeController.singleton.spectatingPlayerIndex].transform;
            characterController = CameraModeController.singleton.localPlayerGameObject.GetComponent<CharacterController>();
        }

        void Update()
        {
            RotateCamera();
            LockCamPointZRotation();
            UpdateCameraDistance();
			SwitchSpectatingPlayer();
        }

		void SwitchSpectatingPlayer()
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				if ((CameraModeController.singleton.spectatingPlayerIndex + 1) >= NetworkGameManager.instance.Players.Count)
					CameraModeController.singleton.spectatingPlayerIndex = 0;
				else
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
