using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePool : MonoBehaviour 
{
	public Text amountDisplay;

	public float amount { get; private set; }

	public void ChangeAmount(float amountDelta)
	{
		amount += amountDelta;
		amountDisplay.text = amount.ToString();
	}
}
