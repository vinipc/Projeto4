using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
	public ResourcePool requiredResource;
	public ResourcePool generatedResource;

	[Tooltip("required / generated ratio")]
	public float requirementToGenerationRatio = 1f;

	public virtual void GenerateResource(float generatedAmount)
	{
		float requiredAmount = requirementToGenerationRatio * generatedAmount;

		if (requiredResource == null || requiredAmount <= requiredResource.amount)
		{
			if (requiredResource != null)
				requiredResource.RemoveResource(requiredAmount);

			generatedResource.AddResource(generatedAmount);
		}
		else
		{
			float maxGenerateable = requiredResource.amount / requirementToGenerationRatio;
			requiredResource.RemoveResource(requiredResource.amount);
			generatedResource.AddResource(maxGenerateable);
		}
	}
}