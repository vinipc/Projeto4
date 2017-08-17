using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneActivity : MonoBehaviour
{
	private readonly int SAMPLE_COUNT = 1024; 
	public static float MIC_SENSITIVITY = 100f; // Multiplies volume into more intelligible values
	public static float threshold = 1f; // How much accumulated volume there must be to generate resource
	public static bool isCalibrated = false;
	public static float calibratedAmbientVolume = 0f;
	public static float calibratedClapVolume = 0f;

	[Header("Read only:")]
	public float volume; // Current volume
	public float ambientVolume;
	public float clapVolume;

	[Header("Activity config:")]
	public string generatedResourceName;

	private float[] _samples;
	private AudioSource audioSource;

	private float lastFrameVolume = 0f;
	private bool isAmbientVolume;
	private bool detectedClap;

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

		if (isCalibrated)
		{
			ambientVolume = calibratedAmbientVolume;
			clapVolume = calibratedClapVolume;
		}
	}

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();
		}
	}

	private void CheckInput()
	{
		volume = GetAverageVolume() * MIC_SENSITIVITY;
	
		detectedClap = lastFrameVolume <= ambientVolume && volume >= clapVolume;
		if (detectedClap)
		{
			ResourcesMaster.AddResource(generatedResourceName, ResourcesMaster.instance.resourcePerMicThreshold);
		}

		lastFrameVolume = volume;
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
