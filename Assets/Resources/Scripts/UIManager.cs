using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
	private static UIManager m_Instance;

	public static UIManager Instance => m_Instance;



	public enum ESnakeSprite
	{
		SPRITE_Head	,
		SPRITE_Body	,
		SPRITE_Tail
	}




	[SerializeField]	private Text		m_HighScoreText;
	[SerializeField]	private Text		m_CurrentScoreText;
	[Space]

	[SerializeField]	private Snake		m_Snake;
	[SerializeField]	private Food		m_Food;
	[Space]

	[SerializeField]	private GameObject	m_MainButtonsParent;
	[SerializeField]	private GameObject	m_OptionsButtonparent;
	[Space]

	[SerializeField]	private GameObject	m_VictoryScreen;
	[SerializeField]	private GameObject	m_GameOverScreen;
	[Space]

	[SerializeField]	private Image		m_DisplayHeadSprite;
	[SerializeField]	private Image		m_DisplayBodySprite;
	[SerializeField]	private Image		m_DisplayTailSprite;


						private int			m_HeadSpriteIndex;
						private int			m_BodySpriteIndex;
						private int			m_TailSpriteIndex;

						private Sprite[]	m_HeadSprites;	// All the head sprites
						private Sprite[]	m_BodySprites;	// All the body sprites
						private Sprite[]	m_TailSprites;	// All the tail sprites


	private void Awake()
	{
		if ( !m_Instance )
			m_Instance = this;
		else
		{
			Debug.LogError( "More than one instance of UIManager has been detected. Care: deleting the extra..." );
			Destroy( gameObject );
		}

		m_HeadSprites = Resources.LoadAll<Sprite>("Art/Snake/Heads");
		m_BodySprites = Resources.LoadAll<Sprite>("Art/Snake/Bodies");
		m_TailSprites = Resources.LoadAll<Sprite>("Art/Snake/Tails");

	}

	// Used by a UI-button to start a game of classic snake
	public void UIButtonStart()
	{
		m_MainButtonsParent.SetActive( false );
		m_OptionsButtonparent.SetActive( false );


		m_Snake.gameObject.SetActive( true );

		// Prepare all the correct sprites
		Sprite HeadSprite = m_HeadSprites[ m_HeadSpriteIndex ];
		Sprite BodySprite = m_BodySprites[ m_BodySpriteIndex ];
		Sprite TailSprite = m_TailSprites[ m_TailSpriteIndex ];

		m_Snake.GenerateSnake( HeadSprite, BodySprite, TailSprite );

		// The food object is disabled at the start of the game, enable it
		m_Food.gameObject.SetActive( true );

		// Reset text since we're starting a new game
		m_CurrentScoreText.text = "0000";
	}

	public void UIButtonOptions()
	{
		m_MainButtonsParent.SetActive( false );
		m_OptionsButtonparent.SetActive( true );
		Debug.Log( "Starting a game of modern snake!" );
	}

	public void UIButtonExit()
	{
		Application.Quit();
	}


	public void UIButtonSnakeColorCycleLeft()
	{

	}



	public void UIButtonSnakeColorCycleRight()
	{

	}


	// Cycles through which sprite will be used for a specific part of the snake.
	public void CycleSnakeTexture( ESnakeSprite _SpriteToAffect, int _CycleDirection )
	{
		int ClampedDirection = Mathf.Clamp( _CycleDirection, -1, 1 );

		switch ( _SpriteToAffect )
		{
			case ESnakeSprite.SPRITE_Head:
				{
					m_HeadSpriteIndex += ClampedDirection;

					if ( m_HeadSpriteIndex >= m_HeadSprites.Length )
						m_HeadSpriteIndex = 0;
					else if ( m_HeadSpriteIndex < 0 )
						m_HeadSpriteIndex = m_HeadSprites.Length - 1;

					m_DisplayHeadSprite.sprite = m_HeadSprites[ m_HeadSpriteIndex ];
				}
				break;

			case ESnakeSprite.SPRITE_Body:
				{
					m_BodySpriteIndex += ClampedDirection;

					if ( m_BodySpriteIndex >= m_BodySprites.Length )
						m_BodySpriteIndex = 0;
					else if ( m_BodySpriteIndex < 0 )
						m_BodySpriteIndex = m_BodySprites.Length - 1;

					m_DisplayBodySprite.sprite = m_BodySprites[ m_BodySpriteIndex ];
				}
				break;

			case ESnakeSprite.SPRITE_Tail:
				{
					m_TailSpriteIndex += ClampedDirection;

					if ( m_TailSpriteIndex >= m_TailSprites.Length )
						m_TailSpriteIndex = 0;
					else if ( m_TailSpriteIndex < 0 )
						m_TailSpriteIndex = m_TailSprites.Length - 1;

					m_DisplayTailSprite.sprite = m_TailSprites[ m_TailSpriteIndex ];
				}
				break;
		}
	}



	public void UIButtonShowMain()
	{
		m_MainButtonsParent.SetActive( true );
		m_OptionsButtonparent.SetActive( false );

		m_VictoryScreen.SetActive( false );
		m_GameOverScreen.SetActive( false );
	}



	public void ShowVictory()
	{
		m_VictoryScreen.SetActive( true );
	}



	public void ShowGameOver()
	{
		m_GameOverScreen.SetActive( true );
	}


	public void UpdateCurrentScore( string _NewCurrentScore )
	{
		m_CurrentScoreText.text = _NewCurrentScore;
	}



	public void UpdateHighScore( string _NewHighScore )
	{
		m_HighScoreText.text = _NewHighScore;
	}

}
