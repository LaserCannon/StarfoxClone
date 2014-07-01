using UnityEngine;
using System.Collections;

public static class Utilities
{

	public static void NormalizeQuaternion (ref Quaternion q)
	{
  		float sum = 0;

    	for (int i = 0; i < 4; ++i)
   			sum += q[i] * q[i];

    	float magnitudeInverse = 1 / Mathf.Sqrt(sum);

    	for (int i = 0; i < 4; ++i)
   			q[i] *= magnitudeInverse;   
	}
	
	
	public static Vector3 bezierInterp( ref Vector3 a, ref Vector3 a_tan, ref Vector3 b_tan, ref Vector3 b, float t )
	{
	    return (((-a + 3*(a_tan-b_tan) + b)* t + 3*((a+b_tan) - 2*a_tan))* t + 3*(a_tan-a))* t + a;
		
	}
	
	public static Vector3 bezierTangentInterp( ref Vector3 a, ref Vector3 a_tan, ref Vector3 b_tan, ref Vector3 b, float t )
	{
		return 3 * ( ( (b - 3*b_tan + 3*a_tan - a)*t + 2*(b_tan - 2*a_tan + a))*t + (a_tan - a) );
	}
}
