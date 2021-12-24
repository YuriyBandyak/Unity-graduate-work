using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script mainly for pointer.
/// What should be changed/added: proper pointer position - there will be problems with vertical levels, 
///     so script should know what is terrein and where pointer should be setted(only on terrain, not on tree for example)
/// </summary>
public class MouseController : MonoBehaviour
{
    //References
    public Camera mainCamera;
    public GameObject pointer;

    
    public void ShowPointer()
    {
        {
            Ray ray;
            RaycastHit hit;

            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider != null)
                {
                    //pointer.transform.position = hit.point + new Vector3(0, 0.1f, 0); // old and wrong
                    pointer.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
                }
            }
        }
    }

    public void lookAt() // not my code
    {
        // Store the other object's position in a temporary variable
        var temp = pointer.transform.position;
        // Deflate it's x and z coordinate
        temp.x = temp.z = uint.MinValue;
        var lookRotation = Quaternion.LookRotation(temp);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0);
        transform.LookAt(pointer.transform);
    }
}
