using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BarycentricUVS : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Mesh m = GetComponent<MeshFilter> ().mesh;
		m.uv2 = GetBarycentricFromMesh (m);
		
	}

	Vector2[] GetBarycentricFromMesh(Mesh m) {
		List<Vector2> uvs = new List<Vector2>();
		Vector2[] fixedUvs = GetBarycentricFixed ();
		int fixedUvsLength = fixedUvs.Length;
		for (int i = 0; i < m.vertices.Length; i++) {
			uvs.Add (fixedUvs [i % fixedUvsLength]);
		}
		return uvs.ToArray ();
	}
	public Vector2[] GetBarycentricFixed()
	{
		Vector2[] uvs = new Vector2[]{
			new Vector2(0,0),
			new Vector2(0,1),
			// new Vector2(1,0),
			// new Vector2(1,1)
		};

		return uvs;
	}
}
