using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePool : MonoBehaviour 
{
	public Text amountDisplay;

	public int resourceAmount { get; private set; }

	protected virtual void Awake()
	{
		amountDisplay.text = resourceAmount.ToString();
	}

	public virtual void AddResource(int amount)
	{
		resourceAmount += amount;
		amountDisplay.text = resourceAmount.ToString();
	}

	public virtual void RemoveResource(int amount)
	{
		resourceAmount -= amount;
		resourceAmount = Mathf.Max(0, resourceAmount);
		amountDisplay.text = resourceAmount.ToString();
	}
}
