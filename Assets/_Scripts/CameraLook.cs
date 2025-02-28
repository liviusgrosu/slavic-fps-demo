using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivityX = 100f;
    [SerializeField] private float sensitivityY = 100f;
    private Transform _mainCamera;

    private float _mouseX, _mouseY;
    private const float Multiplier = 0.01f;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        _mainCamera = Camera.main.transform;
        // Hide mouse and lock it to middle of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get mouse input
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");
         
        // Apply sensitivity to input
        _yRotation += _mouseX * sensitivityX * Multiplier;
        _xRotation -= _mouseY * sensitivityY * Multiplier;

        // Clamp X rotation so that it the player cant look back infinitely
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        // Apply rotation input to camera
        _mainCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
    }
}
