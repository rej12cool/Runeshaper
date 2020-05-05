 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour  {

        public static bool GameisPaused = false;
        public GameObject pauseMenuUI;
        public AudioMixer mixer;

        void Start(){
                pauseMenuUI.SetActive(false);
        }

        void Update(){
                if (Input.GetKeyDown(KeyCode.Escape)){
                        if (GameisPaused){
                                Resume();
                        }
                        else{
                                Pause();
                        }
                }
        }

        void Pause(){
                pauseMenuUI.SetActive(true);
                Time.timeScale = 0f;
                GameisPaused = true;
        }

        public void Resume(){
                pauseMenuUI.SetActive(false);
                Time.timeScale = 1f;
                GameisPaused = false;
        }

        public void Restart(){
                Time.timeScale = 1f;
                //restart the game:
                SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame(){
                SceneManager.LoadScene("Menu");
        }
} 