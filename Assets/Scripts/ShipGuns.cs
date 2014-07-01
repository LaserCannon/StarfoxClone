using UnityEngine;
using System.Collections;

public enum GunMode { Single, Double, Mega, }

public class ShipGuns : MonoBehaviour
{

	public Transform GunTransformMiddle = null;
	public Transform GunTransformLeft = null;
	public Transform GunTransformRight = null;
	
	public Renderer CrosshairClose = null;
	public Renderer CrosshairFar = null;
	
	public float CHCloseDist = 1f;
	public float CHFarDist = 2f;
	
	public LayerMask aimMask;
	
	public Bullet BulletPrefab = null;
	
	private GunMode gunMode = GunMode.Double;
	
	
	
	public void DoUpdate()
	{
		Ray shootRay = new Ray(GunTransformMiddle.position,GunTransformMiddle.forward);
		
		Vector3 close = shootRay.GetPoint(CHCloseDist);
		Vector3 far = shootRay.GetPoint(CHFarDist);
		
		Ray camRayClose = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(close));
		Ray camRayFar = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(far));
		
		CrosshairClose.transform.position = camRayClose.GetPoint(1f);
		CrosshairFar.transform.position = camRayFar.GetPoint(1f);
	}
	
	
	public void Shoot(Collider firingObject)
	{
		Vector3 chpos = CrosshairFar.transform.position;
		Vector3 chscreenpos = Camera.main.WorldToScreenPoint(chpos);
		Ray ray = Camera.main.ScreenPointToRay(chscreenpos);
		ray.direction = Vector3.RotateTowards(firingObject.transform.forward,ray.direction,Mathf.PI/12f,1f);
		
		Vector3 towardPoint = Vector3.zero;
		RaycastHit hit = new RaycastHit();
		if(Physics.SphereCast(ray,0.5f,out hit,100f,aimMask))
		{
			towardPoint = hit.point;
		}
		else
		{
			towardPoint = ray.GetPoint(80f);
		}
		
		if(gunMode==GunMode.Single)
		{
			Bullet bullet = (Bullet)Instantiate(BulletPrefab,GunTransformMiddle.position,Quaternion.LookRotation(towardPoint-GunTransformMiddle.position));
			bullet.FiringObject = firingObject;	
		}
		else
		{
			Bullet bullet1 = (Bullet)Instantiate(BulletPrefab,GunTransformLeft.position,Quaternion.LookRotation(towardPoint-GunTransformLeft.position));
			bullet1.FiringObject = firingObject;
			Bullet bullet2 = (Bullet)Instantiate(BulletPrefab,GunTransformRight.position,Quaternion.LookRotation(towardPoint-GunTransformRight.position));
			bullet2.FiringObject = firingObject;	
		}
	}
		
	
}
