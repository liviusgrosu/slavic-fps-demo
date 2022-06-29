using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivityX = 100f;
    [SerializeField] private float sensitivityY = 100f;
    [SerializeField] private Transform mainCamera = null;

    private float _mouseX, _mouseY;
    private const float Multiplier = 0.01f;

    private float _xRotation;
    private float _yRotation;

    private bool _lockCamera;
    
    private void Start()
    {
        // Hide mouse and lock it to middle of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _lockCamera = !_lockCamera;
        }

        if (_lockCamera)
        {
            return;
        }
        
        // Get mouse input
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");
         
        // Apply sensitivity to input
        _yRotation += _mouseX * sensitivityX * Multiplier;
        _xRotation -= _mouseY * sensitivityY * Multiplier;

        // Clamp X rotation so that it the player cant look back infinitely
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        // Apply rotation input to camera
        mainCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
    }
}
