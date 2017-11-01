using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesBunch : MonoBehaviour
{
	public float lifetime { get { return currentLifetime / totalLifetime; } }

	public float currentLifetime;
	public float totalLifetime;

	[Range(0f, 1f)]
	public float colorOffsetRange = 0.05f;

	public AnimationCurve sizeCurve;
	private List<SpriteRenderer> grapes = new List<SpriteRenderer>();
	private Color[] grapeColorOffset;
	private float grapesSize;

	private void Awake()
	{
		grapes.AddRange(GetComponentsInChildren<SpriteRenderer>());
		grapeColorOffset = new Color[grapes.Count];
		grapesSize = grapes[0].transform.localScale.x;

		for (int i = 0; i < grapeColorOffset.Length; i++)
		{
			grapeColorOffset[i] = new Color(Random.Range(0f, colorOffsetRange), Random.Range(0f, colorOffsetRange), Random.Range(0f, colorOffsetRange), 0f);
		}
	}

	public void Initialize(float lifeTime)
	{
		currentLifetime = totalLifetime = lifeTime;
		SetColors(0f);
		SetSizes(0f);
	}

	public void RemoveGrape()
	{
		SpriteRenderer grape = grapes.GetRandom();
		grapes.Remove(grape);
		Destroy(grape.gameObject);

		if (grapes.Count == 0)
			Destroy(gameObject);
	}

	private void Update()
	{
		if (currentLifetime > 0f)
		{
			currentLifetime -= Time.deltaTime;
			currentLifetime = Mathf.Max(0f, currentLifetime);
			
			SetColors(1f - lifetime);
			SetSizes(1f - lifetime);
		}
	}

	public void SetColors(float percentage)
	{
		for (int i = 0; i < grapes.Count; i++)
		{
			Color newColor = ResourcesMaster.instance.grapeColorGradient.Evaluate(percentage);
			newColor += grapeColorOffset[i];
			grapes[i].color = newColor;
		}
	}

	public void SetSizes(float percentage)
	{
		for (int i = 0; i < grapes.Count; i++)
		{
			grapes[i].transform.localScale = Vector3.one * grapesSize * sizeCurve.Evaluate(percentage);
		}
	}
}
