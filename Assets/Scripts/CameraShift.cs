using UnityEngine;
using System.Collections;

public class CameraShift : MonoBehaviour
{
	
	public float DistanceBack = 3f;
	
	public void UpdatePosition(Vector3 shipForward, Vector3 shipOffset, float speedRatio)
	{
		Vector3 targetOffset = shipOffset * 0.25f;
		Vector3 targetLookOffset = Vector3.Lerp(Vector3.forward, shipForward, 0.1f) + shipOffset * 0.015f;
		
		Vector3 upVec = Vector3.up;
		upVec.x += shipForward.x * 0.1f;
		
		transform.localPosition = Vector3.Lerp(transform.localPosition,targetOffset - Vector3.forward*DistanceBack,Time.deltaTime*5f);
		transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.LookRotation(targetLookOffset,upVec),Time.deltaTime*5f);
	}
	
	
}
