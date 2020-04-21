using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG2 : MonoBehaviour
{
    [SerializeField]
    private Transform centerBackground;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= centerBackground.position.x + 21.33f)
            centerBackground.position = new Vector2(centerBackground.position.x + 21.33f, transform.position.y);

        else if (transform.position.x <= centerBackground.position.x - 21.33f)
            centerBackground.position = new Vector2(centerBackground.position.x - 21.33f, transform.position.y);
    }
}
