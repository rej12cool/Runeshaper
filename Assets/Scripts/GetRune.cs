using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script given to children of a rune object

// Assign this script if some process knows the child and
// needs to know what rune (and its matching Rune script)
// the object is part of.

public class GetRune : MonoBehaviour
{
    // The Rune of this object (must assign manually)
    public GameObject rune;

    // METHOD: Rune
    // Returns the Rune object associated with this object
    public GameObject Rune()
    {
        return rune;
    }
}
