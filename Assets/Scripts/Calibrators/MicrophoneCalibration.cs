using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneCalibration : MonoBehaviour
{
	private readonly int SAMPLE_COUNT = 1024; 
	public readonly float MIC_SENSITIVITY = 100f; // Multiplies volume into more intelligible values
	public static float threshold = 100f; // How much accumulated volume there must be to generate resource

	[Header("Read only:")]
	public float volume; // Current volume
	public float ambientVolume = 0.6f; // Ambient volume to adjust input volume
	public int numberOfDisplaySamples = 128;
	public RectTransform sampleDisplayPrefab;
	public float sampleInterval;
	public float sampleScaler = 1f;

	private float countdown;
	private RectTransform[] samplesDisplay;
	private float[] volumeHistory;
	private float[] _samples;
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		samplesDisplay = new RectTransform[numberOfDisplaySamples];
		volumeHistory = new float[numberOfDisplaySamples];
		for (int i = 0; i < numberOfDisplaySamples; i++)
		{
			samplesDisplay[i] = Instantiate<RectTransform>(sampleDisplayPrefab, transform);
		}
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
		countdown -= Time.deltaTime;
		if (countdown <= 0f)
		{
			CheckInput();
			countdown = sampleInterval;
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

		for (int i = 0; i < volumeHistory.Length - 1; i++)
		{
			volumeHistory[i] = volumeHistory[i + 1];
			samplesDisplay[i].localScale = new Vector3(1f, volumeHistory[i] * sampleScaler, 1f);
		}

		volumeHistory[volumeHistory.Length - 1] = volume;
		samplesDisplay[samplesDisplay.Length - 1].localScale = new Vector3(1f, volume * sampleScaler, 1f);
		// Factors in ambient volume without letting it go into negative
		float adjustedVolume = volume - ambientVolume; 
		adjustedVolume = Mathf.Max(0f, adjustedVolume);
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
