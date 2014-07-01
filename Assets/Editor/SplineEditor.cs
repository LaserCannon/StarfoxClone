using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
	private Spline Spline;
	private int Selection = -1;
	
	
	void OnEnable()
	{
		Spline = (Spline)target;
		Selection = -1;
	}
	
	
	void OnDisable()
	{
		Spline = null;
		Selection = -1;
	}
	
	
	void OnSceneGUI()
	{
		if (Spline == null)
			return;
		
		bool splineDirty = false;
		
		for (int i = 0; i < Spline.HandleCount; i++)
		{
			Spline.SplineHandle a = Spline.getHandleCopy(i, true);
			Spline.SplineHandle b = (i < Spline.HandleCount-1) ? Spline.getHandleCopy(i+1,true) : null;
			
			if (i == Selection)
			{
				switch (Tools.current)
				{
				case Tool.Move:
					{
						Undo.SetSnapshotTarget( target, "moved spline handle/tangent" );
					
						Quaternion handleRot = (Tools.pivotRotation == PivotRotation.Local) ? a.rotation : Quaternion.identity;
						Utilities.NormalizeQuaternion( ref handleRot );
					
						Vector3 posNew = Handles.PositionHandle( a.position, handleRot );
						if (posNew != a.position)
						{
							splineDirty = true;
							a.position = posNew;
						}
					
						Vector3 tIn = a.transformTangentIn();
						Vector3 tInNew = Handles.PositionHandle( tIn, handleRot );
						if (tInNew != tIn)
						{
							splineDirty = true;
							a.inverseTransformTangentIn( tInNew );
						}
					
						Vector3 tOut = a.transformTangentOut();
						Vector3 tOutNew = Handles.PositionHandle( tOut, handleRot );
						if (tOutNew != tOut)
						{
							splineDirty = true;
							a.inverseTansformTangentOut( tOutNew );
						}
					}
					break;
					
				case Tool.Rotate:
					{
						Quaternion handleRot = a.rotation;
						Utilities.NormalizeQuaternion( ref handleRot );
					
						Undo.SetSnapshotTarget( target, "rotated spline handle" );
					
						Quaternion rotNew = Handles.RotationHandle( handleRot, a.position );
						if (rotNew != handleRot)
						{
							splineDirty = true;
							a.rotation = rotNew;
							Utilities.NormalizeQuaternion(ref a.rotation);
						}
					}
					break;
				}
			}
			
			// draw the tangent handles...
			Handles.color = Color.yellow;
			Handles.DrawLine( a.position, a.transformTangentIn() );
			
			Handles.color = Color.cyan;
			Handles.DrawLine( a.position, a.transformTangentOut() );
			
			// draw "left" vector for reference
			Handles.color = Color.red;
			Handles.DrawLine( a.position, a.transformVector(Vector3.right) );
			
			// draw bezier...
			if (b != null)
			{
				Handles.DrawBezier( a.position, b.position, a.transformTangentOut(), b.transformTangentIn(), Color.white, null, HandleUtility.GetHandleSize(a.position) * 2.0f );
			}
			
			Spline.setHandle( i, a, true );
			
			
			// selector...
			if (i != Selection)
			{
				Handles.color = Color.grey;
				float size = HandleUtility.GetHandleSize( a.position ) * 0.2f;
				Quaternion rot = a.rotation;
				Utilities.NormalizeQuaternion(ref rot);
				if (Handles.Button( a.position, rot, size, size, Handles.CubeCap ))
				{
					Selection = i;
					EditorUtility.SetDirty(target);
				}
			}
			
			if (splineDirty)
			{
				Spline.markLengthDirty();
				
			/*	SplineRoad sr = Spline.gameObject.GetComponent<SplineRoad>();
				if (sr)
				{
					sr.generateMesh();
					Unwrapping.GenerateSecondaryUVSet( sr.getMesh() );
				}*/
			}
		}
		
		
	}
	
	public override void OnInspectorGUI()
	{
		// Implement new inspector buttons for our new features
        GUILayout.Label("Edit Mode");
	
		// add a point
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button( "Add Handle" ))
		{
			Spline.addHandle( (Selection >= 0) ? (Selection) : (-1) );
		}
		
		if (GUILayout.Button( "Remove Handle" ))
		{
			if (Selection >= 0)
			{
				Spline.removeHandle( Selection );
				Selection = -1;
			}
		}
		
		GUILayout.EndHorizontal();
		
		
		if ((Spline != null) && (Selection != -1))
		{
			Spline.SplineHandle a = Spline.getHandleCopy(Selection, false);
			a.position = EditorGUILayout.Vector3Field( "Position", a.position );
			
			Vector3 euler = a.rotation.eulerAngles;
			Vector3 newEuler = EditorGUILayout.Vector3Field( "Rotation", euler );
			if (newEuler != euler)
			{
				a.rotation = Quaternion.Euler( newEuler );
				Utilities.NormalizeQuaternion(ref a.rotation);
			}
			
			a.tangentIn = EditorGUILayout.Vector3Field( "Tangent In", a.tangentIn );
			a.tangentOut = EditorGUILayout.Vector3Field( "Tangent Out", a.tangentOut );
			
			Spline.setHandle( Selection, a, false );
		}
		
		
		if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
			
			Spline.markLengthDirty();
				
		/*	SplineRoad sr = Spline.gameObject.GetComponent<SplineRoad>();
			if (sr)
				sr.generateMesh();*/
        }
        // After we drawn our stuff, draw the default inspector
        DrawDefaultInspector();
	}

}
