using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency;
    public float magnitude;
    public Vector3 direction;
    Vector3 initialPosition;
    public bool bobbing;

    private void Start()
    {
        initialPosition = transform.position;
        bobbing = true;
    }

    private void Update()
    {
        if (bobbing)
        {
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
        }
    }

}
