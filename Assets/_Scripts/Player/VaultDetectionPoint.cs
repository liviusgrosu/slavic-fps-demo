using UnityEngine;

public class VaultDetectionPoint : MonoBehaviour
{
    private Transform _camera;

    private float _forwardDistance, _upwardDistance;
    public Vector3 forwardDir, upwardDir;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.transform;
        var camToPoint = transform.position - _camera.position;
        _forwardDistance = new Vector2(camToPoint.x, camToPoint.z).magnitude;
        _upwardDistance = Mathf.Abs(camToPoint.y);

    }

    // Update is called once per frame
    void Update()
    {
        forwardDir = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized * _forwardDistance;
        upwardDir = Vector3.up * _upwardDistance;

        transform.position = _camera.position + forwardDir + upwardDir;
    }
}
