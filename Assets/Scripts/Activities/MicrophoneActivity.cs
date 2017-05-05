using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneActivity : Activity
{
	private readonly int SAMPLE_COUNT = 1024; 
	public readonly float MIC_SENSITIVITY = 100f; // Multiplies volume into more intelligible values

	[Header("Read only:")]
	public float volume; // Current volume
	public float accumulatedVolume = 0f; // Current accumulated volume

	[Header("Activity config:")]
	public float threshold = 100f; // How much accumulated volume there must be to generate resource
	public float ambientVolume = 0.6f; // Ambient volume to adjust input volume
	public int generatedAmount; // How much resource is generated whenever threshold is beaten

	private float[] _samples;
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Start() 
	{
		// Intitialize the audio buffer
		_samples = new float[SAMPLE_COUNT];

		// Hook event for device change handling
		AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;

		// Setup the microphone input stream
		SetupMicrophoneInput();
	}

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();
		}

		if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			threshold -= 5f;
			threshold = Mathf.Max(1f, threshold);
		}

		if (Input.GetKeyDown(KeyCode.KeypadMinus))
		{
			threshold += 5f;
		}
	}

	private void CheckInput()
	{
		volume = GetAverageVolume() * MIC_SENSITIVITY;

		// Factors in ambient volume without letting it go into negative
		float adjustedVolume = volume - ambientVolume; 
		adjustedVolume = Mathf.Max(0f, adjustedVolume);

		accumulatedVolume += adjustedVolume;

		if (accumulatedVolume >= threshold)
		{
			int timesThresholdBeaten = (int) (accumulatedVolume / threshold);
			accumulatedVolume -= timesThresholdBeaten * threshold;

			GenerateResource(generatedAmount * timesThresholdBeaten);
		}
	}

	private float GetAverageVolume()
	{
		float average = 0.0f;
		audioSource.GetOutputData(_samples, 0);

		for (int i = 0; i < _samples.Length; i++)
		{
			average += Mathf.Abs(_samples[i]);
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
			Debug.Log("Primary recording device: " + primaryAudioRecordingDevice);
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

	private void OnAudioConfigurationChanged(bool deviceWasChanged)
	{ 
		if (deviceWasChanged) 
		{
			audioSource.Stop();
			SetupMicrophoneInput();
		}
	}
}
