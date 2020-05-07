using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public void RestartGame() {
        SceneManager.LoadScene("1-0");
    }

    public void QuitGame() {
        Application.Quit();
    }
}