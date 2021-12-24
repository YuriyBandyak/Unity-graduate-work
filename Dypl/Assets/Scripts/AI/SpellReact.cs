using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellReact : MonoBehaviour
{
    private SimpleEnemy parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<SimpleEnemy>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("name: " + other.name);
        if (other.gameObject.layer == 7)
        {
            if (other.gameObject.transform.GetComponent<DirectedForm>())
            {
                parent.GetDamage(other.gameObject.transform.GetComponent<DirectedForm>().power);
            }
            else if (other.gameObject.transform.GetComponent<RingForm>())
            {
                parent.GetDamage(other.gameObject.transform.GetComponent<RingForm>().power);
            }
            else if (other.gameObject.transform.GetComponent<RayForm>())
            {
                parent.GetDamage(other.gameObject.transform.GetComponent<RayForm>().power);
            }
            else if (other.gameObject.transform.GetComponent<ConeForm>())
            {
                parent.GetDamage(other.gameObject.transform.GetComponent<ConeForm>().power);
            }
        }
    }
}
