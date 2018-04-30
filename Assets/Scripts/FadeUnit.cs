using UnityEngine;
using System.Collections;

public class FadeUnit : MonoBehaviour
{
	public float FadeSize = 6.0f;
	public int NumSegments = 5;

	// Update is called once per frame
	void Update ()
	{
		float Distance = Vector3.Distance(Camera.main.transform.position, GetComponent<Renderer>().bounds.center);
		//Get a range that won''t overlap the target
		Distance = ((Distance/10.0f)*7.5f);
		Vector3 Direction = (Camera.main.transform.position - GetComponent<Renderer>().bounds.center).normalized;

		for(int i = 1; i <= NumSegments; i++)
		{
			float TmpDist = (Distance / NumSegments) * i;

			Vector3 Position = Camera.main.transform.position - Direction * TmpDist;
			Debug.DrawRay(Position, Vector3.up, Color.magenta);
			FadeQueue.Add(new FadeAmount(Position, FadeSize));
		}
	}
}
