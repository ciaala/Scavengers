using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	// Singleton
	public static GameManager instance;

	// Board 
	BoardManager boardScript;
	// UI
	private Text levelText;
	private GameObject levelImage;
	// GamePlay Rules
	public float turnDelay = .1f;
	public float levelStartDelay = 2f;

	public int playerFoodPoints = 100;
	private int level = 1;

	[HideInInspector]
	public bool playersTurn = false;
	private bool enemiesMoving;

	private List<Enemy> enemies;
	private bool doingSetup;

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	private void OnLevelWasLoaded(int index) {
		Debug.Log ("OnLevelWasLoaded");
		level++;
		InitGame ();
	}

	void InitGame() {
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		enemies.Clear ();
		boardScript.SetupScene (level);
		Invoke ("HideLevelImage", levelStartDelay);
		 
	}

	private void HideLevelImage() { 
		levelImage.SetActive (false);
		doingSetup = false;
	}


	public void GameOver(){ 
		levelText.text = "After " + level + " days, you starved."; 
		levelImage.SetActive (true);
		enabled = false;
	}
	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving || doingSetup) { 
			return;
		}
		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy enemy) {
		enemies.Add (enemy);
	}

	IEnumerator MoveEnemies() { 
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}
		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].MoveEnemy ();

			yield return new WaitForSeconds (enemies [i].moveTime);
		}
		playersTurn = true;
		enemiesMoving = false;
	}
}
