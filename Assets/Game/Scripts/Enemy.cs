using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	public int playerDamage;
	private Animator animator;
	private Transform target;
	private bool skipMove;
	public AudioClip[] enemyAttacks;

	protected override void Start () {
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		GameManager.instance.AddEnemyToList (this);
		base.Start ();
	}
	
	protected override void AttemptMove<T>(int xDir, int yDir) {
		if (skipMove) {
			skipMove = false;
			return;
		}
		base.AttemptMove <T> (xDir, yDir);
		skipMove = true;
	}
	List<Vector2> movements = new List<Vector2> {
		new Vector2(0,-1),
		new Vector2(0, 1),
		new Vector2(-1, 0),
		new Vector2( 1, 0)
	};

	private class DirComparer: Comparer<Vector2>  {
		float weightX;
		float weightY;
		public DirComparer(float weightX, float weightY) {
			this.weightX = weightX;
			this.weightY = weightY;
		}
		public override int Compare(Vector2 dir1, Vector2 dir2) {
			float weight1 = dir1.x * weightX + dir1.y * weightY;
			float weight2 = dir2.x * weightX + dir2.y * weightY;
			return weight1.CompareTo (weight2);
		}
	};

	public void MoveEnemy() {
		float weightYDir = target.position.y - transform.position.y;
		float weightXDir = target.position.x - transform.position.x;

		movements.Sort (new DirComparer (weightXDir, weightYDir));
		foreach (Vector2 dir in movements) {
			Debug.Log ("d: " + dir + " w: " + (dir.x * weightXDir + dir.y * weightYDir));
		}

		movements.Reverse ();

		foreach (Vector2 dir in movements) {			
			if (canUnitMoveIn ((int) dir.x,(int) dir.y)) { 
				// Debug.Log (" ==> d: " + dir + " w: " + (dir.x * weightXDir + dir.y * weightYDir));
				AttemptMove<Player> ((int)dir.x, (int) dir.y);
				return;
			}
		}
	}
	protected bool canUnitMoveIn (int xDir, int yDir) { 
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;
		RaycastHit2D hit = Physics2D.Linecast (start, end, blockingLayer);

		boxCollider.enabled = true;
		if (hit.transform == null) { 			
			return true;
		} else if ( hit.transform == target ){
			Debug.Log ("Target");
			return true;
		}
	/*
	else if (hit.transform is Player) {
			return true;
		*/
		return false;
	}
	protected override void OnCantMove<T> (T component)
	{
		Player hitPlayer = component as Player;
		hitPlayer.LoseFood (playerDamage);
		animator.SetTrigger ("enemyAttack");
		SoundManager.singleton.RandomizeSFX (enemyAttacks);
	}

}
