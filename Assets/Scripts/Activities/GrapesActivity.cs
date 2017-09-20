using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrapesActivity : Activity
{
	public GrapesBunch bunchPrefab;
	public Vector2 spawnInterval;
	public Vector2 spawnAreaSize;
	public Transform spawnAreaCenter;
	public float bunchesLifetime = 1f;

	private Countdown bunchSpawnCountdown;
	private List<GrapesBunch> growingBunches = new List<GrapesBunch>();

	private void Awake()
	{
		SpawnGrape();
	}

	public void Update()
	{
		UpdateBunches();

		if (Input.GetKeyDown(KeyCode.Q))
			OnButtonPressed();
	}

	public void OnButtonPressed()
	{		
		if (GameMaster.isCounting)
		{
			CollectBunches();
		}
	}

	private void SpawnGrape()
	{
		Vector2 position = new Vector2();
		position.x = Random.Range(spawnAreaCenter.position.x - spawnAreaSize.x / 2f, spawnAreaCenter.position.x + spawnAreaSize.x / 2f);
		position.y = Random.Range(spawnAreaCenter.position.y - spawnAreaSize.y / 2f, spawnAreaCenter.position.y + spawnAreaSize.y / 2f);

		GrapesBunch newBunch = Instantiate<GrapesBunch>(bunchPrefab, position, bunchPrefab.transform.rotation, spawnAreaCenter);
		growingBunches.Add(newBunch);

		bunchSpawnCountdown = Countdown.New(Random.Range(spawnInterval.x, spawnInterval.y), SpawnGrape);
	}

	private void UpdateBunches()
	{
		for (int i = 0; i < growingBunches.Count; i++)
		{
			growingBunches[i].lifetime = Mathf.Min(1f, growingBunches[i].lifetime + Time.deltaTime / bunchesLifetime);
		}
	}

	private void CollectBunches()
	{
		List<GrapesBunch> collectedBunches = new List<GrapesBunch>();
		for (int i = 0; i < growingBunches.Count; i++)
		{
			if (growingBunches[i].lifetime >= 0.15f)
			{
				ResourcesMaster.AddResource(generatedResourceName, ResourcesMaster.instance.resourcePerButtonTap);
				collectedBunches.Add(growingBunches[i]);

				Rigidbody2D bunchRb = growingBunches[i].GetComponent<Rigidbody2D>();
				bunchRb.simulated = true;
				bunchRb.AddForce(new Vector2(Random.Range(0f, 1f), Random.Range(0, 5f)), ForceMode2D.Impulse);
				Countdown.New(1f, ()=> Destroy(bunchRb.gameObject));
			}
		}

		for (int i = 0; i < collectedBunches.Count; i++)
		{
			growingBunches.Remove(collectedBunches[i]);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(spawnAreaCenter.position, spawnAreaSize);
	}
}