using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour
{
	public float distance = 15;
	public float camHeight = 5;
	public float xSpeed = 150;
	public float xMinLimit = -360;
	public float xMaxLimit = 360;

	private bool rightClicked = false;
	// private bool spacePressed = false;
	private float x;
	private Vector3 position;
	private Quaternion rotation;

	void Awake()
	{		
		Vector3 angles = transform.eulerAngles;
		x = angles.x;
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(1))
		{
			rightClicked = true;
		}
		if(Input.GetMouseButtonUp(1))
		{
			rightClicked = false;
		}

		if(Camera.main != null)
		{
			//Force as child
			Camera.main.transform.parent = transform;
			//Wank
			Camera.main.orthographic = false;

			if(rightClicked == true)
			{
				x += (float)(Input.GetAxis("Mouse X") * xSpeed * 0.02f);
				x = ClampAngle(x, xMinLimit, xMaxLimit);
			}

			rotation = Quaternion.Euler(30f, x, 0f);
			position = rotation * new Vector3(0f, camHeight, -distance ) ;
			
			Camera.main.transform.localRotation = rotation;
			Camera.main.transform.localPosition = position;
		}//if
	}//Update

	private float ClampAngle(float angle, float min, float max)
	{
		if(angle < -360)
		{
			angle += 360;
		}
		if(angle > 360)
		{
			angle -= 360;
		}
		return Mathf.Clamp (angle, min, max);
	}//ClampAngle
}