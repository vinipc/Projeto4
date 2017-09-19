using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesBunch : MonoBehaviour
{
	[Range(0f, 1f)]
	public float lifetime;
	[Range(0f, 1f)]
	public float colorOffsetRange = 0.05f;

	public AnimationCurve sizeCurve;
	private SpriteRenderer[] grapes;
	private Color[] grapeColorOffset;
	private float grapesSize;

	private void Awake()
	{
		grapes = GetComponentsInChildren<SpriteRenderer>();
		grapeColorOffset = new Color[grapes.Length];
		grapesSize = grapes[0].transform.localScale.x;

		for (int i = 0; i < grapeColorOffset.Length; i++)
		{
			grapeColorOffset[i] = new Color(Random.Range(0f, colorOffsetRange), Random.Range(0f, colorOffsetRange), Random.Range(0f, colorOffsetRange), 0f);
		}

		Update();
	}

	private void Update()
	{
		SetColors(lifetime);
		SetSizes(lifetime);
	}

	private void SetColors(float percentage)
	{
		for (int i = 0; i < grapes.Length; i++)
		{
			Color newColor = ResourcesMaster.instance.grapeColorGradient.Evaluate(percentage);
			newColor += grapeColorOffset[i];
			grapes[i].color = newColor;
		}
	}

	private void SetSizes(float percentage)
	{
		for (int i = 0; i < grapes.Length; i++)
		{
			grapes[i].transform.localScale = Vector3.one * grapesSize * sizeCurve.Evaluate(percentage);
		}
	}
}
