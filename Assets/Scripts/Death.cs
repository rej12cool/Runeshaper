using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    // Causes the player to "die" and restart the level
    public void Die()
    {
    	SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
