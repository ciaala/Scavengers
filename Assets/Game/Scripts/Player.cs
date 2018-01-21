using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : MovingObject
{
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;

	public float restartLevelDelay = 1f;
	private Animator animator;
	private int food;
	public Text foodText; 
	private Dictionary<string, int> foodPointsMap = new Dictionary<string, int> {
		{"Soda", 20},
		{"Food", 10}			
	};

	public AudioClip[] moveSounds;
	public AudioClip[] eatSounds;
	public AudioClip[] drinkSounds;
	public AudioClip gameOverSound;
		
	#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

	#else 
	// outside the screen so the touch is skipped
	private Vector2 touchOrigin = -Vector2.one;
	#endif
	// Use this for initialization
	protected override void Start ()
	{
		foodText = GameObject.Find ("FoodText").GetComponent<Text> ();
		animator = GetComponent<Animator> ();
		food = GameManager.singleton.playerFoodPoints;
		foodText.text = "Food: " + food;
		base.Start ();
	}
	private void OnDisable() {
		GameManager.singleton.playerFoodPoints = food;
	}

	// Update is called once per frame
	void Update ()
	{

		if (!GameManager.singleton.playersTurn)
			return;
		
		int horizontal = 0;
		int vertical = 0;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		// avoid diagonal movement
		if (horizontal != 0) {
			vertical = 0;
		}
		#else 
		if (Input.touchCount > 0) {
		Touch myTouch = Input.GetTouch(0);
			if ( myTouch.phase == TouchPhase.Began) {
				touchOrigin = myTouch.position;
			} else if ( myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0 ) { 
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;
				if (Mathf.Abs(x) > Mathf.Abs(y)) {
					horizontal = x > 0 ? 1 : -1;
				} else {
					vertical = y > 0 ? 1 : -1;
				}
			}
		}
		#endif
		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);

		}
	
	}
	protected override void OnCantMove<T> (T component)
	{
		Debug.Log ("CHOP");
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);

		animator.SetTrigger ("playerChop");

	}
	private void Restart() {
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if ( other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if ( foodPointsMap.ContainsKey(other.tag) ) { 
			int genericFoodPoints;
			foodPointsMap.TryGetValue (other.tag, out genericFoodPoints);
			food += genericFoodPoints;
			foodText.text = "+" + genericFoodPoints + " food:" + food;
			other.gameObject.SetActive(false);
			switch ( other.tag ) { 
			case "Soda": 
				SoundManager.singleton.RandomizeSFX (drinkSounds);
				break;
			case "Food":
				SoundManager.singleton.RandomizeSFX (eatSounds);
				break;
			}
		}
	}
			

	protected override void AttemptMove<T> (int xDir, int yDir) { 
		food--;
		foodText.text = "Food: " + food;
		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit; 
		if ( Move(xDir, yDir, out hit)){
			// add audio/sfx
			SoundManager.singleton.RandomizeSFX(moveSounds);
		}
		CheckIfGameOver ();
		GameManager.singleton.playersTurn = false;
	}

	public void LoseFood(int loss) {
		animator.SetTrigger ("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food:" + food; 
		CheckIfGameOver ();
	}
	private void CheckIfGameOver() {
		if (food <= 0) { 
			SoundManager.singleton.RandomizeSFX (gameOverSound);
			SoundManager.singleton.musicSource.Stop ();
			GameManager.singleton.GameOver ();
		}
	}
}

