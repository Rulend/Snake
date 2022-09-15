using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
	// Because all walls have the Wall-layer, they will only interact with the Snake-layer.
	private void OnTriggerEnter2D( Collider2D collision )
	{
		collision.GetComponent<Snake>().Die(); // This is ok since only the snake can collide with this
	}
}
