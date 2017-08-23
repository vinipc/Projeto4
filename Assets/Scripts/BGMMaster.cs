using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMMaster : MonoBehaviour
{
	public AudioClip[] availableBGMs;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		audioSource.clip = availableBGMs.GetRandom();
		audioSource.Play();
	}
}
