using UnityEngine;

abstract public class CameraController : MonoBehaviour
{
    //These values represent local positions and rotations!
    [Header("Camera Point")]
    public Vector3 pointStartingPos = Vector3.zero;
    public Vector3 pointStartingRot = Vector3.zero;
    [Header("Camera")]
    public Vector3 camStartingPos = Vector3.zero;
    public Vector3 camStartingRot = Vector3.zero;

    [SerializeField]
    public GameObject camPoint;
    public GameObject cam;

    public CameraModeController modeController;
    public Transform parent = null;
    public bool preventMovement = false;

    public void Start()
    {
        modeController = GetComponent<CameraModeController>();

        modeController.ChangeCameraParent(parent);

        camPoint = modeController.camPoint;

        //Assign the camera.
        cam = camPoint.transform.GetChild(0).gameObject;

        //Activate the camera.
        cam.SetActive(true);

        ResetCamAndCamPointPosAndRot();
    }

    //Sets the position and rotation of the camera and camPoint to the starting values.
    public void ResetCamAndCamPointPosAndRot()
    {
        cam.transform.localPosition = camStartingPos;
        cam.transform.localEulerAngles = camStartingRot;
        camPoint.transform.localPosition = pointStartingPos;
        camPoint.transform.localEulerAngles = pointStartingRot;
    }

    //Sets the z rotation of camPoint to 0.
    public void LockCamPointZRotation()
    {
        Vector3 _camPointCurrentRot = camPoint.transform.eulerAngles;
        camPoint.transform.eulerAngles = new Vector3(_camPointCurrentRot.x, _camPointCurrentRot.y, 0);
    }

    //Sets the z value of cam to 0.
    public void LockCamZRotation()
    {
        Vector3 _camCurrentRot = cam.transform.eulerAngles;
        cam.transform.eulerAngles = new Vector3(_camCurrentRot.x, _camCurrentRot.y, 0);
    }

    //Sets the y value of camPoint to 0.
    public void LockCamPointYRotation()
    {
        Vector3 _camPointCurrentRot = camPoint.transform.eulerAngles;
        camPoint.transform.localEulerAngles = new Vector3(_camPointCurrentRot.x, 0, _camPointCurrentRot.z);
    }

    /// <summary>
    /// Prevents the a camera rotation from flipping to the back of the player.
    /// </summary>
    /// <param name="_Rotation">The current rotation of the camera or point.</param>
    /// <param name="_Rotate">The proposed rotate of the camera or point.</param>
    /// <returns>Returns the corrected rotation.</returns>
    public Vector3 ApplyXBufferToRotation(Vector3 _currentRotation, Vector3 _rotate)
    {
        if (_currentRotation.x + _rotate.x > 90 - modeController.xAxisBuffer && _currentRotation.x < 270)
        {
            _rotate.x = (90 - modeController.xAxisBuffer) - _currentRotation.x;
        }
        else if (_currentRotation.x + _rotate.x < 270 + modeController.xAxisBuffer && _currentRotation.x > 90)
        {
            _rotate.x = (270 + modeController.xAxisBuffer) - _currentRotation.x;
        }
        return _rotate;
    }
}
