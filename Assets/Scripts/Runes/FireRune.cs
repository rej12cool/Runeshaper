﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRune : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Death>().Die();
        }
    }
}
