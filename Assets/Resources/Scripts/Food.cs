using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
	public enum FoodProperty
	{
		FOOD_GROW	,
		FOOD_SPEED	,
	}


	// Because the food has the Food-layer, it will only interact with the Snake-layer.
	private void OnTriggerEnter2D( Collider2D collision )
	{
		Snake		rSnake		= collision.GetComponent<Snake>();  // This is ok since only the snake can collide with this
		GridManager	rGridSystem = GridManager.Instance; // Save down the reference so we don't have to do GridSystem.Instance for all the ones below

		// Return true if it wins, that way we can return from this function and not be stuck in a while loop
		rSnake.EatFood(); // Eat the food


		Vector3 RandomPos	= Vector3.zero;
		RandomPos.x			= Random.Range( rGridSystem.GridMinX, rGridSystem.GridMaxX );
		RandomPos.y			= Random.Range( rGridSystem.GridMinY, rGridSystem.GridMaxY );

		while ( rSnake.IsSegmentInPosition( RandomPos ) ) // Randomize position until we get one where there isn't a segment in.
		{
			RandomPos.x = Random.Range( rGridSystem.GridMinX, rGridSystem.GridMaxX );
			RandomPos.y = Random.Range( rGridSystem.GridMinY, rGridSystem.GridMaxY );
		}

		transform.position = RandomPos;
	}
}

