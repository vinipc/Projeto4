using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrapesActivity : Activity
{
	public GrapesBunch bunchPrefab;
	public Vector2 bunchSpawnAreaSize;
	public Transform bunchSpawnCenter;
	public float bunchesLifetime = 1f;

	public Grape grapePrefab;
	public Vector2 grapeSpawnArea;
	public Transform grapeSpawnCenter;

	public Rigidbody2D paddleRigidbody;
	public float paddleTorque = 10f;

	private List<Grape> collectedGrapes = new List<Grape>();
	private List<GrapesBunch> growingBunches = new List<GrapesBunch>();
	private List<float> grapesQueue = new List<float>();

	protected override void Awake()
	{
		SpawnBunch();
		Countdown.New(ResourcesMaster.instance.collectedGrapesSpawnInterval, SpawnGrape, false);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			OnShakeDetected();

		if (Input.GetKeyDown(KeyCode.LeftArrow))
			paddleRigidbody.AddTorque(paddleTorque);

		if (Input.GetKeyDown(KeyCode.RightArrow))
			paddleRigidbody.AddTorque(-paddleTorque);
	}

	#region Input detection
	public void OnShakeDetected()
	{		
		if (GameMaster.isCounting)
		{
			CollectBunches();
		}
	}

	public void OnLeftButtonPressed()
	{
		if (GameMaster.isCounting)
			paddleRigidbody.AddTorque(paddleTorque);
	}

	public void OnRightButtonPressed()
	{
		if (GameMaster.isCounting)
			paddleRigidbody.AddTorque(-paddleTorque);
	}
	#endregion

	#region Grapes
	public void CollectGrape(Grape grape)
	{
		collectedGrapes.Add(grape);
		GameMaster.instance.danceMatActivity.AddCollectedGrape(grape);
	}

	private void RemoveGrape(Grape grape)
	{		
		if (grape != null)
		{
			collectedGrapes.Remove(grape);
			Destroy(grape.gameObject);				
		}
	}
	
	public void RemoveGrapeByColor(float colorValue)
	{
		Grape grape = collectedGrapes.Find(g => g.colorSpectrumValue == colorValue);
		RemoveGrape(grape);
	}

	private void SpawnGrape()
	{
		Countdown.New(ResourcesMaster.instance.collectedGrapesSpawnInterval, SpawnGrape, false);

		if (grapesQueue.Count == 0)
			return;
		
		float color = grapesQueue[0];
		grapesQueue.RemoveAt(0);

		Vector2 position = new Vector2();
		position.x = Random.Range(grapeSpawnCenter.position.x - grapeSpawnArea.x / 2f, grapeSpawnCenter.position.x + grapeSpawnArea.x / 2f);
		position.y = Random.Range(grapeSpawnCenter.position.y - grapeSpawnArea.y / 2f, grapeSpawnCenter.position.y + grapeSpawnArea.y / 2f);

		Grape newGrape = Instantiate<Grape>(grapePrefab, position, grapePrefab.transform.rotation, grapeSpawnCenter);
		newGrape.SetColor(color);
	}
	#endregion

	#region Bunches
	private void SpawnBunch()
	{
		Vector2 position = new Vector2();
		position.x = Random.Range(bunchSpawnCenter.position.x - bunchSpawnAreaSize.x / 2f, bunchSpawnCenter.position.x + bunchSpawnAreaSize.x / 2f);
		position.y = Random.Range(bunchSpawnCenter.position.y - bunchSpawnAreaSize.y / 2f, bunchSpawnCenter.position.y + bunchSpawnAreaSize.y / 2f);

		GrapesBunch newBunch = Instantiate<GrapesBunch>(bunchPrefab, position, bunchPrefab.transform.rotation, bunchSpawnCenter);
		growingBunches.Add(newBunch);

		Vector2 lifeTimeRange = ResourcesMaster.instance.bunchLifetimeRange;
		newBunch.Initialize(Random.Range(lifeTimeRange.x, lifeTimeRange.y));

		Vector2 intervalRange = ResourcesMaster.instance.bunchSpawnIntervalRange;
		Countdown.New(Random.Range(intervalRange.x, intervalRange.y), SpawnBunch);
	}

	private void CollectBunches()
	{
		List<GrapesBunch> collectedBunches = new List<GrapesBunch>();
		List<float> collectedGrapesColors = new List<float>() ;
		for (int i = 0; i < growingBunches.Count; i++)
		{
			if (growingBunches[i].lifetime >= 0.15f)
			{				
				collectedBunches.Add(growingBunches[i]);
				Rigidbody2D bunchRb = growingBunches[i].GetComponent<Rigidbody2D>();
				bunchRb.simulated = true;
				bunchRb.AddForce(new Vector2(Random.Range(0f, 1f), Random.Range(0, 5f)), ForceMode2D.Impulse);
				Destroy(bunchRb.gameObject, 1f);

				for (int j = 0; j < ResourcesMaster.instance.grapesPerBunch; j++)
				{
					collectedGrapesColors.Add((1f - growingBunches[i].lifetime) * Random.Range(0.9f, 1.1f));
				}
			}
		}

		for (int i = 0; i < collectedBunches.Count; i++)
		{
			growingBunches.Remove(collectedBunches[i]);
		}

		collectedGrapesColors.Sort();
		grapesQueue.AddRange(collectedGrapesColors);
	}
	#endregion

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(bunchSpawnCenter.position, bunchSpawnAreaSize);
	}
}