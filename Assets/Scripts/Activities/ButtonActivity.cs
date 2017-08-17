using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivity : MonoBehaviour
{
	public int generatedAmount = 1;
	public string generatedResourceName;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			OnButtonPressed();
	}

	public void OnButtonPressed()
	{
		if (GameMaster.isCounting)
		{
			ResourcesMaster.AddResource(generatedResourceName, generatedAmount);
		}
	}
}