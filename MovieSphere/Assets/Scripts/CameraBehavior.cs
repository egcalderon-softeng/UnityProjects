using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {
	
	private float xAxisAngle = 0;
	private float yAxisAngle = 0;
	private int initCounter = 0;

	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Input.gyro.enabled = true;
	}
	
	void Update () {
		if (initCounter < 10) {
			xAxisAngle = (Input.gyro.attitude.eulerAngles.y+90)/Mathf.Rad2Deg;
			yAxisAngle = Input.gyro.attitude.eulerAngles.x/Mathf.Rad2Deg;
			initCounter++;
		} else {
			xAxisAngle -= Input.gyro.rotationRateUnbiased.x*Time.deltaTime;
			yAxisAngle -= Input.gyro.rotationRateUnbiased.y*Time.deltaTime;
		}

		transform.eulerAngles = new Vector3(xAxisAngle*Mathf.Rad2Deg, yAxisAngle*Mathf.Rad2Deg, 0);
	}

	void OnGUI(){
		//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), (Texture) Resources.LoadAssetAtPath(frontCameraImagePath, typeof(Texture2D)));
	}
}
