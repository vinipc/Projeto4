using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
	public ResourcePool requiredResource;
	public ResourcePool generatedResource;

	[Tooltip("required / generated ratio")]
	public float requirementToGenerationRatio = 1f;

	public virtual void GenerateResource(int generatedAmount)
	{
		int requiredAmount = (int) requirementToGenerationRatio * generatedAmount;

		if (requiredResource == null || requiredAmount <= requiredResource.resourceAmount)
		{
			if (requiredResource != null)
				requiredResource.RemoveResource(requiredAmount);

			generatedResource.AddResource(generatedAmount);
		}
		else
		{
			int maxGenerateable = (int) (requiredResource.resourceAmount / requirementToGenerationRatio);
			requiredResource.RemoveResource(requiredResource.resourceAmount);
			generatedResource.AddResource(maxGenerateable);
		}
	}
}