using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InvertedSphere2 : MonoBehaviour {

	// Both
	public int frameBufferSize;
	public string sensationName;
	public int sensationFramesSize;
	private string imagesFormat = ".jpg";
	private string audioFormat = ".mp3";
	private int frameRate = 25;
	
	private string userSensationsPath;
	private int currentFrameNumber = 1;
	private int lastFrameNumber = 0;
	private Texture2D currentFrame = null;
	private AudioSource currentAudioSource = null;
	private bool playTouched = false;
	private bool regularSliderUpdate = true;
	private Slider playerSlider;
	private Button pauseButton;
	private Button playButton;
	private Button stopButton;
	private Button exitButton;
	private bool previousFrameLoaded = true;

	
	// camera movement parameters
	private Camera leftEye;
	private Camera rightEye;
	private bool sensationPlayerActivated = false;

	private int frameProducerBufferPosition = 0;
	private int frameProducerBufferPositionCount = 0;
	private Texture2D[] frameBuffer;

	void Start() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		transform.localScale = new Vector3 (-1, 1, 1);
		Input.gyro.enabled = true;
		
		leftEye = GameObject.Find("LeftEye").GetComponent<Camera>();
		rightEye = GameObject.Find("RightEye").GetComponent<Camera>();
		
		playerSlider = GameObject.Find("PlayerSlider").GetComponent<Slider>();
		pauseButton = GameObject.Find ("Pause").GetComponent<Button>();
		playButton = GameObject.Find ("Play").GetComponent<Button>();
		stopButton = GameObject.Find ("Stop").GetComponent<Button>();
		exitButton = GameObject.Find ("Exit").GetComponent<Button>();
		currentAudioSource = GetComponent<AudioSource>();
		
		pauseButton.gameObject.SetActive (false);
		playerSlider.minValue = 1;
		playerSlider.value = 1;
		
		playerSlider.onValueChanged.AddListener(sliderValueChange);
		playButton.onClick.AddListener (playButtonTouched);
		pauseButton.onClick.AddListener (pauseButtonTouched);
		stopButton.onClick.AddListener (stopButtonTouched);
		exitButton.onClick.AddListener (exitButtonTouched);
		
		startSensationPlayer ("15,192.168.1.106,3000,Neimar,2394");
	}
	
	// Update is called once per frame
	void Update () {
		GL.Clear(false, true, Color.clear);
		if (this.frameProducerBufferPositionCount+this.frameProducerBufferPosition < this.sensationFramesSize && previousFrameLoaded) {
			StartCoroutine (frameProducerMainLoop());
		}
		if (sensationPlayerActivated) {
			// gyro update
			Quaternion gyroAttitude = Input.gyro.attitude;
			gyroAttitude.x *= -1;
			gyroAttitude.y *= -1;
			
			leftEye.transform.rotation = gyroAttitude;
			rightEye.transform.rotation = gyroAttitude;
			
			if (playTouched && sensationFramesSize != 1 && currentFrameNumber <= sensationFramesSize) {
				loadVideoBasedSensationCurrentFrame ();
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
		WWW www = new WWW (userSensationsPath + "sphereEntertainment.jpg");
		yield return www;
		Renderer renderer = GetComponent<Renderer> ();
		if (null != currentFrame) {
			Object.DestroyImmediate (currentFrame);
		}
		currentFrame = www.texture;
		renderer.material.mainTexture = currentFrame;
		currentAudioSource.Stop ();
	}
	
	void loadVideoBasedSensationCurrentFrame() {
		currentFrameNumber = (int) (currentAudioSource.time * frameRate) + 1;
		if (currentFrameNumber >= 0 && currentFrameNumber != lastFrameNumber && (currentFrameNumber < frameProducerBufferPositionCount + frameProducerBufferPosition)) {
			lastFrameNumber = currentFrameNumber;
			Renderer renderer = GetComponent<Renderer> ();
			if (null != currentFrame) {
				Object.DestroyImmediate (currentFrame);
			}
			currentFrame = frameBuffer [currentFrameNumber % frameBufferSize];
			renderer.material.mainTexture = currentFrame;
		} else {
			pauseButtonTouched();
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
	
	public void startSensationPlayer (string sensationPlayerStartingString) {
		string[] sensationPlayerStartingStringIndividualParameters = sensationPlayerStartingString.Split(',');
		this.frameBufferSize = int.Parse (sensationPlayerStartingStringIndividualParameters [0]) * this.frameRate;
		this.frameBuffer = new Texture2D[this.frameBufferSize];
		this.userSensationsPath = "http://" + sensationPlayerStartingStringIndividualParameters[1] + 
			":" + sensationPlayerStartingStringIndividualParameters[2] + 
				"/UserSensations/";
		this.sensationName = sensationPlayerStartingStringIndividualParameters[3];
		this.sensationFramesSize = int.Parse(sensationPlayerStartingStringIndividualParameters[4]);
			
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

	IEnumerator frameProducerMainLoop() {
		if (this.frameProducerBufferPosition < this.frameBufferSize) {
			WWW www = new WWW (userSensationsPath + sensationName + "/" + sensationName +  (frameProducerBufferPositionCount + frameProducerBufferPosition + 1) + imagesFormat);
			previousFrameLoaded = false;
			yield return www;
			previousFrameLoaded = true;
			this.frameBuffer[this.frameProducerBufferPosition] = www.texture;
			this.frameProducerBufferPosition++;
		} else if (playTouched && (frameProducerBufferPositionCount < currentFrameNumber)){
			this.frameProducerBufferPosition = 0;
			this.frameProducerBufferPositionCount += this.frameBufferSize;
			WWW www = new WWW (userSensationsPath + sensationName + "/" + sensationName +  (frameProducerBufferPositionCount + frameProducerBufferPosition + 1) + imagesFormat);
			previousFrameLoaded = false;
			yield return www;
			previousFrameLoaded = true;
			this.frameBuffer[this.frameProducerBufferPosition] = www.texture;
		}
	}
}
