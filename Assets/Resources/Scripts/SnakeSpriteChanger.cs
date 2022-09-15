using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpriteChanger : MonoBehaviour
{
	[SerializeField] private UIManager.ESnakeSprite		m_SpriteToAffect;
	[SerializeField] private int						m_ChangeDirection;

	public void UIButtonChangeSpriteTexture()
	{
		UIManager.Instance.CycleSnakeTexture( m_SpriteToAffect, m_ChangeDirection );
	}
}
