using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class PinPad : MonoBehaviour{

	public UnityEvent	EntryAllowed;

	public string 		passcode= "1234";
	private string 		userInput="";

	public int inputLength = 4;

	public GameObject radio;
	public AudioClip correctClue;
	public AudioClip incorrectStatic;

	public AudioClip 	beepSound;
	public AudioClip 	failSound;
	public AudioClip 	successSound;

	public Text display;
	public Image image;

	AudioSource			audioSource;

	bool 				done=false;

	private void Start(){
		userInput="";
		audioSource= GetComponent<AudioSource>();
	}

	public void ButtonClicked(string n){
		audioSource.PlayOneShot(beepSound);
		userInput+= n;
		display.text = userInput;
		AudioSource radioSource = radio.GetComponent<AudioSource>();

		if(done==false){
				if(userInput.Length>=4){
		
					if(userInput==passcode){
						audioSource.PlayOneShot(successSound);
						Debug.Log("Correct passcode");
						image.GetComponent<Image>().color = new Color32(0,255,0,100);
						EntryAllowed.Invoke();
						done= true;
						radioSource.PlayOneShot(correctClue);
		
					
					}else{
						audioSource.PlayOneShot(failSound);
						Debug.Log("Incorrect passcode. Try again");
						image.GetComponent<Image>().color = new Color32(255,0,0,100);
						userInput="";
						radioSource.PlayOneShot(incorrectStatic);

					}
				}
		}else{
			Debug.Log("You already have the correct password, what are you doing??");
			image.GetComponent<Image>().color = new Color32(255,255,255,100);
			display.text="";
			userInput="";
		}
	}
}