using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneCalibration : MonoBehaviour
{
	private readonly int SAMPLE_COUNT = 1024; 
	public readonly float AMBIENT_MEASURE_DURATION = 1f;

	[Header("Read only:")]
	public float volume; // Current volume

	[Header("Calibration config:")]
	public int numberOfDisplaySamples = 128;
	public RectTransform sampleDisplayPrefab;
	public float sampleInterval;
	public float sampleScaler = 1f;
	public Text messageDisplay;

	private RectTransform[] samplesDisplay;
	private float[] volumeHistory;
	private float[] _samples;
	private AudioSource audioSource;

	private float accumulatedVolume = 0f;
	private int samplesInAccumulatedVolume = 0;
	private float accumulatedVolumeCountdown;

	private float maxVolume = Mathf.NegativeInfinity;
	private float ambientVolume = Mathf.NegativeInfinity;
	private float clapVolume = 0f;

	private Countdown sampleCountdown, ambientSampleCountdown;

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

		// Sets up the required countdowns
		sampleCountdown = Countdown.New(sampleInterval, CheckInput, null, true);
		ambientSampleCountdown = Countdown.New(AMBIENT_MEASURE_DURATION, CalculatedAmbientVolume, UpdateCountdownDisplay, false, "Ambient sample countdown");

		messageDisplay.text = "Por favor, fique em silêncio por 5 segundos para detecção do volume do ambiente.";
	}

	private void Update()
	{
		if (Input.anyKeyDown && maxVolume > 0f)
		{
			clapVolume = maxVolume * 0.75f;
			ambientVolume += (clapVolume - ambientVolume) * 0.4f;
			MicrophoneActivity.calibratedClapVolume = clapVolume;
			MicrophoneActivity.calibratedAmbientVolume = ambientVolume;
			MicrophoneActivity.isCalibrated = true;
			Debug.Log("Ambient volume: " + ambientVolume + "\nMax volume: " + maxVolume + "\nClap volume: " + clapVolume);
			UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
		}
	}

	private void CheckInput()
	{
		volume = GetAverageVolume() * MicrophoneActivity.MIC_SENSITIVITY;

		for (int i = 0; i < volumeHistory.Length - 1; i++)
		{
			volumeHistory[i] = volumeHistory[i + 1];
			samplesDisplay[i].localScale = new Vector3(1f, volumeHistory[i] * sampleScaler, 1f);
		}

		volumeHistory[volumeHistory.Length - 1] = volume;
		samplesDisplay[samplesDisplay.Length - 1].localScale = new Vector3(1f, volume * sampleScaler, 1f);

		accumulatedVolume += volume;
		samplesInAccumulatedVolume++;

		if (volume > maxVolume)
			maxVolume = volume;
	}

	private void UpdateCountdownDisplay()
	{
		string timeLeft = Mathf.CeilToInt(ambientSampleCountdown.time).ToString("0");
		messageDisplay.text = string.Concat("Por favor, fique em silêncio por ", timeLeft, " para detecção do volume do ambiente");
	}

	private void CalculatedAmbientVolume()
	{		
		ambientVolume = accumulatedVolume / samplesInAccumulatedVolume;
		messageDisplay.text = "Agora bata palma.";
		Countdown.New(2f, ()=> messageDisplay.text = "Agora bata palma.\nE então aperte qualquer botão para terminar.");
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
