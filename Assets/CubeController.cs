using UnityEngine;

public class CubeController : MonoBehaviour
{
    private UnityEngine.Renderer cubeRenderer;
    private Collider cubeCollider;
    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        cubeCollider = GetComponent<Collider>();
    }

    void Update()
    {
    }

    void OnBecameInvisible()
    {
    }

    void OnBecameVisible()
    {
    }

    private bool IsSeenByPlayer()
    {
        Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(cubeRenderer.bounds.center);

        //Is in front
        if (pointOnScreen.z < 0)
        {
            Debug.Log("Behind: " + name);
            return false;
        }

        //Is in FOV
        if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
                (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        {
            Debug.Log("OutOfBounds: " + name);
            return false;
        }

        RaycastHit hit;
        Vector3 heading = transform.position - Camera.main.transform.position;
        Vector3 direction = heading.normalized;// / heading.magnitude;

        if (Physics.Linecast(Camera.main.transform.position, cubeRenderer.bounds.center, out hit))
        {
            if (hit.transform.name != name)
            {
                /* -->
                Debug.DrawLine(cam.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, Color.red);
                Debug.LogError(toCheck.name + " occluded by " + hit.transform.name);
                */
                Debug.Log(name + " occluded by " + hit.transform.name);
                return false;
            }
        }
        return true;
    }
}
