using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 100f; // Speed of the spin (degrees per second)

    void Update()
    {
        // Rotate the GameObject on the Y-axis
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }
}
