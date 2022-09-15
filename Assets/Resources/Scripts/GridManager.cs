using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	private static GridManager m_Instance;

	public static GridManager Instance => m_Instance;	// TODO:: Remove this in favour of an event system or just give a reference to the snake.

	// Usually I would just specify a size here, but because of the offset needed because of the white panel on the right, and some other strange things regarding the scale of the snake needing the walls to be at exact intervals, I decided not to. 
	[SerializeField] private int	m_GridMinX;
	[SerializeField] private int	m_GridMaxX;
	[Space]
	[SerializeField] private int	m_GridMinY;
	[SerializeField] private int	m_GridMaxY;

	public int	GridMinX => m_GridMinX;
	public int	GridMaxX => m_GridMaxX;

	public int	GridMinY => m_GridMinY;
	public int	GridMaxY => m_GridMaxY;



	private void Awake()
	{
		if ( !m_Instance )
		{
			m_Instance = this;
		}
		else
		{
			Debug.LogError( "More than one instance of GridManager has been detected. Care: deleting the extra..." );
			Destroy( gameObject );
			return;
		}
	}

	public int GetMaxNumScore()
	{
		return ( Mathf.Abs(m_GridMinX) + Mathf.Abs(GridMaxX) ) * ( Mathf.Abs(GridMinY) + Mathf.Abs(GridMaxY) );
	}

}
