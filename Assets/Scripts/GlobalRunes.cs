using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains important functions for managing runes as a whole group

public class GlobalRunes : MonoBehaviour
{
    // The queue of all updates to runes to be completed
    private Queue<RuneUpdate> updateQ = null;

    // Determines if currently processing a rune update or not
    private bool updating = false;

    // Struct for storing info about an update
    private struct RuneUpdate
    {
        public GameObject rune;
        public string mode;
        public float rot;
        public Vector2 point;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (updateQ == null)
        {
            updateQ = new Queue<RuneUpdate>();
        }

        if ((!updating) && (updateQ.Count > 0))
        {
            updating = true;
            RuneUpdate r = updateQ.Dequeue();
            if (r.mode == "rotate")
            {
                r.rune.GetComponent<Rune>().Rotate(r.rot);
            }
            if (r.mode == "adjust_length")
            {
                r.rune.GetComponent<Rune>().AdjustLength(r.point);
            }
            updating = false;
        }
    }

    public void Test()
    {
        Debug.Log("Test successful!");
    }


    // AddQueue adds a rune and the desired update to the queue
    public void AddQueue(GameObject rune, string mode, float rot, Vector2 point)
    {
        if (updateQ == null)
        {
            updateQ = new Queue<RuneUpdate>();
        }

        // Create the struct
        RuneUpdate u;
        u.rune = rune;
        u.mode = mode;
        u.rot = rot;
        u.point = point;

        // Enqueue the update
        updateQ.Enqueue(u);
        
        return;
    }
}

