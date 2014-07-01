using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spline : MonoBehaviour 
{
	[System.Serializable]
	public class SplineHandle
	{
		[SerializeField]
		public Vector3 position = Vector3.zero;
		
		[SerializeField]
		public Quaternion rotation = Quaternion.identity;
		
		[SerializeField]
		public Vector3 tangentIn = Vector3.back;
		
		[SerializeField]
		public Vector3 tangentOut = Vector3.forward;
		
		public Vector3 transformTangentIn() { return transformVector(tangentIn); }
		public Vector3 transformTangentOut() { return transformVector(tangentOut); }
		
		public void inverseTransformTangentIn( Vector3 v ) { tangentIn = inverseTransformVector(v); }
		public void inverseTansformTangentOut( Vector3 v ) { tangentOut = inverseTransformVector(v); }
		
		public Vector3 transformVector( Vector3 v ) { return position + (rotation * v); }
		public Vector3 inverseTransformVector( Vector3 v ) { return Quaternion.Inverse(rotation) * (v - position); }
	};
	
	[SerializeField]
	public List<SplineHandle> mHandles = new List<SplineHandle>();
	
	// cache of segment lengths
	[SerializeField]
	private float[] mSegLengths = null;
	
	// cache of overall length
	[SerializeField]
	private float mLength = 0.0f;
	
	// flag to signal we need to re-gen length cache.
	[SerializeField]
	private bool mLengthDirty = true;
	
	// Use this for initialization
	void Start () 
	{
		mLengthDirty = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void markLengthDirty()
	{
		mLengthDirty = true;
	}
	
	public float getLength()
	{
		if (mLengthDirty)
			_recalcLengthCache();
		
		return mLength;
	}
	
	//public float getSegmentLength()
	//{}
	
	public int HandleCount
	{
		get { return mHandles.Count; }
	}
	public int SegmentCount
	{
		get { return mSegLengths.Length; }
	}
	
	public Vector3 getHandlePos(int i, bool transformToWorldSpace)
	{
		if ((i < 0) || (i >= mHandles.Count))
			return Vector3.zero;
		
		if(transformToWorldSpace)
			return gameObject.transform.TransformPoint(mHandles[i].position);
		else
			return mHandles[i].position;
	}
	
	public Quaternion getHandleRot(int i, bool transformToWorldSpace)
	{
		if ((i < 0) || (i >= mHandles.Count))
			return Quaternion.identity;
		
		if(transformToWorldSpace)
			return gameObject.transform.rotation * mHandles[i].rotation;
		else
			return mHandles[i].rotation;
	}
	
	public SplineHandle getHandleFast(int i)
	{
		if ((i < 0) || (i >= mHandles.Count))
			return null;
		
		return mHandles[i];
	}
	
	public SplineHandle getHandleCopy( int i, bool transformToWorldSpace )
	{
		if ((i < 0) || (i >= mHandles.Count))
			return null;
		
		SplineHandle ret = new SplineHandle();
		
		ret.position = mHandles[i].position;
		ret.rotation = mHandles[i].rotation;
		ret.tangentIn = mHandles[i].tangentIn;
		ret.tangentOut = mHandles[i].tangentOut;
		
		if (transformToWorldSpace)
		{
			ret.position = gameObject.transform.TransformPoint( ret.position );
			ret.rotation = gameObject.transform.rotation * ret.rotation;
		}
		
		return ret;
	}
	
	public void setHandle( int i, SplineHandle handle, bool isInWorldSpace )
	{
		if ((i < 0) || (i >= mHandles.Count))
			return;
		
		SplineHandle s = mHandles[i];
		
		if (isInWorldSpace)
		{
			s.position = gameObject.transform.InverseTransformPoint( handle.position );
			s.rotation = Quaternion.Inverse(gameObject.transform.rotation) * handle.rotation;
		}
		else
		{
			s.position = handle.position;
			s.rotation = handle.rotation;
		}		
		
		s.tangentIn = handle.tangentIn;
		s.tangentOut = handle.tangentOut;
	}
	
	public void addHandle()
	{
		addHandle(mHandles.Count - 1);
	}
	
	public void SmoothSlpine(int index)
	{
		if(mHandles.Count <= 1)
		{
			return;
		}
		
		if(index == 0)
		{
			mHandles[0].tangentOut = (mHandles[1].position - mHandles[0].position)/3f;
			
			//mHandles[0].tangentOut *= -1f * mHandles[0].rotation;
		}
		else if(index == mHandles.Count - 1)
		{
			mHandles[mHandles.Count - 1].tangentIn = (mHandles[mHandles.Count - 2].position - mHandles[mHandles.Count - 1].position)/3f;
		}
		else
		{
			mHandles[index].tangentOut = (mHandles[index + 1].position - mHandles[index - 1].position)/6f;
			mHandles[index].tangentIn = (mHandles[index - 1].position - mHandles[index + 1].position)/6f;
			
		}
	}
	public void SmoothWholeSlpine()
	{
		for(int i = 0; i < mHandles.Count; i++)
		{
			SmoothSlpine(i);
		}
	}
	
	public void addHandle(int after_index)
	{
		SplineHandle before = null;
		SplineHandle after = null;
		
		if ((after_index >= 0) && (after_index < mHandles.Count))
		{
			before = mHandles[after_index];
			if (after_index < (mHandles.Count-1))
				after = mHandles[after_index+1];
		}
		
		SplineHandle n = new SplineHandle();
		if (before != null)
		{
			if (after != null)
			{
				n.position = Vector3.Lerp(before.position, after.position, 0.5f);
				n.rotation = Quaternion.Lerp(before.rotation, after.rotation, 0.5f);
			}
			else
			{
				n.position = before.position + (before.rotation * new Vector3(0,0,2));
				n.rotation = before.rotation;
			}
		}
		else
		{
			n.position = new Vector3(0,0,1);
			n.rotation = Quaternion.identity;
		}
		
		if (before != null)
		{
			mHandles.Insert( after_index+1, n );
		}
		else
		{
			mHandles.Add( n );
		}
		
		mLengthDirty = true;
	}
	
	
	public void removeHandle( int idx )
	{
		if ((idx >= 0) && (idx < mHandles.Count))
		{
			mHandles.RemoveAt( idx );
			
			mLengthDirty = true;
		}
	}
	
	public void clearHandles()
	{
		mHandles.Clear();
		mLengthDirty = true;
	}
	
	Vector3 apos;
	Vector3 bpos;
	Quaternion arot;
	Quaternion brot;
	Vector3 a_tan;
	Vector3 b_tan;
	Vector3 fwd;
	float final_t;
	
	
	public void interpolateOnNode( float t, int nodeIndex, out Vector3 pos, out Quaternion orient, out Vector3 tangent, bool inWorldSpace )
	{
		if (mHandles.Count == 0)
		{
			pos = Vector3.zero;
			orient = Quaternion.identity;
			tangent = Vector3.forward;
			return;
		}
		if (mHandles.Count == 1)
		{
			pos = mHandles[0].position;
			orient = mHandles[0].rotation;
			tangent = Vector3.forward;
			return;
		}
		if(mHandles.Count<=nodeIndex-1)
		{
			pos = mHandles[mHandles.Count-1].position;
			orient = mHandles[mHandles.Count-1].rotation;
			tangent = mHandles[mHandles.Count-1].tangentOut;
		}
		
		if (mLengthDirty)
			_recalcLengthCache();
		
		int i = nodeIndex;//(int) (t*(float)(mSegLengths.Length));
		
		
		final_t = t;//(t*(float)(mSegLengths.Length)) % 1f;
		
		if(i==mSegLengths.Length)
		{
			i--;
			final_t = 1f;
		}

		SplineHandle ah = getHandleFast(i);//, inWorldSpace);
		SplineHandle bh = getHandleFast(i+1);//, inWorldSpace);
		
		//apos = inWorldSpace ? transform.TransformPoint(ah.position) : ah.position;
		//bpos = inWorldSpace ? transform.TransformPoint(bh.position) : bh.position;
		//arot = inWorldSpace ? transform.rotation * ah.rotation : ah.rotation;
		//brot = inWorldSpace ? transform.rotation * bh.rotation : bh.rotation;
		if(ah == null)
		{
		Debug.Log(i + " " +  t);	
		}
		a_tan = ah.position + (ah.rotation * ah.tangentOut);
		b_tan = bh.position + (bh.rotation * bh.tangentIn);
	
		pos = Utilities.bezierInterp(ref ah.position, ref a_tan, ref b_tan, ref bh.position, final_t);
		fwd = Utilities.bezierTangentInterp(ref ah.position, ref a_tan, ref b_tan, ref bh.position, final_t);
		
		orient = Quaternion.LookRotation(fwd, Quaternion.Slerp(ah.rotation, bh.rotation, final_t) * Vector3.up);
		
		tangent = fwd * (float)(mSegLengths.Length);
		
		if(inWorldSpace)
		{
			pos = transform.TransformPoint(pos);
			orient = transform.rotation * orient;
			tangent = transform.TransformDirection(tangent);
		}
	}
	
	public void interpolate( float t, out Vector3 pos, out Quaternion orient, out Vector3 tangent, bool inWorldSpace )
	{
		if (mHandles.Count == 0)
		{
			pos = Vector3.zero;
			orient = Quaternion.identity;
			tangent = Vector3.forward;
			return;
		}
		if (mHandles.Count == 1)
		{
			pos = mHandles[0].position;
			orient = mHandles[0].rotation;
			tangent = Vector3.forward;
			return;
		}
		
		if (mLengthDirty)
			_recalcLengthCache();
		
		int i = (int) (t*(float)(mSegLengths.Length));
		
		
		final_t = (t*(float)(mSegLengths.Length)) % 1f;
		
		if(i==mSegLengths.Length)
		{
			i--;
			final_t = 1f;
		}

		SplineHandle ah = getHandleFast(i);//, inWorldSpace);
		SplineHandle bh = getHandleFast(i+1);//, inWorldSpace);
		
		//apos = inWorldSpace ? transform.TransformPoint(ah.position) : ah.position;
		//bpos = inWorldSpace ? transform.TransformPoint(bh.position) : bh.position;
		//arot = inWorldSpace ? transform.rotation * ah.rotation : ah.rotation;
		//brot = inWorldSpace ? transform.rotation * bh.rotation : bh.rotation;
		if(ah == null)
		{
		Debug.Log(i + " " +  t);	
		}
		a_tan = ah.position + (ah.rotation * ah.tangentOut);
		b_tan = bh.position + (bh.rotation * bh.tangentIn);
	
		pos = Utilities.bezierInterp(ref ah.position, ref a_tan, ref b_tan, ref bh.position, final_t);
		fwd = Utilities.bezierTangentInterp(ref ah.position, ref a_tan, ref b_tan, ref bh.position, final_t);
		
		orient = Quaternion.LookRotation(fwd, Quaternion.Slerp(ah.rotation, bh.rotation, final_t) * Vector3.up);
		
		tangent = fwd * (float)(mSegLengths.Length);
		
		if(inWorldSpace)
		{
			pos = transform.TransformPoint(pos);
			orient = transform.rotation * orient;
			tangent = transform.TransformDirection(tangent);
		}
	}
	
	
	
	private void _recalcLengthCache()
	{
		if (mHandles.Count == 0)
		{
			mLength = 0.0f;
			return;
		}
		
		mSegLengths = new float[ mHandles.Count - 1 ];
		mLength = 0.0f;
		
		for (int i = 0; i < (mHandles.Count-1); i++)
		{
			float seg_l = 0.0f;
			
			SplineHandle a = getHandleCopy(i, true);
			SplineHandle b = getHandleCopy(i+1, true);
			
			Vector3 a_tan = a.position + (a.rotation * a.tangentOut);
			Vector3 b_tan = b.position + (b.rotation * b.tangentIn);
			
			Vector3 prev = a.position;
			
			for (int j = 0; j < 30; j++)
			{
				Vector3 cur = Utilities.bezierInterp(ref a.position, ref a_tan, ref b_tan, ref b.position, (float)j / 29.0f);
				
				seg_l += Vector3.Distance(cur, prev);
				
				prev = cur;				
			}
			
			mSegLengths[i] = seg_l;		
			mLength += seg_l;
		}
		mLengthDirty = false;
	}
	
	
	
	
	void OnDrawGizmos()
	{
		// draw our path!
		Gizmos.color = Color.yellow;
		
		for (int i = 0; i < (mHandles.Count-1); i++)
		{
			SplineHandle a = getHandleCopy(i, true);
			SplineHandle b = getHandleCopy(i+1, true);
			
			Vector3 a_tan = a.position + (a.rotation * a.tangentOut);
			Vector3 b_tan = b.position + (b.rotation * b.tangentIn);
			
			Vector3 prev = a.position;
			
			for (int j = 0; j < 10; j++)
			{
				Vector3 cur = Utilities.bezierInterp(ref a.position, ref a_tan, ref b_tan, ref b.position, (float)j / 9.0f);
				
				Gizmos.DrawLine(prev, cur);
				
				prev = cur;				
			}
			
		}
	}
	
}
