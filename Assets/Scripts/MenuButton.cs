using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public void StartGame() {
        SceneManager.LoadScene("1-0");
    }

    public void Levels() {
        SceneManager.LoadScene("Levels");
    }

    public void LevelOne(int x)
    {
        LevelSelect(1, x);
    }

    public void LevelTwo(int x)
    {
        LevelSelect(2, x);
    }

    public void LevelThree(int x)
    {
        LevelSelect(3, x);
    }

    public void LevelFour(int x)
    {
        LevelSelect(4, x);
    }

    public void LevelFive(int x)
    {
        LevelSelect(5, x);
    }
    public void LevelSelect(int level, int x) {

        SceneManager.LoadScene(level + "-" + x);
    }

    public void QuitGame() {
        Application.Quit();
    }
}