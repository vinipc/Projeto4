using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesResourcePool : MonoBehaviour
{
	[Header("Resource config:")]
	public GameObject grapePrefab;
	public Transform spawnPoint;
	public float spawnRadius = 1f;
	public string displayedResource;

	private float displayedAmount = 0;
	private List<GameObject> grapes = new List<GameObject>();

	private void Update()
	{
		float currentAmount = ResourcesMaster.GetResourceAmount(displayedResource);
		if (currentAmount != displayedAmount)
		{
			displayedAmount = currentAmount;
			UpdateGrapesQuantity();
			displayedAmount = currentAmount;
		}
	}

	private void UpdateGrapesQuantity()
	{
		int expectedQuantity = Mathf.FloorToInt(displayedAmount / ResourcesMaster.instance.resourcePerGrape);
		if (grapes.Count > expectedQuantity)
		{
			while (grapes.Count > expectedQuantity)
			{
				Destroy(grapes[0]);
				grapes.RemoveAt(0);
			}
		}
		else
		{
			while (grapes.Count < expectedQuantity)
			{
				Vector3 position = spawnPoint.position;
				position.x += Random.Range(-spawnRadius, spawnRadius);
				position.y += Random.Range(-0.5f, 0.5f);
				grapes.Add(Instantiate<GameObject>(grapePrefab, position, grapePrefab.transform.rotation, spawnPoint));
			}
		}
	}

	public void RemoveResource(int amount)
	{
		// Destroys consumed grapes
		if (grapes.Count > amount)
		{
			for (int i = 0; i < amount; i++)
			{
				Destroy(grapes[0]);
				grapes.RemoveAt(0);
			}			
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(spawnPoint.position, new Vector3(spawnRadius * 2f, 0.5f, 0f));
	}
}
