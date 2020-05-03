using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public void RestartGame() {
        SceneManager.LoadScene("Jump Tutorial");
    }

    public void QuitGame() {
        Application.Quit();
    }
}