using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxisDetector : MonoBehaviour
{
	public Text axisDebugger;

	private void Update()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		axisDebugger.text = string.Concat("Horizontal: ", horizontal.ToString(), "\nVertical: ", vertical.ToString());
	}
}