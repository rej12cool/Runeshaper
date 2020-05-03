using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour
{
    // Reference to the script with a queue of all runes
    public GlobalRunes allRunes;

    // All rune element types
    public enum RuneType
    {
        WATER,
        FIRE,
        EARTH,
        AIR,
        STEAM
    }

    // The direction of the rune (0 is straight up)
    public float rot;
    // The furthest distance that the rune effect can reach
    public float max_reach = 5;
    // The scale of the rune, to be used for adjusting the size of runes
    // without messing up the math
    public float scale = 0.5f;

    // The effect area object (collider is trigger)
    public GameObject effectArea;
    // The collider used to check for runes crossing
    public GameObject crossLine;
    // A collider that ignores other runes only shortens if it hits a wall
    // Disabled by default, temporarily activated when recalculating an intersection
    public GameObject wallLine;

    // The type (element) of the rune
    public RuneType type; 

    // The rune this rune is crossing (null if none)
    public GameObject crossingRune;
    // If crossing a rune, this is the hybrid rune created
    public GameObject hybridRune;

    public float test_rot;
    public int test_count;


    // Set up the rune for its current config
    void Start()
    {
        allRunes = transform.parent.GetComponent<GlobalRunes>();

        crossingRune = null;

        float temp = rot;
        rot = 0;
        allRunes.AddQueue(this.gameObject, "rotate", temp, new Vector2(0, 0));
    }

    // Frame update function
    void Update()
    {
        if ((test_rot != 0) && (test_count > 0))
        {
            allRunes.AddQueue(this.gameObject, "rotate", test_rot, new Vector2(0, 0));
            test_count--;
        }
    }

    /** GET RUNE TYPE **/
    // Returns the type of the rune
    public RuneType GetRuneType()
    {
        return type;
    }

    /** SET POSITION **/
    // Moves the base of the rune to the given position
    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    /** GET POSITION **/
    // Returns the position of the base of this rune
    public Vector2 GetPosition()
    {
        return transform.position;
    }

    /** GET VECTOR **/
    // Returns a normalized vector2 with the same angle as this rune
    public Vector2 GetVector()
    {
        return new Vector2((-1 * Mathf.Sin(Mathf.PI * rot / 180)), Mathf.Cos(Mathf.PI * rot / 180));
    }


    /** GET ROTATION **/
    // Returns the rotation of the rune in degrees (a float)
    public float GetRotation()
    {
        return rot;
    }

    /** QUEUE ROTATION **/
    // Public method for changing the rotation by degrees d
    public void QueueRotation(float d)
    {
        // Add the rotation to back of the queue
        allRunes.AddQueue(this.gameObject, "rotate", d, new Vector2(0, 0));
    }

    /** ROTATE **/
    // Rotates the rune on the z-axis by a change in degrees d
    // Should only be called by GlobalRunes, use QueueRotation() otherwise
    public void Rotate(float d)
    {
        // variables for storing info needed if there's a cross
        bool crossed = false;
        GameObject other_rune = null;
        Vector2 point = new Vector2(0, 0);

        /**** rotate the whole rune ****/
        rot += d;
        if (rot > 360.0f)
        {
            rot = rot % 360;
        }
        if (rot < 0.0f)
        {
            rot = rot + 360;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, rot);


        // If this rune is already crossing another rune, temporarily activate that rune's
        // wall collider so can determine if rotating breaks the cross or not
        if (crossingRune != null)
        {
            crossingRune.GetComponent<Rune>().wallLine.GetComponent<BoxCollider2D>().enabled = true;
        }
        // If this rune is crossing AND is making a hybrid rune, temporarily deactivate the
        // hybrid rune's crossing colliders so there isn't a conflict
        if ((crossingRune != null) && (hybridRune != null))
        {
            hybridRune.GetComponent<Rune>().crossLine.GetComponent<BoxCollider2D>().enabled = false;
            hybridRune.GetComponent<Rune>().wallLine.GetComponent<BoxCollider2D>().enabled = false;
        }

        /**** Calculate the furthest the rune can reach in the environment (max distance of max_reach) ****/
        float reach = max_reach;
        float wall_reach = max_reach;

        // Direction of the ray, using the angle
        Vector2 direction = new Vector2((-1 * Mathf.Sin(Mathf.PI * rot / 180)), Mathf.Cos(Mathf.PI * rot / 180));
        // Stores info from raycast hit detection, passed by reference
        RaycastHit2D hitInfo;

        /*** Cast ray twice: in layer 10 ('Platform'), then in layer 9 ('Rune') ***/
        // Determine if hits a wall, and update wall_reach if so
        int layerMask = 1 << 10;
        hitInfo = Physics2D.Raycast(transform.position, direction, max_reach, layerMask);
        if (hitInfo.collider != null)
        {
            wall_reach = hitInfo.distance;
        }
        // Determine if hits a rune before a wall, and set up the cross parameters if so
        crossLine.GetComponent<BoxCollider2D>().enabled = false;
        layerMask = 1 << 9;
        hitInfo = Physics2D.Raycast(transform.position, direction, max_reach, layerMask);
        crossLine.GetComponent<BoxCollider2D>().enabled = true;
        if ((hitInfo.collider != null) && (hitInfo.distance < wall_reach))
        {
            reach = hitInfo.distance;

            crossed = true;
            point = hitInfo.point;
            // Call GetRune method of crossLine that was hit
            other_rune = hitInfo.collider.GetComponent<GetRune>().Rune();
        }
        // Otherwise, set reach to wall_reach
        else
        {
            reach = wall_reach;
        }

        /** One last calculation for reach: **/
        // If this rune is crossing another and they are nearly pointing straight at
        // each other, use an alternative calculation for the intersecting point
        // (to avoid glitchiness caused by raycasting at that point)
        if (other_rune != null)
        {
            float angle1 = GetRotation();
            float angle2 = other_rune.GetComponent<Rune>().GetRotation();

            // Check for near parallelism
            if (Mathf.Abs((angle1 - angle2) - 180) < 6)
            {
                // Set intersection to midpoint between the two
                point.x = transform.position.x + ((other_rune.transform.position.x - transform.position.x) / 2);
                point.y = transform.position.y + ((other_rune.transform.position.y - transform.position.y) / 2);
                // Set reach to distance to midpoint
                reach = Vector2.Distance(transform.position, point);
            }
        }


        /***********/

        // Disable the collider for wallCollide of crossing rune (if exists) since we're done with it
        if (crossingRune != null)
        {
            crossingRune.GetComponent<Rune>().wallLine.GetComponent<BoxCollider2D>().enabled = false;
        }
        // Re-enable the colliders of the hybrid rune (if exists)
        if (hybridRune != null)
        {
            hybridRune.GetComponent<Rune>().crossLine.GetComponent<BoxCollider2D>().enabled = true;
        }


        /**** adjust the effect area and cross line to the reach by scaling and moving the box ****/
        // Scale effect area and rune cross line by true reach
        effectArea.transform.localScale = new Vector3(1f, reach / scale, 1f);
        crossLine.transform.localScale = new Vector3(0.01f, reach / scale, 0.01f);
        float y = ((reach - (1f * scale)) / 2f) / scale;
        effectArea.transform.localPosition = new Vector3(0f, y, 0f);
        crossLine.transform.localPosition = new Vector3(0f, y, 0f);

        // Scale wall reach line by wall_reach, then disable its collider
        wallLine.transform.localScale = new Vector3(0.01f, wall_reach / scale, 0.01f);
        y = ((wall_reach - (1f * scale)) / 2f) / scale;
        wallLine.transform.localPosition = new Vector3(0f, y, 0f);
        wallLine.GetComponent<BoxCollider2D>().enabled = false;


        /**** If this rune crossed another rune, call the cross function ****/
        if (crossed)
        {
            Cross(point, other_rune);
        }
        // If it didn't but it was previously crossing a rune, reset that rune by
        // removing the cross and recalcing the rotation, and remove the hybrid rune
        else if (crossingRune != null)
        {
            crossingRune.GetComponent<Rune>().hybridRune = null;
            crossingRune.GetComponent<Rune>().crossingRune = null;
            allRunes.AddQueueFirst(crossingRune, "rotate", 0f, new Vector2(0, 0));
            crossingRune = null;

            if (hybridRune != null)
            {
                hybridRune.GetComponent<Rune>().DestroyRune();
                hybridRune = null;
            }
        }
    }

    /** ADJUST LENGTH **/
    // Adjusts the reach to end at the vector2 point given; used to cut a rune effect short when
    // another rune has intersected this rune
    public void AdjustLength(Vector2 point)
    {
        // calculate distance between rune base and point, set reach to that length
        float reach = Vector2.Distance(transform.position, point);

        /**** adjust the effect area to the reach by scaling and moving the box ****/
        effectArea.transform.localScale = new Vector3(1f, reach / scale, 1f);
        crossLine.transform.localScale = new Vector3(0.0f, reach / scale, 0.01f);
        float y = ((reach - (1f * scale)) / 2f) / scale;
        effectArea.transform.localPosition = new Vector3(0f, y, 0f);
        crossLine.transform.localPosition = new Vector3(0f, y, 0f);
        // (don't adjust the wall reach (aka wallLine)
    }

    /** CROSS **/
    // Called when this rune has crossed another rune, 'other'
    // Uses the point of intersection to make adjustments and create the hybrid rune effect
    public void Cross(Vector2 point, GameObject other)
    {
        // If this rune was crossing another rune that is NOT other, the previous cross is broken
        // Therefore, make the old crossingRune recalculate its reach and delete the hybrid
        if ((crossingRune != null) && (crossingRune != other))
        {
            crossingRune.GetComponent<Rune>().hybridRune = null;
            crossingRune.GetComponent<Rune>().crossingRune = null;
            allRunes.AddQueueFirst(crossingRune, "rotate", 0f, new Vector2(0, 0));

            if (hybridRune != null)
            {
                hybridRune.GetComponent<Rune>().DestroyRune();
                hybridRune = null;
            }
        }

        // Set crossing rune to the other rune
        crossingRune = other;
        // Tell the other rune that it is crossing this rune
        // But first, reset the crossing rune OF the other rune if it is not this rune
        // (Basically if the other rune was already crossing a different rune,
        // tell that third rune to recalculate itself)
        GameObject crossOfOther = crossingRune.GetComponent<Rune>().crossingRune;
        if ((crossOfOther != null) && (crossOfOther != gameObject))
        {
            allRunes.AddQueueFirst(crossOfOther, "rotate", 0f, new Vector2(0, 0));
        }
        crossingRune.GetComponent<Rune>().crossingRune = gameObject;


        /*** Determine if a hybrid rune should be produced, and if so, get the properties it should have ***/
        float h_max_reach = 0f;
        GameObject h_prefab = null;
        if (HasHyridResult(this, crossingRune.GetComponent<Rune>(), point, ref h_max_reach, ref h_prefab))
        {
            // Now, put the hybrid rune at the intersection point
            // with an angle of (r1 + r2)/2 aka the average of the two vectors
            float avg_rot = Vector2.Angle(new Vector2(0, 1), ((GetVector() + crossingRune.GetComponent<Rune>().GetVector()) / 2));
            // Create hybrid if doesn't already exist (this is a new cross)
            if (hybridRune == null)
            {
                // Instantiate a new (inactive) rune from the Prefab
                hybridRune = Instantiate(h_prefab, point, Quaternion.Euler(0f, 0f, avg_rot), transform.parent);
                // Configure its properties
                hybridRune.GetComponent<Rune>().rot = avg_rot;
                if (h_max_reach > 0)
                    hybridRune.GetComponent<Rune>().max_reach = h_max_reach;
                // Activate hybrid to call its Start() function
                hybridRune.SetActive(true);
            }
            else
            {
                // If already exists, just reconfigure for new params
                hybridRune.GetComponent<Rune>().SetPosition(point);
                if (h_max_reach > 0)
                    hybridRune.GetComponent<Rune>().max_reach = h_max_reach;
                allRunes.AddQueueFirst(hybridRune, "rotate", (avg_rot - hybridRune.GetComponent<Rune>().rot),
                                        new Vector2(0, 0));
            }

            // Make the crossing rune stop at the intersection point
            allRunes.AddQueueFirst(crossingRune, "adjust_length", 0f, point);

            // Tell the other rune that it has this hybrid
            crossingRune.GetComponent<Rune>().hybridRune = hybridRune;
        }
    }

    /** HAS HYBRID RESULT? **/
    // Given two runes, determines if crossing them produces a hybrid by
    // checking their types. Returns true if so, false otherwise.
    // Also takes the point of intersection to calculate max_reach (if necessary)
    // -- Secondary return values --
    // 1) If h_max_reach is > 0, then this is the maximum reach the hybrid should have.
    //    If is equal to 0, use the default max reach for that type of rune.
    // 2) h_prefab will be set to rune type prefab that the hybrid should be a clone of.
    bool HasHyridResult(Rune r1, Rune r2, Vector2 point, ref float h_max_reach, ref GameObject h_prefab)
    {
        // AIR + AIR => AIR
        if ((r1.type == RuneType.AIR) && (r2.type == RuneType.AIR))
        {
            h_prefab = (GameObject)Resources.Load("Prefabs/Runes/AirRune", typeof(GameObject));

            // Two air runes produce a third air rune with a max reach that is 
            // the sum of their remaining lengths
            // (The shorter the two runes, the longer their hybrid)
            h_max_reach = r1.max_reach - Vector2.Distance(r1.GetPosition(), point);
            h_max_reach += r2.max_reach - Vector2.Distance(r2.GetPosition(), point);

            // If goes nonpositive, default to 0.5
            if (h_max_reach <= 0)
                h_max_reach = 0.5f;

            return true;
        }

        // WATER + FIRE => STEAM
        if (((r1.type == RuneType.FIRE) && (r2.type == RuneType.WATER))
            || ((r2.type == RuneType.FIRE) && (r1.type == RuneType.WATER)))
        {
            h_prefab = (GameObject)Resources.Load("Prefabs/Runes/AirRune", typeof(GameObject));
            h_max_reach = 0;
            return true;
        }

        return false;
    }

    /** DESTROY RUNE **/
    // Destroys this rune; DOES NOT UPDATE VARIABLES IN OTHER RUNES
    // In other words, make sure all references to this rune are deleted too
    // when calling this function
    public void DestroyRune()
    {
        Destroy(gameObject);
    }
}
