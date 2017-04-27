using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Parent class that interfaces with Activity to keep track and display resources
public class ResourcePool : MonoBehaviour 
{
	public Text amountDisplay;
	public int resourceAmount { get; private set; }

	protected virtual void Awake()
	{
		if (amountDisplay)
		{
			amountDisplay.text = resourceAmount.ToString();			
		}
	}

	public virtual void AddResource(int amount)
	{
		resourceAmount += amount;

		if (amountDisplay)
		{
			amountDisplay.text = resourceAmount.ToString();			
		}
	}

	public virtual void RemoveResource(int amount)
	{
		resourceAmount -= amount;
		resourceAmount = Mathf.Max(0, resourceAmount);

		if (amountDisplay)
		{
			amountDisplay.text = resourceAmount.ToString();
		}
	}
}
