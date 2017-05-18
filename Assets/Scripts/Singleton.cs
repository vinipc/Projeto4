using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;

	//Returns the instance of this singleton.
	public static T instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<T>();

				if (_instance == null)
				{
					Debug.LogError("An instance of " + typeof(T) + 
						" is needed in the scene, but there is none.");
				}
			}

			return _instance;
		}
	}
}