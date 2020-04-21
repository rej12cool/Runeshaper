using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollFG : MonoBehaviour
{
    [SerializeField]
    private Transform centerBackground;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= centerBackground.position.x + 28.43f)
            centerBackground.position = new Vector2(centerBackground.position.x + 28.43f, transform.position.y);

        else if (transform.position.x <= centerBackground.position.x - 28.43f)
            centerBackground.position = new Vector2(centerBackground.position.x - 28.43f, transform.position.y);
    }
}
