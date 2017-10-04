using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesAdder : MonoBehaviour
{
	private GrapesActivity grapesActivity;

	private void Awake()
	{
		grapesActivity = GetComponentInParent<GrapesActivity>();
	}

	public void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.CompareTag("Grape"))
		{
			grapesActivity.AddGrapes();
		}
	}
}
