﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlesResourcePool : MonoBehaviour
{
	[Header("Resource config:")]
	public GameObject bottlePrefab;
	public Transform bottlesParent;

	public AudioClip bottlePopping;
	public AudioSource sfxAudioSource;

	public string displayedResource;
	private float lastDisplayedAmount;

	private List<GameObject> bottles = new List<GameObject>();

	private void Awake()
	{
		sfxAudioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		float currentAmount = ResourcesMaster.GetResourceAmount(displayedResource);
		if (currentAmount != lastDisplayedAmount)
		{
			lastDisplayedAmount = currentAmount;
			CreateNewBottles();
		}
	}

	// Creates new bottles if enough resource was generated
	private void CreateNewBottles()
	{
		float resourcesPerBottle = ResourcesMaster.instance.resourcePerBottle;
		if ((lastDisplayedAmount - resourcesPerBottle * bottles.Count) >= resourcesPerBottle)
		{
			sfxAudioSource.PlayOneShot(bottlePopping);
			bottles.Add(Instantiate<GameObject>(bottlePrefab, bottlesParent));
			float color = FindObjectOfType<DanceMatActivity>().GetAverageColor();
			ResourcesMaster.AddBottle(color);
		}		
	}
}
