using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrapesActivity : Activity
{
	public GrapesBunch bunchPrefab;
	public Vector2 bunchSpawnInterval;
	public Vector2 bunchSpawnAreaSize;
	public Transform bunchSpawnCenter;
	public float bunchesLifetime = 1f;

	public Grape grapePrefab;
	public Vector2 grapeSpawnArea;
	public Transform grapeSpawnCenter;

	public Rigidbody2D paddleRigidbody;
	public float paddleTorque = 10f;

	private Countdown bunchSpawnCountdown;
	private List<GrapesBunch> growingBunches = new List<GrapesBunch>();
	private List<Grape> collectedGrapes = new List<Grape>();

	protected override void Awake()
	{
		base.Awake();
		SpawnBunch();
	}

	public void Update()
	{
		UpdateBunches();

		if (Input.GetKeyDown(KeyCode.Q))
			OnShakeDetected();

		if (Input.GetKeyDown(KeyCode.LeftArrow))
			paddleRigidbody.AddTorque(paddleTorque);

		if (Input.GetKeyDown(KeyCode.RightArrow))
			paddleRigidbody.AddTorque(-paddleTorque);
	}

	public void OnShakeDetected()
	{		
		if (GameMaster.isCounting)
		{
			CollectBunches();
		}
	}

	public void AddGrapes()
	{
		ResourcesMaster.AddResource(generatedResourceName, ResourcesMaster.instance.resourcePerButtonTap);
	}

	public Grape GetCollectedGrape()
	{
		if (collectedGrapes.Count == 0)
			return null;
		else
			return collectedGrapes.GetRandom();
	}

	public void RemoveGrapeWithValue(float colorValue)
	{
		Grape grape = collectedGrapes.Find(g => g.colorSpectrumValue == colorValue);
		collectedGrapes.Remove(grape);
		if (grape != null)
			Destroy(grape.gameObject);
	}

	private void SpawnBunch()
	{
		Vector2 position = new Vector2();
		position.x = Random.Range(bunchSpawnCenter.position.x - bunchSpawnAreaSize.x / 2f, bunchSpawnCenter.position.x + bunchSpawnAreaSize.x / 2f);
		position.y = Random.Range(bunchSpawnCenter.position.y - bunchSpawnAreaSize.y / 2f, bunchSpawnCenter.position.y + bunchSpawnAreaSize.y / 2f);

		GrapesBunch newBunch = Instantiate<GrapesBunch>(bunchPrefab, position, bunchPrefab.transform.rotation, bunchSpawnCenter);
		growingBunches.Add(newBunch);

		bunchSpawnCountdown = Countdown.New(Random.Range(bunchSpawnInterval.x, bunchSpawnInterval.y), SpawnBunch);
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
				collectedBunches.Add(growingBunches[i]);
				Rigidbody2D bunchRb = growingBunches[i].GetComponent<Rigidbody2D>();
				bunchRb.simulated = true;
				bunchRb.AddForce(new Vector2(Random.Range(0f, 1f), Random.Range(0, 5f)), ForceMode2D.Impulse);
				Destroy(bunchRb.gameObject, 1f);
			}

			for (int j = 0; j < ResourcesMaster.instance.grapesPerBunch; j++)
			{
				Vector2 position = new Vector2();
				position.x = Random.Range(grapeSpawnCenter.position.x - grapeSpawnArea.x / 2f, grapeSpawnCenter.position.x + grapeSpawnArea.x / 2f);
				position.y = Random.Range(grapeSpawnCenter.position.y - grapeSpawnArea.y / 2f, grapeSpawnCenter.position.y + grapeSpawnArea.y / 2f);

				Grape newGrape = Instantiate<Grape>(grapePrefab, position, grapePrefab.transform.rotation, grapeSpawnCenter);
				newGrape.SetColor(growingBunches[i].lifetime);
				collectedGrapes.Add(newGrape);
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
		Gizmos.DrawWireCube(bunchSpawnCenter.position, bunchSpawnAreaSize);
	}
}