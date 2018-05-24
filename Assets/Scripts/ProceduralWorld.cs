using UnityEngine;
using System.Collections;

public class ProceduralWorld : MonoBehaviour {
	public Renderer rend;
	public Material material;
	public int textureSize = 100;
	public Color primaryGridColor = Color.green;
	public Color secondaryGridColor = Color.blue;
	public Color tertiaryGridColor = Color.green / 4;
	public Color backgroundColor = Color.black;
	public float transformScale = 10;
	private Texture2D texture;
	private Color[] pixels;
	void Start() {
		if (!rend) {
			rend = GetComponent<Renderer> ();
		}
		if (rend && !material) {
			material = rend.material;
		}

		pixels = getBaseTexture ().GetPixels ();;
	}

	void Update() {
		material.mainTexture = getNextTexture ();
	}

	Texture2D getBaseTexture() {
		Texture2D t = new Texture2D (textureSize, textureSize);
		for (int x = 0; x < textureSize; x++) {
			for (int y = 0; y < textureSize; y++) {
				t.SetPixel (x, y, backgroundColor);
			}
		}
		t.filterMode = FilterMode.Point;
		t.Apply ();
		return t;
	}

	Texture2D getNextTexture() {
		Texture2D t = new Texture2D (textureSize, textureSize);
		t.SetPixels (pixels);
		Vector3 cameraPosition = Camera.main.transform.position * transformScale;
		Vector2Int offset = new Vector2Int(
			Mathf.RoundToInt(cameraPosition.x),
			Mathf.RoundToInt(cameraPosition.z)
		);
		int mid = Mathf.RoundToInt (textureSize / 2);
		for (int i = 0; i < textureSize; i++) {
			t.SetPixel (mid + offset.x, i, tertiaryGridColor);
			t.SetPixel (i, mid + offset.y, tertiaryGridColor);
		}
		for (int i = 0; i < textureSize; i++) {
			t.SetPixel (0, i, primaryGridColor);
			t.SetPixel (i, 0, primaryGridColor);
		}
		t.Apply ();
		return t;
	}
}
