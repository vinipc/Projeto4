using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivity : Activity
{
	public int generatedAmount = 1;
	public string generatedResourceName;

	public void OnButtonPressed()
	{
		if (GameMaster.isCounting)
		{
			GenerateResource(generatedAmount);
			ResourcesMaster.AddResource(generatedResourceName, generatedAmount);
		}
	}
}