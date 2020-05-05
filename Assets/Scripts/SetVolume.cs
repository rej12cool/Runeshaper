using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
public class SetVolume : MonoBehaviour{

 public AudioMixer mixer;

 public void SetLevel (float sliderValue){
    mixer.SetFloat("MusicVolume", Mathf.Log10 (sliderValue) * 20);
 }
}