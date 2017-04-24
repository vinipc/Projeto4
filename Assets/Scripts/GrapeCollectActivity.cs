using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeCollectActivity : Activity
{
	private readonly int SAMPLE_COUNT = 1000; 
	private readonly float THRESHOLD = 5.7f;

	//public GameObject grapePrefab, spawnAreaLeft, spawnAreaRight;

	private float[] _samples;
	private float _sensitivity;

	private AudioSource audioSource;
	//private CriarSonar sonarController;

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		//	sonarController = GetComponent<CriarSonar>();
	}

	void Start() 
	{
		// Intitialize the audio buffer
		_sensitivity = 10.0f;
		_samples = new float[SAMPLE_COUNT];

		// Hook event for device change handling
		AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;

		// Setup the microphone input stream
		SetupMicrophoneInput();
	}

	void Update()
	{
		float volume = GetAverageVolume() * _sensitivity;
		//print(volume);
		if (volume > THRESHOLD)
		{
			ThresholdBeaten();
		}
	}

	private void ThresholdBeaten()
	{
		/*  if(sonarController.IsAllowed)
			sonarController.InitVoice();
		*/
		if (GameMaster.isCounting == true)
		{
			generatedResource.AddResource(1);
			//Instantiate (grapePrefab, new Vector3 (Random.Range (spawnAreaLeft.transform.position.x, spawnAreaRight.transform.position.x), 6.08f, 0), Quaternion.identity);
		}
	}

	private float GetAverageVolume()
	{
		float average = 0.0f;
		audioSource.GetOutputData (_samples, 0);

		foreach(var sample in _samples) 
		{
			average += Mathf.Abs(sample);
		}

		return average / SAMPLE_COUNT;
	}

	private void SetupMicrophoneInput()
	{
		// We're assuming here that the first recording device is the default
		string primaryAudioRecordingDevice = Microphone.devices[0];

		// Checking if we're not wrong
		if (primaryAudioRecordingDevice == string.Empty) 
		{
			Debug.Log("No input devices found");
			return;
		}
		else
		{
			Debug.Log("Primarey recording device: " + primaryAudioRecordingDevice);
		}

		// Do the microphone audio setup
		AudioClip audioClip = Microphone.Start(primaryAudioRecordingDevice, true, 10, 44100);
		audioSource.loop = true;
		audioSource.mute =  false;
		audioSource.clip = audioClip;

		// Gambeta
		while (!(Microphone.GetPosition(null) > 0)) { }

		// Reproduces teh audio recorded from the mic
		audioSource.Play();
	}

	void OnAudioConfigurationChanged(bool deviceWasChanged)
	{ 
		if (deviceWasChanged) 
		{
			audioSource.Stop();
			SetupMicrophoneInput();
		}
	}
}
