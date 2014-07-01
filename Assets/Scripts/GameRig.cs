using UnityEngine;
using System.Collections;

public class GameRig : SplineFollower
{
	
	public float Speed = 3f;
	
	
	void Update()
	{
		Progress(Speed*Time.deltaTime);
	}
	
	
	
	
}
