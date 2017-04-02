using UnityEngine;

public class MapGenerator : MonoBehaviour {

	[SerializeField]
	private int mapWidth = 16;

	[SerializeField]
	private int mapHeight = 9;

	[SerializeField]
	[Range(0, 100)]
	private int mapFillPercent = 45;

	[SerializeField]
	private bool useRandomSeed = true;

	[SerializeField]
	private string seed = "";

	[SerializeField]
	private int wallThreshold = 4;

	[SerializeField]
	private int emptyThreshold = 3;

	private int[,] map;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	private void Start()
	{
		GenerateMap(mapWidth, mapHeight);
		SmoothMap(map);
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GenerateMap(mapWidth, mapHeight);
		}

		if (Input.GetMouseButtonDown(1))
		{
			SmoothMap(map);
		}
	}

	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	private void OnDrawGizmos()
	{
		if (map == null) return;

		int w = map.GetLength(0);
		int h = map.GetLength(1);

		for (int x = 0; x < w; ++ x)
		{
			for (int y = 0; y < h; ++ y)
			{
				Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
				Vector3 pos = new Vector3(
					-w / 2 + x + 0.5f,
					-h / 2 + y + 0.5f,
					0f
				);
				Gizmos.DrawCube(pos, Vector3.one);
			}
		}
	}

	private void GenerateMap(int w, int h)
	{
		map = new int[w, h];

		if (useRandomSeed)
		{
			seed = Time.time.ToString();
		}

		FillMap(map, seed, mapFillPercent);
	}

	private void FillMap(int[,] map, string seed, int fill)
	{
		System.Random rando = new System.Random(seed.GetHashCode());

		int w = map.GetLength(0);
		int h = map.GetLength(1);

		for (int x = 0; x < w; ++ x)
		{
			for (int y = 0; y < h; ++ y)
			{
				bool isWall = rando.Next(0, 100) < fill;
				bool isEdge = x == 0 || x == w - 1 || y == 0 || y == h - 1;
				map[x, y] = (isWall || isEdge) ? 1 : 0;
			}
		}
	}

	private void SmoothMap(int[,] map)
	{
		int w = map.GetLength(0);
		int h = map.GetLength(1);

		for (int x = 0; x < w; ++ x)
		{
			for (int y = 0; y < h; ++ y)
			{
				int n = GetNeighboringWallCount(map, x, y);

				if (n > wallThreshold) map[x,y] = 1;
				else if (n < emptyThreshold) map[x,y] = 0;
			}
		}
	}

	private int GetNeighboringWallCount(int[,] map, int x, int y)
	{
		int count = 0;

		int w = map.GetLength(0);
		int h = map.GetLength(1);

		for (int i = x - 1; i <= x + 1; ++ i)
		{
			for (int j = y - 1; j <= y + 1; ++ j)
			{
				if (i == x && j == y) continue;

				int val = map[
					Mathf.Min(Mathf.Max(0, i), w - 1),
					Mathf.Min(Mathf.Max(0, j), h - 1)
				];
				count += val;
			}
		}

		return count;
	}

}
