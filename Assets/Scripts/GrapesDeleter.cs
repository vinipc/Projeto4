using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesDeleter : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag("Grape"))
		{
			Destroy(coll.gameObject);
		}
	}
}