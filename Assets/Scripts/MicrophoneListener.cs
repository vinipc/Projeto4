using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioListener))]
public class MicrophoneListener : MonoBehaviour
{
	/*
	private readonly int SAMPLE_COUNT = 512; 
	private readonly float THRESHOLD = 60.0f;

	private float[] _samples;
	private float _sensitivity;

	public AudioSource audioSource;
	private CriarSonar sonarController;

	private void ThresholdBeaten()
	{
        if(sonarController.IsAllowed)
		sonarController.InitVoice();
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

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		sonarController = GetComponent<CriarSonar>();
	}

	void SetupMicrophoneInput()
	{
		// We're assuming here that the first recording device is the default
		string primaryAudioRecordingDevice = Microphone.devices[0];

		// Checking if we're not wrong
		if (primaryAudioRecordingDevice == string.Empty) 
		{
			Debug.Log("No input devices found");
			return;
		}

		// Do the microphone audio setup
		AudioClip audioClip = Microphone.Start(primaryAudioRecordingDevice, true, 10, 44100);
		audioSource.loop = true;
		audioSource.mute = false;
		audioSource.clip = audioClip;

		// Gambeta
		while (!(Microphone.GetPosition(null) > 0)) { }

		// Reproduces teh audio recorded from the mic
		audioSource.Play();
	}

	void Start() 
	{
		// Intitialize the audio buffer
		_sensitivity = 100.0f;
		_samples = new float[SAMPLE_COUNT];

		// Hook event for device change handling
		AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged; 

		// Setup the microphone input stream
		SetupMicrophoneInput();
	}

	void OnAudioConfigurationChanged(bool deviceWasChanged)
	{ 
		if (deviceWasChanged) 
		{
			audioSource.Stop();
			SetupMicrophoneInput();
		}
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
	*/
}
