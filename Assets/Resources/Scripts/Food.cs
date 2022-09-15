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
		Snake		rSnake			= collision.GetComponent<Snake>();  // This is ok since only the snake can collide with this

		// Return true if it wins, that way we can return from this function and not be stuck in a while loop
		rSnake.EatFood(); // Eat the food

		RandomizePosition( rSnake );
	}


	// Send in the snake as a reference in order to check if the randomized position has a snake segment on it
	private void RandomizePosition( Snake _Snake )
	{
		// Create a list of all the positions where there isn't a snake segment. 
		//It's better to randomize the position in only the available spots, rather than randomizing a position in a while loop and then checking if the position is available.
		// The reason for that is because as the snake grows bigger, it will take longer to find a free position if it is entirely randomized, as it could generate the same position more than once.

		GridManager rGridSManager = GridManager.Instance;

		List<Vector2> AvailablePositions = new List<Vector2>();

		for ( int PosY = rGridSManager.GridMaxY; PosY > rGridSManager.GridMinY; --PosY ) // Start from the top
		{
			for ( int PosX = rGridSManager.GridMinX; PosX < rGridSManager.GridMaxX; ++PosX ) // Start from the left
			{
				Vector3 PotentialPos = new Vector3( PosX, PosY, 0.0f );

				if ( !_Snake.IsSegmentInPosition( PotentialPos ) )
					AvailablePositions.Add( PotentialPos );
			}
		}

		Vector3 NewPos = AvailablePositions[ Random.Range( 0, AvailablePositions.Count ) ]; // Max is exclusive in .Range, so no need to do -1

		transform.position = NewPos;
	}
}

