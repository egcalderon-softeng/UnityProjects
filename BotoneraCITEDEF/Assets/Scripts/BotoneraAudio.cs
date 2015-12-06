using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BotoneraAudio : MonoBehaviour {

	private Image billboard;
	private AudioSource audioSource;
	private AudioClip primoCocoSily;
	private AudioClip whip;
	private AudioClip muelita;
	private AudioClip pasman;
	private AudioClip gigolo;
	private AudioClip preguntasBoludas;
	private AudioClip juanCarlosMessi;
	private AudioClip pareceUnPito;
	private float acumulatedTime = 0;
	public Sprite billboard1;
	public Sprite billboard2;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		audioSource = GetComponent<AudioSource>();
		primoCocoSily = Resources.Load ("Audio/PrimoCocoSily") as AudioClip;
		whip = Resources.Load ("Audio/Whip") as AudioClip;
		muelita = Resources.Load ("Audio/Muelita") as AudioClip;
		pasman = Resources.Load ("Audio/Pasman") as AudioClip;
		gigolo = Resources.Load ("Audio/Gigolo") as AudioClip;
		preguntasBoludas =  Resources.Load ("Audio/PreguntasBoludas") as AudioClip;
		juanCarlosMessi =  Resources.Load ("Audio/JuanCarlosMessi") as AudioClip;
		pareceUnPito =  Resources.Load ("Audio/PareceUnPito") as AudioClip;
		Button cocoSilyButton = GameObject.Find ("PrimoCocoSily").GetComponent<Button>();
		Button whipButton = GameObject.Find ("Whip").GetComponent<Button>();
		Button muelitaButton = GameObject.Find ("Muelita").GetComponent<Button>();
		Button pasmanButton = GameObject.Find ("Pasman").GetComponent<Button>();
		Button gigoloButton = GameObject.Find ("Gigolo").GetComponent<Button>();
		Button preguntasBoludasButton = GameObject.Find ("PreguntasBoludas").GetComponent<Button>();
		Button juanCarlosMessiButton = GameObject.Find ("JuanCarlosMessi").GetComponent<Button>();
		Button pareceUnPitoButton = GameObject.Find ("PareceUnPito").GetComponent<Button>();
		billboard = GameObject.Find ("Billboard").GetComponent<Image>();

		billboard.sprite = billboard1;
		cocoSilyButton.onClick.AddListener(audioPrimoCocoSily);
		whipButton.onClick.AddListener(audioWhip);
		muelitaButton.onClick.AddListener(audioMuelita);
		pasmanButton.onClick.AddListener(audioPasman);
		gigoloButton.onClick.AddListener(audioGigolo);
		juanCarlosMessiButton.onClick.AddListener(audioJuanCarlosMessi);
		pareceUnPitoButton.onClick.AddListener(audioPareceUnPito);
		preguntasBoludasButton.onClick.AddListener(audioPreguntasBoludas);
	}
	
	// Update is called once per frame
	void Update () {
		acumulatedTime += Time.deltaTime;
		if (acumulatedTime > (1.0f/10.0f)) {
			if(billboard.sprite == billboard2) {
				billboard.sprite = billboard1;
			}
			else {
				billboard.sprite = billboard2;
			}
			acumulatedTime = 0;
		} 
	}

	void audioPrimoCocoSily() {
		buttonAudioClip(primoCocoSily);
	}

	void audioWhip() {
		buttonAudioClip(whip);
	}

	void audioMuelita() {
		buttonAudioClip(muelita);
	}

	void audioPasman() {
		buttonAudioClip(pasman);
	}

	void audioGigolo() {
		buttonAudioClip(gigolo);
	}

	void audioPreguntasBoludas() {
		buttonAudioClip(preguntasBoludas);
	}

	void audioPareceUnPito() {
		buttonAudioClip(pareceUnPito);
	}

	void audioJuanCarlosMessi() {
		buttonAudioClip(juanCarlosMessi);
	}

	void buttonAudioClip(AudioClip newAudioClip) {
		if (audioSource.isPlaying) {
			audioSource.Stop();
		} else {
			audioSource.clip = newAudioClip;
			audioSource.Play();
		}
	}
}
