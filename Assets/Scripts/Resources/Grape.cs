using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grape : MonoBehaviour
{
	public float colorSpectrumValue;

	public void SetColor(float value)
	{
		colorSpectrumValue = value;
		GetComponent<SpriteRenderer>().color = ResourcesMaster.instance.grapeColorGradient.Evaluate(colorSpectrumValue);
	}
}