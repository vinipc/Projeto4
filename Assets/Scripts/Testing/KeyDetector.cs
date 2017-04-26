using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyDetector : MonoBehaviour 
{
	public Text keyDebugger;

	private void Awake()
	{
		keyDebugger.text = "";
	}

	private void Update()
	{
		foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
		{
			if(Input.GetKeyDown(vKey))
			{
				keyDebugger.text = string.Concat(keyDebugger.text, "\n", vKey.ToString());
			}
		}
	}
}
