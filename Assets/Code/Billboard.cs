using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera ar;

    void Start()
    {
        ar = Camera.main;
    }
       

    void Update()
    {
        Vector3 direction = ar.transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
