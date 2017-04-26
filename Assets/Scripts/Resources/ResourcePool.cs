using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePool : MonoBehaviour 
{
	public Text amountDisplay;

	public float amount { get; private set; }

	public virtual void AddResource(float amountDelta)
	{
		amount += amountDelta;
		amountDisplay.text = amount.ToString();
	}

	public virtual void RemoveResource(float amountDelta)
	{
		amount -= amountDelta;
		amount = Mathf.Max(0f, amount);
		amountDisplay.text = amount.ToString();
	}
}
