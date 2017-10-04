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
	private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PopGrapeAudio()
    {
        audioSource.pitch = Random.Range(0.8f, 1.55f);
        audioSource.Play();
    }

	private void Update()
	{
		/*
		float currentAmount = ResourcesMaster.GetResourceAmount(displayedResource);
		if (currentAmount != displayedAmount)
		{
			displayedAmount = currentAmount;
			UpdateGrapesQuantity();
			displayedAmount = currentAmount;
		}
		*/
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
				GameObject newGrape = Instantiate<GameObject>(grapePrefab, position, Quaternion.identity);
				newGrape.transform.SetParent(spawnPoint, true);
				newGrape.transform.localScale *= Random.Range(0.9f, 1.1f);
				grapes.Add(newGrape);
                PopGrapeAudio();
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
