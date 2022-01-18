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
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] multyHit;
            multyHit = Physics.RaycastAll(ray, 100);

            for (int j = 0; j < multyHit.Length; j++) // sortowanie b¹belkowe 
            {
                for (int i = 0; i < multyHit.Length - 1; i++)
                {
                    if (multyHit[i].distance > multyHit[i + 1].distance)
                    {
                        RaycastHit temp = multyHit[i + 1];
                        multyHit[i + 1] = multyHit[i];
                        multyHit[i] = temp;
                    }
                }
            }



            foreach (RaycastHit hitt in multyHit)
            {
                if (hitt.collider.gameObject.layer == 9)
                {
                    pointer.transform.position = new Vector3(hitt.point.x, hitt.point.y + 0.1f, hitt.point.z);
                    break;
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
