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
    public RuneType type = RuneType.WATER;
    // The associated component script for this type
    private MonoBehaviour type_script;

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

        SetRuneType(type);
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


    /** SET RUNE TYPE **/
    // Updates the type of the rune
    public void SetRuneType(RuneType t)
    {
        type = t;
        type_script = null;
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

    /** ROTATE **/
    // Rotates the rune on the z-axis by a change in degrees d
    public void Rotate(float d)
    {
        // variables for storing info needed if there's a cross
        bool crossed = false;
        GameObject other_rune = null;
        Vector2 point = new Vector2(0, 0);

        /**** rotate the whole rune ****/
        rot += d;
        if (rot > 360)
        {
            rot = rot - 360;
        }
        if (rot < -360)
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

        /*** Cast ray twice: in layer 8 ('Wall'), then in layer 9 ('Rune') ***/
        // Determine if hits a wall, and update wall_reach if so
        int layerMask = 1 << 8;
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
            float angle1 = rot;
            float angle2 = other_rune.GetComponent<Rune>().rot;
            if (angle1 < 0)
            {
                angle1 = angle1 + 360;
            }
            if (angle2 < 0)
            {
                angle2 = angle2 + 360;
            }
            // Check for near parallelism
            if (Mathf.Abs((angle1 - angle2) - 180) < 4)
            {
                Debug.Log("Nearly parallel " + rot + " " + other_rune.GetComponent<Rune>().rot);
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
            allRunes.AddQueue(crossingRune, "rotate", 0f, new Vector2(0, 0));
            crossingRune = null;

            if (hybridRune != null)
            {
                Debug.Log("about to call destroy");
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
    public void Cross(Vector3 point, GameObject other)
    {
        // If this rune was crossing another rune that is NOT other, the previous cross is broken
        // Therefore, make the old crossingRune recalculate its reach and delete the hybrid
        if ((crossingRune != null) && (crossingRune != other))
        {
            crossingRune.GetComponent<Rune>().hybridRune = null;
            crossingRune.GetComponent<Rune>().crossingRune = null;
            crossingRune.GetComponent<Rune>().Rotate(0);

            hybridRune.GetComponent<Rune>().DestroyRune();
            hybridRune = null;
        }

        // Make the other rune stop at the intersection point
        allRunes.AddQueueFirst(other, "adjust_length", 0f, point);

        // Set crossing to the other rune
        crossingRune = other;
        // Tell the other rune that it is crossing this rune
        crossingRune.GetComponent<Rune>().crossingRune = gameObject;

        // Now, put the hybrid rune at the intersection point
        // with an angle of (r1 + r2)/2 aka the average of the two angles
        float avg_rot = ((rot + crossingRune.GetComponent<Rune>().rot) / 2);
        // Create hybrid if doesn't already exist (this is a new cross)
        if (hybridRune == null)
        {
            float temp = rot;
            rot = avg_rot;
            hybridRune = Instantiate(gameObject, point, Quaternion.Euler(0f, 0f, avg_rot), transform.parent);
            rot = temp;
        }
        else
        {
            hybridRune.GetComponent<Rune>().SetPosition(point);
            allRunes.AddQueueFirst(hybridRune, "rotate", (avg_rot - hybridRune.GetComponent<Rune>().rot),
                                    new Vector2(0, 0));
        }

        // Tell the other rune that it has this hybrid
        crossingRune.GetComponent<Rune>().hybridRune = hybridRune;
    }

    /** DESTROY RUNE **/
    // Destroys this rune; DOES NOT UPDATE VARIABLES IN OTHER RUNES
    // In other words, make sure all references to this rune are deleted too
    // when calling this function
    public void DestroyRune()
    {
        Debug.Log("destroying");
        Destroy(gameObject);
    }
}
