using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InvertedSphereBehavior : MonoBehaviour {

	// Both
	public string sensationName;
	public int sensationFramesSize;
	public string imagesFormat = ".jpg";
	public string audioFormat = ".mp3";
	public int frameRate = 25;

	private string userSensationsPath;
	private int currentFrameNumber = 1;
	private Texture2D currentFrame = null;
	private Texture2D sphereEntertainmentFirstImage = null; 
	private AudioSource currentAudioSource = null;
	private bool playTouched = false;
	private bool regularSliderUpdate = true;
	private Slider playerSlider;
	private Slider volumeSlider;
	private Button pauseButton;
	private Button playButton;
	private Button stopButton;
	private Button exitButton;
	private Button volumeButton;
	private Button wideOrSideBySideButton;
	private bool previousFrameLoaded = true;

	// camera movement parameters
	private Camera leftEye;
	private Camera rightEye;
	private bool sensationPlayerActivated = false;
	private bool sideBySideOn = true;

	void Start() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		transform.localScale = new Vector3 (-1, 1, 1);
		Input.gyro.enabled = true;

		leftEye = GameObject.Find("LeftEye").GetComponent<Camera>();
		rightEye = GameObject.Find("RightEye").GetComponent<Camera>();

		playerSlider = GameObject.Find("PlayerSlider").GetComponent<Slider>();
		volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
		pauseButton = GameObject.Find ("Pause").GetComponent<Button>();
		playButton = GameObject.Find ("Play").GetComponent<Button>();
		stopButton = GameObject.Find ("Stop").GetComponent<Button>();
		exitButton = GameObject.Find ("Exit").GetComponent<Button>();
		volumeButton = GameObject.Find ("Volume").GetComponent<Button>();
		wideOrSideBySideButton = GameObject.Find ("WideOrSideBySide").GetComponent<Button>();
		currentAudioSource = GetComponent<AudioSource>();

		pauseButton.gameObject.SetActive (false);
		playerSlider.minValue = 1;
		playerSlider.value = 1;

		playerSlider.onValueChanged.AddListener(sliderValueChange);
		playButton.onClick.AddListener (playButtonTouched);
		pauseButton.onClick.AddListener (pauseButtonTouched);
		stopButton.onClick.AddListener (stopButtonTouched);
		exitButton.onClick.AddListener (exitButtonTouched);
		wideOrSideBySideButton.onClick.AddListener (wideOrSideBySideButtonTouched);
		volumeSlider.gameObject.SetActive(false);
		//startSensationPlayer ("true,192.168.1.105,3000,Bernabeu1280,2005");
	}
	
	// Update is called once per frame
	void Update () {
		GL.Clear(false, true, Color.clear);
		if (sensationPlayerActivated) {
			// gyro update
			Quaternion gyroAttitude = Input.gyro.attitude;
			gyroAttitude.x *= -1;
			gyroAttitude.y *= -1;

			leftEye.transform.rotation = gyroAttitude;
			rightEye.transform.rotation = gyroAttitude;

			if (playTouched && sensationFramesSize != 1 && currentFrameNumber <= sensationFramesSize && previousFrameLoaded) {
				StartCoroutine (loadVideoBasedSensationCurrentFrame ());
			} else if (currentFrameNumber > sensationFramesSize) {
				stopButtonTouched ();
			}
		}
	}

	void OnGUI(){
		if (sensationPlayerActivated) {
			regularSliderUpdate = true;
			playerSlider.value = currentFrameNumber;
			regularSliderUpdate = false;
		}
	}
	
	IEnumerator loadImageBasedSensationCurrentFrame() {
		WWW www = new WWW (userSensationsPath + sensationName + "/" + sensationName + imagesFormat);
		yield return www;
		Renderer renderer = GetComponent<Renderer> ();
		if (null != currentFrame) {
			Object.DestroyImmediate (currentFrame);
		}
		currentFrame = www.texture;
		renderer.material.mainTexture = currentFrame;
	}

	IEnumerator loadSphereEntertainmentFirstImage() {
		if (null == sphereEntertainmentFirstImage) {
			WWW www = new WWW (userSensationsPath + "sphereEntertainment.jpg");
			yield return www;
			sphereEntertainmentFirstImage = www.texture;
		} else {
			yield return new WaitForSeconds(0.1f);
		}
		Renderer renderer = GetComponent<Renderer> ();
		renderer.material.mainTexture = sphereEntertainmentFirstImage;
		currentAudioSource.Stop ();
	}
	
	IEnumerator loadVideoBasedSensationCurrentFrame() {
		currentFrameNumber = (int) (currentAudioSource.time * frameRate);
		if (currentFrameNumber > 0) {
			WWW www = new WWW (userSensationsPath + sensationName + "/" + sensationName +  currentFrameNumber + imagesFormat);
			previousFrameLoaded = false;
			yield return www;
			previousFrameLoaded = true;
			Renderer renderer = GetComponent<Renderer> ();
			if (null != currentFrame) {
				Object.DestroyImmediate (currentFrame);
			}
			currentFrame = www.texture;
			renderer.material.mainTexture = currentFrame;
		}
	}
	
	IEnumerator loadSensationCurrentAudioClip() {
		WWW www = new WWW(userSensationsPath + sensationName + "/" + sensationName + "SoundTrack" + audioFormat);
		yield return www;
		AudioClip currentAudioClip = www.audioClip;
		currentAudioSource.clip = currentAudioClip;
	}

	void sliderValueChange(float value) {
		if (sensationPlayerActivated) {
			if (!regularSliderUpdate) {
				currentAudioSource.time = value / frameRate;
				playButtonTouched ();
			}
		}
	}

	void playButtonTouched() {
		if (sensationPlayerActivated) {
			pauseButton.gameObject.SetActive (true);
			playButton.gameObject.SetActive (false);
			currentAudioSource.Play ();
			playTouched = true;
		}
	}

	void stopButtonTouched() {
		if (sensationPlayerActivated) {
			currentFrameNumber = 1;
			playerSlider.value = 1;
			pauseButtonTouched ();
			StartCoroutine (loadSphereEntertainmentFirstImage ());
		}
	}

	void pauseButtonTouched() {	
		if (sensationPlayerActivated) {
			pauseButton.gameObject.SetActive (false);
			playButton.gameObject.SetActive (true);
			currentAudioSource.Pause ();
			playTouched = false;
		}
	}

	void exitButtonTouched() {
		Application.Quit();
	}

	void wideOrSideBySideButtonTouched() {
		if (sideBySideOn) {
			rightEye.rect = new Rect(0, 0, 1, 1);
			leftEye.gameObject.SetActive(false);
			sideBySideOn = false;
		} else {
			rightEye.rect = new Rect(0, 0, 0.5f, 1);
			leftEye.gameObject.SetActive(true);
			sideBySideOn = true;
		}
	}

	public void startSensationPlayer (string sensationPlayerStartingString) {
		string[] sensationPlayerStartingStringIndividualParameters = sensationPlayerStartingString.Split (',');

		if ("true" == sensationPlayerStartingStringIndividualParameters[0]) {
			this.userSensationsPath = "http://" + sensationPlayerStartingStringIndividualParameters [1] + 
				":" + sensationPlayerStartingStringIndividualParameters [2] + 
					"/UserSensations/";
			this.sensationName = sensationPlayerStartingStringIndividualParameters[3];
			this.sensationFramesSize = int.Parse(sensationPlayerStartingStringIndividualParameters[4]);
		} else {
			this.userSensationsPath = sensationPlayerStartingStringIndividualParameters[1];
			this.sensationName = sensationPlayerStartingStringIndividualParameters[2];
			this.sensationFramesSize = int.Parse(sensationPlayerStartingStringIndividualParameters[3]);
		}

		playerSlider.maxValue = sensationFramesSize;

		StartCoroutine(loadSensationCurrentAudioClip());

		if (sensationFramesSize == 1) {
			playButton.gameObject.SetActive(false);
			stopButton.gameObject.SetActive(false);
			playerSlider.gameObject.SetActive(false);
			StartCoroutine (loadImageBasedSensationCurrentFrame());
		} else {
			StartCoroutine (loadSphereEntertainmentFirstImage());
		}

		sensationPlayerActivated = true;
	}
}

//file:///mnt/sdcard/UserSensations/
//file://C:/Users/Eduardo/Desktop/UserSensations/