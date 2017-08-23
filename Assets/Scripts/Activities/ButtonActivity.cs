using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivity : Activity
{
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			OnButtonPressed();
	}

	public void OnButtonPressed()
	{
		if (GameMaster.isCounting)
		{
			ResourcesMaster.AddResource(generatedResourceName, ResourcesMaster.instance.resourcePerButtonTap);
		}
	}
}