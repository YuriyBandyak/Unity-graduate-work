using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedForm : MonoBehaviour
{
    //stats 
    public float size = 0;
    public float speed = 0;
    public float power = 0;
    public float speedOfDisappearing = 0;

    public Vector3 direction;
    public GameObject parent;

    //reference
    public GameObject magicType;

    private void Start()
    {
        size = MagicBalance.Directed.size;
        speed = MagicBalance.Directed.speed;
        power = MagicBalance.Directed.power;
        speedOfDisappearing = MagicBalance.Directed.speedOfDisappearing;
        
    }

    /// <summary>
    /// Kind of constructor
    /// for now size and speed are constants, but maybe better to change them to something better
    /// </summary>
    public void setStats(GameObject magicType, Vector3 direction, GameObject parent) //Kind of construcotr, maybe should change it to normal constructor, not the method
    {
        this.magicType = magicType;
        this.direction = direction;
        this.parent = parent;
        GameObject type = Instantiate(this.magicType, gameObject.transform);
    }

    public void setStats(GameObject magicType, GameObject parent)
    {
        this.magicType = magicType;
        this.parent = parent;
        GameObject type = Instantiate(this.magicType, gameObject.transform);
    }

    private void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        size -= speedOfDisappearing; // change in terms of balance, i mean depends on power
        transform.localScale = new Vector3(size, size, size); // childrens also scale with parent
        if (transform.GetChild(0).Find("Particles"))
        {
            transform.GetChild(0).Find("Particles").localScale = new Vector3(size, size, size);
        }

        if (transform.GetChild(0).Find("Light")) // not sure if it works
        {
            transform.GetChild(0).Find("Light").GetComponent<Light>().range -= speedOfDisappearing;
        }

        if (size <= 0.2) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("name: " + other.name);
        if (other.gameObject.layer == 6) // enemy
        {
            if (other.transform.parent.GetComponent<SimpleEnemy>())
            {
                other.transform.parent.GetComponent<SimpleEnemy>().GetDamage(power);

                /*
                 * should add type of spell to constructor and if its fire then add effect on enemy or player
                 */

                Destroy(gameObject);
            }
        }
        else if(other.gameObject.layer == 3) //player
        {
            if (other.transform.parent.GetComponent<PlayerStatsAndFunction>())
            {
                other.transform.parent.GetComponent<PlayerStatsAndFunction>().GetDamage(power);
                Destroy(gameObject);
            }
        }
    }
}
