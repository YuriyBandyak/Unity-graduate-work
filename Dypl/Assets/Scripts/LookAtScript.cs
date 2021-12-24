using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    ////stats
    //public bool freezeXRotate;
    //public bool freezeYRotate;
    //public bool freezeZRotate;

    //public float xRotate;
    //public float yRotate;
    //public float zRotate;

    //references
    public GameObject target;

    //private void Start()
    //{
    //    xRotate = transform.rotation.x;
    //    yRotate = transform.rotation.y;
    //    zRotate = transform.rotation.z;
    //}

    void Update()
    {
        transform.LookAt(target.transform);
        transform.Rotate(new Vector3(90, 0, 0));
        //if (freezeXRotate) transform.Rotate(new Vector3(xRotate, gameObject.transform.rotation.y, gameObject.transform.rotation.z));
        //if (freezeYRotate) transform.Rotate(new Vector3(gameObject.transform.rotation.x, yRotate, gameObject.transform.rotation.z));
        //if (freezeZRotate) transform.Rotate(new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, zRotate));
    }
}
