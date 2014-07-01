using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

	private int hp = 5;

	public ParticleSystem ParticlePrefab = null;


	void OnWasHit(int power)
	{
		hp -= power;

		StopAllCoroutines();


		if(hp<=0)
		{
			ParticleSystem prt = (ParticleSystem)Instantiate(ParticlePrefab,transform.position,transform.rotation);
			Destroy (prt.gameObject,3f);
			Destroy (gameObject);
			return;
		}


		StartCoroutine(Flash ());
	}


	IEnumerator Flash()
	{
		float lastTimeHit = Time.time;
		while(Time.time-lastTimeHit < 0.5f)
		{
			renderer.material.color = Color.red;

			yield return new WaitForSeconds(0.05f);

			renderer.material.color = Color.white;

			yield return new WaitForSeconds(0.05f);
		}
	}


}
