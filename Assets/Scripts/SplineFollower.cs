using UnityEngine;
using System.Collections;

public class SplineFollower : MonoBehaviour
{

	
	public Spline SplineFollowing = null;
	
	private float interp = 0f;
	private int node = 0;
	
	private Vector3 splinePos = Vector3.zero;
	private Quaternion splineRot = Quaternion.identity;
	private Vector3 splineTan = Vector3.forward;
	
	
	
	void Awake()
	{
		Progress(0f);
	}
	
	
	
	protected void Progress(float distance)
	{
		if(splineTan.sqrMagnitude > Mathf.Epsilon)
		{
			interp += distance / splineTan.magnitude;
			
			while(interp>=1f)
			{
				interp -= 1f;
				node = Mathf.Min(node+1,SplineFollowing.mHandles.Count-1);
			}
		}
		
		SplineFollowing.interpolateOnNode(interp,node,out splinePos,out splineRot,out splineTan,true);
		
		UpdatePosition(splinePos,splineRot);
	}
			
	protected virtual void UpdatePosition(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}
	
	
}
