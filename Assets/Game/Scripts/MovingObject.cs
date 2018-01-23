using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
	public LayerMask blockingLayer;

	public float moveTime = 0.1f;
	private Rigidbody2D rb2D;
	protected BoxCollider2D boxCollider;
	private float inverseMoveTime;

	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		float sqrRemainigDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainigDistance > float.Epsilon) { 
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position,
				                      end,
				                      inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainigDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	// out is a reference
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		// Implicit conversion discard Z data
		Vector2 start = transform.position;

		Vector2 end = start + new Vector2 (xDir, yDir);
		// Disable our collider to avoid collision with ourself
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);

		boxCollider.enabled = true;

		if (hit.transform == null) { 
			StartCoroutine (SmoothMovement (end));
			return true;
		}
		return false;
	}
	// Parameter T is the expected gameObject with our unit could interact
	// in case of Player -> walls
	// in case of Enemy -> player
	protected virtual void AttemptMove<T> (int xDir, int yDir) 
		where T: Component
	{

		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform == null) {
			return;
		}
		T hitComponent = hit.transform.GetComponent<T> ();
		if (!canMove && hitComponent != null) {
			// Hit some blocker
			OnCantMove (hitComponent);
		}
	}


	protected abstract void OnCantMove <T> (T component) 
		where T : Component;
	
}