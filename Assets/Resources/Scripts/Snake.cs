using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoveDirection // I deciced to make this public, so that not only the snake can use this, but also the segments. That way they can use it to rotate their sprites.
{
	MOVEDIR_NONE,
	MOVEDIR_UP,
	MOVEDIR_LEFT,
	MOVEDIR_DOWN,
	MOVEDIR_RIGHT,
}

public class Snake : MonoBehaviour
{
	private Sprite	m_BodySprite;
	private Sprite	m_TailSprite;


	[Tooltip( "How many segments should the player start with?" )]
	[SerializeField]	private int		m_NumStartSegments = 3;

	[SerializeField] private GameObject m_SegmentPrefab;		// Using a prefab is way more convenient than creating a gameobject and attaching all the components via script.

	[Tooltip("Decides how often the snake should be updated")]
	[SerializeField]	private int		m_SnakeFrameRate = 10;
						private float	m_MoveCooldownDuration;
						private float	m_MoveCooldownTimeLeft;

						bool			m_AlreadyMoved;			// Needed to make sure the player can't chain their inputs between move cycles. That would make it so the player could input: Left -> move -> Up -> Right -> Move, which would cause the snake to go through itself. An enum check of the last input isn't enough, so I settled for this.

	private MoveDirection				m_MoveDirection;		 // The direction that the snake is currently moving in
	private Vector2						m_CurrentDirection;

	private List<Segment>				m_Segments; // Here's where all the body parts are stored (sounds morbid lol)

	private int							m_CurrentScore;
	private int							m_HighScore;
	private int							m_MaxPossibleScore;


	private void Awake()
	{
		m_Segments = new List<Segment>(); // Initialize list
	}


	public void GenerateSnake( Sprite _HeadSprite, Sprite _BodySprite, Sprite _TailSprite )
	{
		enabled				= true;								// Snake is disabled when it dies in order to stop movement: here we enable it again
		m_CurrentDirection	= Vector2.up;						// Set default direction to start moving to up
		m_MoveDirection		= MoveDirection.MOVEDIR_UP;         // Used to rotate the head sprite
		m_CurrentScore		= 0;
		m_MaxPossibleScore	= GridManager.Instance.GetMaxNumScore();

		// Set the sprites
		GetComponent<SpriteRenderer>().sprite = _HeadSprite; // This script is on the head 
		m_BodySprite = _BodySprite;
		m_TailSprite = _TailSprite;

		transform.position		= new Vector3( Random.Range( GridManager.Instance.GridMinX, GridManager.Instance.GridMaxX ), 0.0f, 0.0f ); // TODO:: Fix this
		transform.eulerAngles	= Vector3.zero; // Reset rotation


		////////////////////////////////////////////////////////////

		// Delete the previous segments and generate the start-amount of new ones.

		foreach ( Segment CurrentSegment in m_Segments )  
			Destroy( CurrentSegment.gameObject );

		m_Segments.Clear(); // Clear list


		// Create the starting segments and position them downwards. Loop starts at 1 in order to account for the head.
		for ( int SegmentIndex = 1; SegmentIndex < m_NumStartSegments; ++SegmentIndex ) 
		{
			Segment AttachedSegment = Instantiate( m_SegmentPrefab ).GetComponent<Segment>();

			AttachedSegment.SetSprite( m_BodySprite );

			Vector3 NewSegmentPos = transform.position + new Vector3( 0.0f, -SegmentIndex, 0.0f );
			AttachedSegment.transform.position = NewSegmentPos;

			m_Segments.Add( AttachedSegment );
		}

		m_Segments[ m_Segments.Count - 1 ].SetSprite( m_TailSprite ); // Change the last sprite to the tail sprite



		if ( m_SnakeFrameRate < 50 ) // TODO:: Set this to be based on frame rate
		{
			m_MoveCooldownDuration = 1 - ( (float)m_SnakeFrameRate / 50.0f );
			m_MoveCooldownTimeLeft = m_MoveCooldownDuration;
		}
		else
		{
			m_MoveCooldownDuration = -0.1f;
			m_MoveCooldownTimeLeft = m_MoveCooldownDuration;
		}
	}


	// With some small changes the code below would work, but I decided to use layers instead. This means that the other classes decide their interactions with the snake, rather than having the snake do everything.
//	private void OnTriggerEnter2D( Collider2D collision )
//	{
//		if ( collision.gameObject.CompareTag( "Food" ) )
//			EatFood();
//		else if ( collision.gameObject.CompareTag( "Snake" ) )
//			Die();
//		else if ( collision.gameObject.CompareTag( "Wall" ) )
//			Die();
//	}


	void Update()
    {
		TakeInput();
    }


	private void FixedUpdate()
	{
		m_MoveCooldownTimeLeft -= Time.fixedDeltaTime;

		if ( m_MoveCooldownTimeLeft < 0.0f ) // Limit how often the snake will move
		{
			m_MoveCooldownTimeLeft = m_MoveCooldownDuration;

			Move();
		}
	}



	private void Move()
	{
		m_AlreadyMoved = false; // Set this to false in order to let the player input new move-commands

		for ( int SegmentIndex = m_Segments.Count; SegmentIndex-- > 1; ) // Loop through the list backwards and exit on element 0, since we want every segment except the head to have the position and direction of the previous segment. 
		{
			Segment CurrentSegment	= m_Segments[ SegmentIndex ];		// Safe because of the -- above
			Segment NextSegment		= m_Segments[ SegmentIndex - 1 ];	// This one is only safe if you start with 2 or more segments. Head + tail = 2 segments.

			CurrentSegment.transform.position		= NextSegment.transform.position;
			CurrentSegment.transform.eulerAngles	= NextSegment.transform.eulerAngles;
		}

		// Move the first segment 
		m_Segments[ 0 ].transform.position		= transform.position;
		m_Segments[ 0 ].transform.eulerAngles	= transform.eulerAngles;


		// Rotate the tail
		if ( m_Segments.Count > 1 ) // If there's more than 1 segment
			m_Segments[ m_Segments.Count - 1 ].transform.eulerAngles = m_Segments[ m_Segments.Count - 2 ].transform.eulerAngles;


		// Move and rotate the head
		transform.position += new Vector3( m_CurrentDirection.x, m_CurrentDirection.y, 0.0f );
		RotateSprite();
	}

	private void TakeInput() 
	{
		// Implemented a very primitive version of input; using the new input system seemed a bit overboard for this.

		if ( m_AlreadyMoved )
			return;

		if ( Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) )
		{
			if ( m_MoveDirection != MoveDirection.MOVEDIR_DOWN )
			{
				m_CurrentDirection	= Vector2.up;
				m_MoveDirection		= MoveDirection.MOVEDIR_UP;
				m_AlreadyMoved		= true;
			}
		}

		if ( Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) )
		{
			if ( m_MoveDirection != MoveDirection.MOVEDIR_RIGHT )
			{
				m_CurrentDirection	= Vector2.left;
				m_MoveDirection		= MoveDirection.MOVEDIR_LEFT;
				m_AlreadyMoved		= true;
			}
		}

		if ( Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) )
		{
			if ( m_MoveDirection != MoveDirection.MOVEDIR_UP )
			{
				m_CurrentDirection	= Vector2.down;
				m_MoveDirection		= MoveDirection.MOVEDIR_DOWN;
				m_AlreadyMoved		= true;
			}
		}

		if ( Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) )
		{
			if ( m_MoveDirection != MoveDirection.MOVEDIR_LEFT )
			{
				m_CurrentDirection	= Vector2.right;
				m_MoveDirection		= MoveDirection.MOVEDIR_RIGHT;
				m_AlreadyMoved		= true;
			}
		}
	}


	public void RotateSprite()
	{
		switch ( m_MoveDirection )
		{
			case MoveDirection.MOVEDIR_UP:
				transform.eulerAngles = new Vector3( 0.0f, 0.0f, 0.0f );
				break;

			case MoveDirection.MOVEDIR_LEFT:
				transform.eulerAngles = new Vector3( 0.0f, 0.0f, 90.0f );
				break;

			case MoveDirection.MOVEDIR_DOWN:
				transform.eulerAngles = new Vector3( 0.0f, 0.0f, 180.0f );
				break;

			case MoveDirection.MOVEDIR_RIGHT:
				transform.eulerAngles = new Vector3( 0.0f, 0.0f, -90.0f );
				break;
		}
	}


	public bool IsSegmentInPosition( Vector3 _Position )
	{
		//	Since we are moving on a grid, we could just compare the x- and y-values. Otherwise I would
		foreach ( Segment CurrentSegment in m_Segments )
		{
			if ( CurrentSegment.transform.position == _Position ) // Check distance using sqrMagnitude rather than regular magnitude, this is cheaper
				return true;
		}

		if ( transform.position == _Position )
			return true;


		return false;
	}


	public void EatFood()
	{
		m_CurrentScore++;

		string DisplayedScore = "";	

		if ( m_CurrentScore < 10 )
			DisplayedScore += "000";
		else if ( m_CurrentScore < 100 )
			DisplayedScore += "00";
		else if ( m_CurrentScore < 1000 )
			DisplayedScore += "0";

		DisplayedScore += m_CurrentScore;

		UIManager.Instance.UpdateCurrentScore( DisplayedScore ); // Update the displayed score to the right of the screen



		if ( m_CurrentScore > m_HighScore ) // If current score is also the highest score
		{
			m_HighScore = m_CurrentScore;
			UIManager.Instance.UpdateHighScore( DisplayedScore );
		}

		if ( m_CurrentScore + 1 >= m_MaxPossibleScore ) // TODO:: switch this out for the actual amount you need to win
		{
			UIManager.Instance.ShowVictory();
			return;
		}

		// Play sound effect (should be skipped if you win, so the victory tune can be played instead)
		GetComponent<AudioSource>().Play();


		/////////////////
		// Increase segments

		int SegmentCount = m_Segments.Count;

		Segment PreviousLastSegment = m_Segments[ SegmentCount - 1 ];
		PreviousLastSegment.SetSprite( m_BodySprite ); // Since this segment is no longer last, change its sprite

		Segment NewSegment		= Instantiate( m_SegmentPrefab ).GetComponent<Segment>();
		Vector3 NewSegmentPos	= m_Segments[ SegmentCount - 1 ].transform.position;

		NewSegment.transform.SetParent( transform.parent );
		NewSegment.SetSprite( m_TailSprite );
		NewSegment.transform.position = NewSegmentPos;	// Set the position and direction of the new segment

		m_Segments.Add( NewSegment );
	}


	public void Die()
	{
		enabled = false;

		UIManager.Instance.ShowGameOver();
	}
}
