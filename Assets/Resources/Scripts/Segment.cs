using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// In C++ this would fit better as a struct
public class Segment : MonoBehaviour
{
	private SpriteRenderer	m_SpriteRenderer;	// The sprite used for this segment

	private void Awake()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		collision.GetComponent<Snake>().Die();
	}

	public void SetSprite( Sprite _NewSprite )
	{
		m_SpriteRenderer.sprite = _NewSprite;
	}
}
