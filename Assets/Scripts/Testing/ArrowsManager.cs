using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

// Detects which buttons are pressed and 
//	lights up the corresponding arrow
public class ArrowsManager : MonoBehaviour 
{
	public Image upLeft, up, upRight, right, downRight, down, downLeft, left;

	private void Update()
	{
		ResetColors();
		
		if (Input.GetKey(KeyCode.JoystickButton0))
			downLeft.color = Color.red;
		
		if (Input.GetKey(KeyCode.JoystickButton1))
			upRight.color = Color.red;
		
		if (Input.GetKey(KeyCode.JoystickButton2))
			upLeft.color = Color.red;
		
		if (Input.GetKey(KeyCode.JoystickButton3))
			downRight.color = Color.red;
		
		if (Input.GetAxis("Horizontal") > 0f)
			right.color = Color.red;
		
		if (Input.GetAxis("Horizontal") < 0f)
			left.color = Color.red;
		
		if (Input.GetAxis("Vertical") > 0f)
			up.color = Color.red;
		
		if (Input.GetAxis("Vertical") < 0f)
			down.color = Color.red;
	}

	private void ResetColors()
	{
		upLeft.color = up.color = upRight.color = right.color = downRight.color = down.color = downLeft.color = left.color = Color.white;
	}
}
