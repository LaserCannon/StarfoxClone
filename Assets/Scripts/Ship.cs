using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
	
	public Vector2 MaxSpeed = new Vector2(5f,5f);
	public float Responsiveness = 10f;
	public float RollRatio = 0.1f;
	public float TurnRatio = 0.1f;
	
	public CameraShift CameraRig = null;
	
	public ShipGuns Guns = null;
	
	
	private Vector2 speed = Vector2.zero;
	
	private float bankDir = 0f;
	
	void Update()
	{
		Vector2 input = Vector2.zero;
		
		/*if(Input.GetKey(KeyCode.UpArrow))
		{
			input.y -= 1f;
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			input.y += 1f;
		}
		if(Input.GetKey(KeyCode.RightArrow))
		{
			input.x += 1f;
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			input.x -= 1f;
		}*/
		
		if(input==Vector2.zero)
		{
			input.x += Input.GetAxis("Horizontal");
			input.y -= Input.GetAxis("Vertical");
		}
		
		Move (input);
		
		Guns.DoUpdate();

		bankDir = 0f;
		
		if(Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
		{
			Guns.Shoot(collider);
		}
		if(Input.GetKey(KeyCode.X) || Input.GetButton("Fire2"))
		{
			bankDir+=1f;
		}
		if(Input.GetKey(KeyCode.Z) || Input.GetButton("Fire3"))
		{
			bankDir-=1f;
		}
	}
	
	
	void Move(Vector2 inputVector)
	{
		Vector2 targetSpeed = new Vector2(MaxSpeed.x * inputVector.x, MaxSpeed.y * inputVector.y);
		
		speed = Vector2.Lerp(speed,targetSpeed,Time.deltaTime*Responsiveness);
		
		Vector3 speed3 = new Vector3(speed.x,speed.y);
		speed3.x *= (Mathf.Abs(bankDir))*0.5f + 1f;
		
		Vector3 delta = speed3 * Time.deltaTime;

		Vector3 newPos = transform.localPosition;
		newPos += delta;
		if(Mathf.Abs(newPos.x)>9)
			newPos.x = 9 * Mathf.Sign (newPos.x);
		if(Mathf.Abs(newPos.y)>6)
			newPos.y = 6 * Mathf.Sign (newPos.y);
		transform.localPosition = newPos;
		
		if(delta.sqrMagnitude > Mathf.Epsilon)
		{
			Vector3 upVec = Vector3.up;
			upVec.x = speed.x*RollRatio;
			transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.AngleAxis(90f*bankDir,Vector3.forward)*Quaternion.LookRotation((1f-Mathf.Abs(bankDir))*speed3*TurnRatio+Vector3.forward,upVec),Time.deltaTime*Responsiveness);
		}
		
		CameraRig.UpdatePosition((delta+Vector3.forward/5f).normalized,transform.localPosition,1f);
	}
	
	
}
