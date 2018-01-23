using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	//Assign this Camera in the Inspector
	public Camera m_OrthographicCamera;
	//These are the positions and dimensions of the Camera view in the Game view
	// float m_ViewPositionX, m_ViewPositionY, m_ViewWidth, m_ViewHeight;
	private float ratio = 0;
	public Canvas canvas;

	void Start()
	{
		if (canvas != null) { 
			
		} else {
			Debug.Log("Camera Manager disabled");
		}
		ratio = getCurrentRatio ();
		adjustCamera ();
	}
	void Update() {
		float currentRatio = getCurrentRatio ();
 		if (Mathf.Abs (currentRatio - ratio) > Mathf.Epsilon) {
			ratio = currentRatio;
				adjustCamera ();
		}
	}

	static float getCurrentRatio ()
	{
		Resolution resolution = Screen.currentResolution;
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
			return (float)resolution.width / (float)resolution.height;
		} else if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
			return (float)resolution.height / (float)resolution.width;
		} else {
			Debug.Log ("Unknown Orientation: " + Input.deviceOrientation);
			return -1;
		}
	}

	void adjustCamera() {
		
		if (ratio > 1.4f) {
			m_OrthographicCamera.orthographicSize = 5;
			Debug.Log ("Ratio: " + ratio);
		} else if (ratio <= 1.4f && ratio > 0) {
			m_OrthographicCamera.orthographicSize = 9;
			Debug.Log ("Ratio: " + ratio);
		} else {
			Debug.Log ("Ratio out of specification: " + ratio);
			m_OrthographicCamera.orthographicSize = 5;

		}

	}

}
