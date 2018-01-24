using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasResizeListener : MonoBehaviour {
	
	public Camera m_OrthographicCamera;

	void OnRectTransformDimensionsChange( ) {
		Vector2 screenSize = this.GetComponent<RectTransform> ().rect.size ;
		Debug.Log ("onCanvas :" + screenSize);
		adjustCamera (screenSize.x / screenSize.y);
	}

	void adjustCamera(float ratio) {
		if (ratio > 1.0f) {
			m_OrthographicCamera.orthographicSize = 5;
			Debug.Log ("Ratio: " + ratio);
		} else {
			m_OrthographicCamera.orthographicSize = 9;
			Debug.Log ("Ratio: " + ratio);		
		}
	}
}
