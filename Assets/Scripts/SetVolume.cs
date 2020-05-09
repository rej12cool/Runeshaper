using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
public class SetVolume : MonoBehaviour{

 public AudioMixer mixer;
 public string varName;

 public void SetLevel (float sliderValue){
    mixer.SetFloat(varName, Mathf.Log10 (sliderValue) * 20);
 }
}