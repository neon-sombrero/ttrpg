using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FadeAmount
{
	public Transform transform;
	public float Distance = 1;
	private Vector3 Position;

	public FadeAmount(Transform trans, float Dist)
	{
		transform = trans;
		Distance = Dist;
	}

	public FadeAmount(Vector3 Pos, float Dist)
	{
		Position = Pos;
		Distance = Dist;
	}

	public Vector3 GetPosition()
	{
		if(transform != null)
			return transform.position;

		return Position;
	}
}

public class FadeQueue : MonoBehaviour
{
	private List<FadeAmount> Fades = new List<FadeAmount>();
	private Texture2D NoiseTex;
	public static FadeQueue instance;

	public static void Add(FadeAmount newFade)
	{
		if(instance != null)
		{
			bool bExists = false;
			for(int i = 0; i < instance.Fades.Count; i++)
			{
				if(instance.Fades[i].GetPosition() == newFade.GetPosition())
				{
					bExists = true;
					instance.Fades[i].Distance = newFade.Distance;
				}
			}

			if(!bExists)
			{
				instance.Fades.Add(newFade);

				if(instance.Fades.Count >= 10)
				{
					while(instance.Fades.Count > 10)
					{
						instance.Fades.RemoveAt(0);
					}
				}
			}
		}
		else
		{
			GameObject GO = new GameObject("FadeSystem");
			GO.hideFlags = HideFlags.HideAndDontSave;
			instance = GO.AddComponent<FadeQueue>();
			Add(newFade);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if(NoiseTex == null)
		{
			NoiseTex = (Texture2D)Resources.Load<Texture2D>("NoiseTex");
		}
		if(NoiseTex != null)
		{
			Shader.SetGlobalTexture("_RandomNoise", NoiseTex);
		}

		if(Fades != null)
		{
			for(int i = 0; i < 10; i++)
			{
				if(i < Fades.Count)
				{
					if(Fades[i] != null)
					{
						if(Fades[i].GetPosition() != Vector3.zero)
						{
							Vector4 Pos = new Vector4(Fades[i].GetPosition().x, Fades[i].GetPosition().y, Fades[i].GetPosition().z, Fades[i].Distance);
							Debug.DrawRay(Pos, Vector3.up, Color.yellow);
							Shader.SetGlobalVector("_FadeData"+i.ToString(), Pos);
						}
						else
						{
							Shader.SetGlobalVector("_FadeData"+i.ToString(), new Vector4(0,0,0,0));
							Fades.RemoveAt(i);
							i--;
						}
					}
					else
					{
						Shader.SetGlobalVector("_FadeData"+i.ToString(), new Vector4(0,0,0,0));
						Fades.RemoveAt(i);
						i--;
					}
				}
				else
				{
					Shader.SetGlobalVector("_FadeData"+i.ToString(), new Vector4(0,0,0,0));
				}
			}
		}
	}
}
