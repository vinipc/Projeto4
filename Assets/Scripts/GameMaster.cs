using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ActivitiesProgression { Collecting, Stepping, Bottling }

public class GameMaster : Singleton<GameMaster> 
{
	// Pra usar o node: star essas duas variáveis pra False.
	// Se não for usar o node, pelo menos uma delas tem q ser true @Heitor
	private bool USE_SPREADSHEET = false; 
	private bool USE_PHP = true;

	#region Twitter vars
	string CONSUMER_KEY = "PfF09E6UP2D8dgWZyMAReCjpt";
	string CONSUMER_SECRET = "MToeBjBAfQum3153YKuyu5xZM9vetbHzkfCyRlbPqBIrn5KbNr";

	string VINICOLA_USER_TOKEN_SECRET = "6JOiinuqz933ArRTs2gs9Q2INOF2nFpRItyB5puvoCLXj";
	string VINICOLA_USER_TOKEN = "933838789381967873-kuSLhGPNWnewK0u4n3yvjHyDpSlOVqG";
	private string VINICOLA_USER_ID = "933838789381967873";
	private string VINICOLA_USER_SCREEN_NAME = "VinicoLouca";

	Twitter.RequestTokenResponse m_RequestTokenResponse;
	Twitter.AccessTokenResponse m_AccessTokenResponse;
	#endregion

	#region GoogleSpreadsheet vars
	private const string TABLE_NAME = "Wines";
	private const string ID_FIELD_NAME = "id";
	private const string QUANTITY_FIELD_NAME = "quantity";
	private const string WEBHOST_URL = "http://vinicolouca.000webhostapp.com/";
	private const string HEROKU_URL = " "; // Botar o endereço do server do Heroku @Heitor

	public List<WineInfo> wineInfos = new List<WineInfo>();
	public List<int> wineQuantitiesFromWebhost = new List<int>();
	public List<int> wineQuantitiesFromHeroku = new List<int>();
	#endregion

	public static string MAIN_MENU_NAME = "Main Menu";
	public static bool isCounting = true;
	public static ActivitiesProgression currentProgressionState;

	public Image timerGauge;
	public GameObject gameOverOverlay;
	public Text descriptionDisplay;
	public float maxTime;

	public Color timerFullColor = Color.green; // Timer color when it's full
	public Color timerEmptyColor = Color.red; // Timer color when it's empty

	public Transform activitiesParent;
	public GrapesActivity collectActivity;
	public DanceMatActivity danceMatActivity;
	public MicrophoneActivity bottlingActivity;
	public GameObject pauseMenu;

	private float currentTime;
	private bool isPaused = false;

	private void Awake()
	{
		gameOverOverlay.SetActive(false);
		currentTime = maxTime;
		timerGauge.transform.localScale = Vector3.one;
		isCounting = true;
		currentProgressionState = ActivitiesProgression.Collecting;

		danceMatActivity.gameObject.SetActive(false);
		bottlingActivity.gameObject.SetActive(false);

		CloudConnectorCore.processedResponseCallback.RemoveAllListeners();
		CloudConnectorCore.processedResponseCallback.AddListener(this.ParseData);
		StartCoroutine(LoadWineQuantities());
		LoadTwitterUserInfo();
	}

	private void Update()
	{
		if (isCounting)
		{
			if (currentTime < 0f)
				GameOver();
			else
				DecreaseTimer();
		}

		if (!bottlingActivity.gameObject.activeSelf && ResourcesMaster.GetResourceAmount("juice") >= ResourcesMaster.GetResourceData("bottle").requiredToGeneratedRatio)
		{
			bottlingActivity.gameObject.SetActive(true);
			currentProgressionState = ActivitiesProgression.Bottling;
		}

		if (Input.GetButtonDown("Reset"))
		{
			System.GC.Collect();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		else if (Input.GetButtonDown("Pause"))
		{
			if (isPaused)
				ClosePauseMenu();
			else
				OpenPauseMenu();
		}
	}

	private void OpenPauseMenu()
	{
		isPaused = true;
		isCounting = false;

		pauseMenu.SetActive(true);
	}

	public void ClosePauseMenu()
	{
		Debug.Log("Close pause menu");
		isPaused = false;
		isCounting = true;

		pauseMenu.SetActive(false);
	}

	public void MenuButtonPressed()
	{
		Debug.Log("Load menu");
		SceneManager.LoadScene(MAIN_MENU_NAME);
	}

	private void GameOver()
	{
		isCounting = false;
		gameOverOverlay.SetActive(true);

		int wineIndex = GetWineIndex();

		float percentage;
		StartCoroutine(AddWine(wineIndex));

		if (USE_SPREADSHEET)
			percentage = 100f * ((float) wineInfos[wineIndex].quantity) / (float) GetTotalWinesMade();
		else if (USE_PHP)
			percentage = 100f * ((float) wineQuantitiesFromWebhost[wineIndex] / (float) GetTotalWinesMade());
		else
			percentage = 100f * ((float) wineQuantitiesFromHeroku[wineIndex] / (float) GetTotalWinesMade());

		string percentString = percentage.ToString("##.#");

		string wineDescription = string.Concat(ResourcesMaster.instance.bottlesColors.Count, 
			" garrafas de um ", ResourcesMaster.GetColorDescription(),
			", ", ResourcesMaster.GetDeviationDescription(), 
			" e ", ResourcesMaster.GetQuantityDescription());

		descriptionDisplay.text = string.Concat("Parabéns!\n\nVocês produziram ", wineDescription, ", como ", percentString, "% dos vinhos produzidos");
		string tweet = string.Concat("Acabaram de produzir ", wineDescription, ". ", GetTotalWinesMade().ToString(), " vinhos já foram produzidos!");

		StartCoroutine(Twitter.API.PostTweet(tweet, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, new Twitter.PostTweetCallback(this.OnPostTweet)));
	}

	private void DecreaseTimer()
	{
		currentTime -= Time.deltaTime;
		timerGauge.fillAmount = currentTime / maxTime;
		timerGauge.color = Color.Lerp(timerEmptyColor, timerFullColor, currentTime / maxTime);
	}

	public static void FirstGrapeCollected()
	{
		if (currentProgressionState == ActivitiesProgression.Collecting)
		{
			currentProgressionState = ActivitiesProgression.Stepping;
		}
	}

	private int GetTotalWinesMade()
	{
		if (USE_SPREADSHEET)
		{
			int total = 0;
			for (int i = 0; i < wineInfos.Count; i++)
			{
				total += wineInfos[i].quantity;
			}
			
			return total;			
		}
		else if (USE_PHP)
		{
			return wineQuantitiesFromWebhost.Sum<int>();
		}
		else
		{
			return wineQuantitiesFromHeroku.Sum<int>();
		}
	}

	private int GetWineIndex()
	{
		int quantityIndex = ResourcesMaster.GetQuantityIndex();
		int colorIndex = ResourcesMaster.GetColorIndex();
		int deviationIndex = ResourcesMaster.GetDeviationIndex();

		if (quantityIndex == 0 && colorIndex == 0 && deviationIndex == 0)
			return 0;
		else if (quantityIndex == 0 && colorIndex == 0 && deviationIndex == 1)
			return 1;
		else if (quantityIndex == 0 && colorIndex == 1 && deviationIndex == 0)
			return 2;
		else if (quantityIndex == 0 && colorIndex == 1 && deviationIndex == 1)
			return 3;
		else if (quantityIndex == 0 && colorIndex == 2 && deviationIndex == 0)
			return 4;
		else if (quantityIndex == 0 && colorIndex == 2 && deviationIndex == 1)
			return 5;
		else if (quantityIndex == 1 && colorIndex == 0 && deviationIndex == 0)
			return 6;
		else if (quantityIndex == 1 && colorIndex == 0 && deviationIndex == 1)
			return 7;
		else if (quantityIndex == 1 && colorIndex == 1 && deviationIndex == 0)
			return 8;
		else if (quantityIndex == 1 && colorIndex == 1 && deviationIndex == 1)
			return 9;
		else if (quantityIndex == 1 && colorIndex == 2 && deviationIndex == 0)
			return 10;
		else if (quantityIndex == 1 && colorIndex == 2 && deviationIndex == 1)
			return 11;
		else if (quantityIndex == 2 && colorIndex == 0 && deviationIndex == 0)
			return 12;
		else if (quantityIndex == 2 && colorIndex == 0 && deviationIndex == 1)
			return 13;
		else if (quantityIndex == 2 && colorIndex == 1 && deviationIndex == 0)
			return 14;
		else if (quantityIndex == 2 && colorIndex == 1 && deviationIndex == 1)
			return 15;
		else if (quantityIndex == 2 && colorIndex == 2 && deviationIndex == 0)
			return 16;
		else if (quantityIndex == 2 && colorIndex == 2 && deviationIndex == 1)
			return 17;

		Debug.Log("SHOULD NOT BE HERE!");
		return 17;
	}

	#region Twitter functions
	private void LoadTwitterUserInfo()
	{
		m_AccessTokenResponse = new Twitter.AccessTokenResponse();

		m_AccessTokenResponse.UserId = VINICOLA_USER_ID;
		m_AccessTokenResponse.ScreenName = VINICOLA_USER_SCREEN_NAME;
		m_AccessTokenResponse.Token = VINICOLA_USER_TOKEN;
		m_AccessTokenResponse.TokenSecret = VINICOLA_USER_TOKEN_SECRET;

		if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
		{
			string log = "LoadTwitterUserInfo - succeeded";
			log += "\n    UserId : " + m_AccessTokenResponse.UserId;
			log += "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
			log += "\n    Token : " + m_AccessTokenResponse.Token;
			log += "\n    TokenSecret : " + m_AccessTokenResponse.TokenSecret;
			Debug.Log(log);
		}
	}

	private void OnPostTweet(bool success)
	{
		Debug.Log("OnPostTweet - " + (success ? "succedded." : "failed."));
	}
	#endregion

	#region Internal API functions
	public IEnumerator AddWine(int index)
	{
		// Atualizar spreadsheet:
		WineInfo info = wineInfos.Find(i => i.id == index);
		info.quantity++;
		CloudConnectorCore.UpdateObjects(TABLE_NAME, ID_FIELD_NAME, info.id.ToString(), QUANTITY_FIELD_NAME, info.quantity.ToString());

		// Atualiza server no webhost por php:
		wineQuantitiesFromWebhost[index]++;
		WWW www = new WWW(WEBHOST_URL + "addWine.php?id=" + index.ToString());
		while (!www.isDone)
		{
			yield return 0;			
		}

		// Atualizar server no heroku por node.js: @Heitor
	}

	public IEnumerator LoadWineQuantities()
	{
		CloudConnectorCore.GetTable(TABLE_NAME);

		wineQuantitiesFromWebhost = new List<int>();
		wineQuantitiesFromHeroku = new List<int>();
		for (int i = 0; i < 17; i++)
		{
			WWW www = new WWW(WEBHOST_URL + "getWineQuantity.php?id=" + i.ToString());
			while (!www.isDone)
			{
				yield return 0;				
			}
			wineQuantitiesFromWebhost.Add(int.Parse(www.text));

			// Inserir chamada pro getWineQuantity do node por aqui @Heitor
		}
	}

	// Usado só pelo esquema com Spreadsheet
	public void ParseData(CloudConnectorCore.QueryType query, List<string> objTypeNames, List<string> jsonData)
	{
		if (query == CloudConnectorCore.QueryType.getTable)
		{
			wineInfos.Clear();
			WineInfo[] wines = GSFUJsonHelper.JsonArray<WineInfo>(jsonData[0]);
			for (int i = 0; i < wines.Length; i++)
			{
				wineInfos.Add(wines[i]);
			}
		}
	}
	#endregion
}

[System.Serializable]
public class WineInfo
{
	public int id;
	public int quantity;

	public WineInfo(int id, int quantity)
	{
		this.id = id;
		this.quantity = quantity;
	}
}

// Helper class: because UnityEngine.JsonUtility does not support deserializing an array...
// http://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/
public class GSFUJsonHelper
{
	public static T[] JsonArray<T>(string json)
	{
		string newJson = "{ \"array\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
		return wrapper.array;
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] array = new T[] { };
	}
}