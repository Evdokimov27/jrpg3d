using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GestureRecognizer;
using System.Linq;
using System;

public class ExampleGestureHandler : MonoBehaviour {

	public Text textResult;
	public string ID_Draw;

	public Transform referenceRoot;

	GesturePatternDraw[] references;

	void Start () {
		references = referenceRoot.GetComponentsInChildren<GesturePatternDraw> ();
	}

	void ShowAll(){
		for (int i = 0; i < references.Length; i++) {
			references [i].gameObject.SetActive (true);
		}
	}

	public void OnRecognize(RecognitionResult result){
		StopAllCoroutines ();
		ShowAll ();
		if (result != RecognitionResult.Empty) {
			textResult.text = result.gesture.id + "\n" + Mathf.RoundToInt (result.score.score * 100) + "%";
			ID_Draw = result.gesture.id;
			StartCoroutine (Blink (result.gesture.id));
			StartCoroutine(WaitForAccept());

		} else {
			textResult.text = "?";
			StartCoroutine(WaitForLose());	
		}
		

	}

	IEnumerator WaitForAccept()
	{
		yield return new WaitForSeconds(3);
		GetComponent<DrawDetector>().ClearLines();
	}
	IEnumerator WaitForLose()
	{
		yield return new WaitForSeconds(3);
		GetComponent<DrawDetector>().ClearLines();
	}

	internal void OnRecognize(object result)
    {
        throw new NotImplementedException();
    }

    IEnumerator Blink(string id){
		var draw = references.Where (e => e.pattern.id == id).FirstOrDefault ();
		if (draw != null) {
			var seconds = new WaitForSeconds (0.1f);
			for (int i = 0; i <= 20; i++) {
				draw.gameObject.SetActive (i % 2 == 0);
				yield return seconds;
			}
			draw.gameObject.SetActive (true);
		}
	}

}
