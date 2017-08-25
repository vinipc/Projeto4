using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
	public string generatedResourceName;

	protected ResourceData generatedResource;
	protected ResourceData requiredResource;

	protected virtual void Awake()
	{
		generatedResource = ResourcesMaster.GetResourceData(generatedResourceName);
		requiredResource = ResourcesMaster.GetResourceData(generatedResource.requiredResourceName);
	}
}