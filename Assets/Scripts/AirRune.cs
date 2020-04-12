using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRune : MonoBehaviour
{
    public float force = 20;

    public ArrayList inside;

    void Start()
    {
        inside = new ArrayList();
    }

    void Update()
    {
        if (inside.Count > 0)
        {
            // Get rune component
            Rune rune = transform.parent.GetComponent<Rune>();

            // Get direction vector of the rune
            Vector2 v = rune.GetVector();

            // Loop through all entities that have entered the effect area
            IEnumerator i = inside.GetEnumerator();
            while (i.MoveNext())
            {
                GameObject g = (GameObject) i.Current;

                float thrust = force;

                g.GetComponent<Rigidbody2D>().AddForce(v * thrust);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inside.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        inside.Remove(other.gameObject);
    }
}