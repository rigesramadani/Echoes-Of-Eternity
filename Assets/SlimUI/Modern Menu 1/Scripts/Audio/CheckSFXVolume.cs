using UnityEngine;

namespace SlimUI.ModernMenu{
	public class CheckSFXVolume : MonoBehaviour {
		public void  Start (){
			GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume");
		}

		public void UpdateVolume (){
			GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume");
		}
	}
}