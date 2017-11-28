using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
	public const string MAIN_MENU_NAME = "Main Menu";

	public void OpenMenu()
	{
		SceneManager.LoadScene(MAIN_MENU_NAME);
	}
}