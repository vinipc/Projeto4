using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
	public ResourcePool requiredResource;
	public ResourcePool generatedResource;

	[Tooltip("required / generated ratio")]
	public float requirementToGenerationRatio = 1f;

	public void GenerateResource(float generatedAmount)
	{
		float requiredAmount = requirementToGenerationRatio * generatedAmount;

		if (requiredResource == null || requiredAmount <= requiredResource.amount)
		{
			if (requiredResource != null)
				requiredResource.ChangeAmount(-requiredAmount);

			generatedResource.ChangeAmount(generatedAmount);
		}
		else
		{
			float maxGenerateable = requiredResource.amount / requirementToGenerationRatio;
			requiredResource.ChangeAmount(-requiredResource.amount);
			generatedResource.ChangeAmount(maxGenerateable);
		}
	}
}