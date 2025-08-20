using System.Collections;
using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    private bool steering = false;
    private float rotation;
    public void SteerLeft()
    {
        if (!steering)
        {
            StartCoroutine(Steer(true));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Steer(true));
        }

    }
    public void SteerRight()
    {
        if (!steering)
        {
            StartCoroutine(Steer(false));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Steer(false));
        }
    }
    private IEnumerator Steer(bool a)
    {
        steering = true;
        float steered = 0f;
        if (a)
        {
            rotation = 1f;
        }
        else
        {
            rotation = -1f;
        }
        while (steering)
        {
            transform.Rotate(new Vector3(0f, 0f, rotation));
            steered += 1.5f;
            if (steered > 45f)
            {
                steering = false;
            }
            yield return null;
        }

    }

    
}
