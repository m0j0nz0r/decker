using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCircuitTexture : MonoBehaviour {
	private Side _North;
	private Side _South;
	private Side _East;
	private Side _West;

	public float lineWidth = 4;
	public float antiAliasing = 0.5f;
	public Color lineColor = Color.black;
	public  int cellSize = 20;

	private int gridSize = 9;


	// types
	[System.Serializable]
	public struct Side {
		public ushort value;
		public byte connected {
			get {
				return (byte)((this.value & 0xff00) >> 8);
			}
		}
		byte polarity {
			get {
				return (byte)(this.value & 0x00ff);
			}
		}
		public byte positive {
			get {
				return (byte)(this.connected & this.polarity);
			}
		}
		public byte negative {
			get {
				return (byte)(this.connected & ~this.polarity);
			}
		}
	}

	class Connection {
		public Node Target = null;
	}

	class Connections {
		public Connection North = new Connection();
		public Connection NorthEast = new Connection();
		public Connection East = new Connection();
		public Connection SouthEast = new Connection();
		public Connection South = new Connection();
		public Connection SouthWest = new Connection();
		public Connection West = new Connection();
		public Connection NorthWest = new Connection();
		public List<Connection> toList() {
			List<Connection> list = new List<Connection> ();
			if (NorthEast.Target != null)
				list.Add (NorthEast);
			if (SouthEast.Target != null)
				list.Add (SouthEast);
			if (SouthWest.Target != null)
				list.Add (SouthWest);
			if (NorthWest.Target != null)
				list.Add (NorthWest);
			if (North.Target != null)
				list.Add (North);
			if (East.Target != null)
				list.Add (East);
			if (South.Target != null)
				list.Add (South);
			if (West.Target != null)
				list.Add (West);

			return list;
		}
	}

	class Node {
		public Connections Neighbors = new Connections();
		public Coordinates Coordinates = new Coordinates ();
	}

	struct Coordinates {
		public int x;
		public int y;
	}

	class PriorityQueue<T> {
		private struct item {
			public T value;
			public double priority;
		}
		private List<item> data;

		public PriorityQueue() {
			data = new List<item>();
		}
		public void Push(T value, double priority) {
			item i = new item ();
			i.value = value;
			i.priority = priority;

			data.Add (i);
			data.Sort (delegate(item x, item y) {
				if (x.priority > y.priority) return 1;
				if (x.priority < y.priority) return -1;
				return 0;
			});
		}
		public T Peek() {
			return data [0].value;
		}
		public T Pop() {
			T returnValue = Peek ();
			data.RemoveAt (0);
			return returnValue;
		}
		public int Count {
			get {
				return data.Count;
			}
		}
	}

	class Path :IEnumerable<Node> {
		public Node LastStep { get; private set; }
		public Path PreviousSteps {get; private set;}
		public double TotalCost { get; private set; }
		private Path(Node lastStep, Path previousSteps, double totalCost) {
			LastStep = lastStep;
			PreviousSteps = previousSteps;
			TotalCost = totalCost;
		}
		public Path(Node start): this(start, null, 0) {}
		public Path AddStep(Node step, double stepCost) {
			return new Path (step, this, TotalCost + stepCost);
		}
		public bool Contains(Node n) {
			if (LastStep.Equals (n))
				return true;
			
			foreach (Node node in PreviousSteps) {
				if (node.Equals (n)) {
					return true;
				}
			}
			return false;
		}
		public bool Connects(Node a, Node b) {
			for (Path p = this; p != null; p = p.PreviousSteps) {
				if (p.PreviousSteps == null)
					return false;
				
				if ((p.LastStep.Equals (a) && p.PreviousSteps.LastStep.Equals (b)) || (p.LastStep.Equals (b) && p.PreviousSteps.LastStep.Equals (a)))
					return true;
			}
			return false;
		}
		public IEnumerator<Node> GetEnumerator() {
			for (Path p = this; p != null; p = p.PreviousSteps) {
				yield return p.LastStep;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator ();
		}
	}

	List<Path> GetPaths() {
		Node[,] grid = initializeGrid ();

		List<Node> positives = new List<Node> ();
		List<Node> negatives = new List<Node> ();

		int CurrentBit = 0;
		for (int i = 0; i < 8; i++) {
			CurrentBit = (int)Mathf.Pow(2, i);

			if ((CurrentBit & _North.positive) > 0) {
				positives.Add (grid [i, gridSize - 1]);
			}
			if ((CurrentBit & _North.negative) > 0) {
				negatives.Add (grid [i, gridSize - 1]);
			}
			if ((CurrentBit & _East.positive) > 0) {
				positives.Add (grid [gridSize - 1, i + 1]);
			}
			if ((CurrentBit & _East.negative) > 0) {
				negatives.Add (grid [gridSize - 1, i + 1]);
			}
			if ((CurrentBit & _South.positive) > 0) {
				positives.Add (grid [i + 1, 0]);
			}
			if ((CurrentBit & _South.negative) > 0) {
				negatives.Add (grid [i + 1, 0]);
			}
			if ((CurrentBit & _West.positive) > 0) {
				positives.Add (grid [0, i]);
			}
			if ((CurrentBit & _West.negative) > 0) {
				negatives.Add (grid [0, i]);
			}
		}

		List<Node> starts, ends;

		if (positives.Count > negatives.Count) {
			starts = positives;
			ends = negatives;
		} else {
			starts = negatives;
			ends = positives;
		}


		List<Node> unmatchedNodes = new List<Node> ();
		Path path = null;
		List<Path> paths =  new List<Path>();

		unmatchedNodes.AddRange(positives);
		unmatchedNodes.AddRange(negatives);

		foreach (Node start in starts) {
			ends.Sort (delegate(Node x, Node y) {
				double dx = distanceSqr(x, start);
				double dy = distanceSqr(y, start);

				if (dx < dy) return -1;
				if (dx > dy) return 1;
				return 0;
			});

			foreach (Node end in ends) {

				if (getSide (start) == getSide (end))
					continue;

				path = FindPath (start, end, paths);

				if (path != null) {
					paths.Add (path);
					unmatchedNodes.Remove(start);
					unmatchedNodes.Remove(end);
					break;
				}
			}
		}

		foreach (Node start in unmatchedNodes) {
			path = FindPath(start, null, paths);

			if (path != null)
				paths.Add(FindPath(start, path.LastStep, paths));
			else
				paths.Add(new Path(start));
		}

		return paths;
	}

	Path FindPath(Node start, Node end, List<Path> currentPaths) { // A* implementation
		HashSet<Node> closed = new HashSet<Node>();

		foreach (Path otherPath in currentPaths) {
			foreach (Node node in otherPath) {
				closed.Add (node);
			}
		}

		PriorityQueue<Path> queue = new PriorityQueue<Path> ();
		Path path = null;
		Path newPath;
		queue.Push (new Path(start), 0);
		bool intersect;

		while (queue.Count > 0) {
			path = queue.Pop ();
			if (closed.Contains (path.LastStep)) {
				continue;
			}


			if (end != null && path.LastStep.Equals(end)) {
				return path;
			}


			closed.Add (path.LastStep);

			foreach (Connection c in path.LastStep.Neighbors.toList()) {
				// is terminal but not end
				if ((end == null || c.Target != end) && getSide(c.Target) != -1) {
					closed.Add (c.Target);
					continue;
				}

				// is diagonal
				if (distanceSqr(c.Target, path.LastStep) > 1) {
					// get shared neighbors
					List<Node> intersection = new List<Node>();
					foreach (Connection a in c.Target.Neighbors.toList()) {
						foreach (Connection b in path.LastStep.Neighbors.toList()) {
							if (a.Target.Equals (b.Target))
								intersection.Add (a.Target);
						}
					}

					// check intersections
					intersect = false;
					foreach (Path p in currentPaths) {
						if (p.Connects(intersection[0], intersection[1])) {
							intersect = true;
							break;
						}
					}

					if (intersect) {
						continue;
					}
				}

				newPath = path.AddStep (c.Target, 1);

				if (end == null) {
					queue.Push (newPath, newPath.TotalCost + distanceSqr (c.Target, start));
					continue;
				}
				queue.Push (newPath, newPath.TotalCost + distanceSqr (c.Target, end));
			}
		}

		if (end == null) {
			return path.PreviousSteps;
		}
		return null;
	}

	int getSide(Node n) {
		if (n.Coordinates.x == 0)
			return 3;
		if (n.Coordinates.y == 0)
			return 2;
		if (n.Coordinates.x == gridSize - 1)
			return 1;
		if (n.Coordinates.y == gridSize - 1)
			return 0;

		return -1;
	}

	// Initialization functions
	Node[,] initializeGrid() {
		Node[,] grid = new Node[gridSize, gridSize];
		Node currentNode;
		Node Target;
		for (int y = 0; y < gridSize; y++) {
			for (int x = 0; x < gridSize; x++) {
				currentNode = new Node ();

				currentNode.Coordinates.x = x;
				currentNode.Coordinates.y = y;

				if (x != 0) {
					Target = grid [x - 1, y];
					Target.Neighbors.East.Target = currentNode;
					currentNode.Neighbors.West.Target = Target;
				}

				if (y != 0) {
					Target = grid [x, y - 1];
					currentNode.Neighbors.South.Target = Target;
					Target.Neighbors.North.Target = currentNode;
				}

				if (x > 0 && y > 0) {
					Target = grid [x - 1, y - 1];
					currentNode.Neighbors.SouthWest.Target = Target;
					Target.Neighbors.NorthEast.Target = currentNode;
				}

				if (x < gridSize - 1 && y > 0) {
					Target = grid [x + 1, y - 1];
					currentNode.Neighbors.SouthEast.Target = Target;
					Target.Neighbors.NorthWest.Target = currentNode;
				}

				grid [x, y] = currentNode;
			}
		}
		return grid;
	}
		
	// Utility functions

	void drawLine(Texture2D t, Vector2 a, Vector2 b, float width, Color color, float antiAliasing) {
		Vector2 v = a - b;

		int bottomLeftX = Mathf.FloorToInt(Mathf.Max(0, Mathf.Min(a.x - (width + antiAliasing), b.x - (width + antiAliasing))));
		int bottomLeftY = Mathf.FloorToInt(Mathf.Max(0, Mathf.Min(a.y - (width + antiAliasing), b.y - (width + antiAliasing))));
		int topRightX = Mathf.CeilToInt(Mathf.Min(t.width, Mathf.Max(a.x + (width + antiAliasing), b.x + (width + antiAliasing))));
		int topRightY = Mathf.CeilToInt(Mathf.Min(t.height, Mathf.Max(a.y + (width + antiAliasing), b.y + (width + antiAliasing))));

		float d = 0;
		float w = width / 2;

		for (int x = bottomLeftX; x < topRightX; x++) {
			for (int y = bottomLeftY; y < topRightY; y++) {
				v = new Vector2(x, y);

				d = distanceToSegment(a, b, v);

				if (d <= w) {
					t.SetPixel (x, y, color);
				} else if (d - w < antiAliasing) {
					t.SetPixel (x, y, Color.Lerp(color, t.GetPixel(x, y), (d - w) / antiAliasing));
				}
			}
		}
	}

	void drawPath(Texture2D t, Path p, float scale, float width, Color color, float antiAliasing) {
		Path n = p;

		while (n.PreviousSteps != null) {
			drawLine (
				t,
				new Vector2 (n.LastStep.Coordinates.x * scale + scale / 2, n.LastStep.Coordinates.y * scale + scale / 2),
				new Vector2 (n.PreviousSteps.LastStep.Coordinates.x * scale + scale / 2, n.PreviousSteps.LastStep.Coordinates.y * scale + scale / 2),
				width,
				color,
				antiAliasing
			);
			n = n.PreviousSteps;
		}

		if (getSide(p.LastStep) == -1 || p.PreviousSteps == null) {
			drawVia(t, p.LastStep, scale, width, width, color, antiAliasing);
		}
	}

	void drawTerminals(Texture2D t, float width, Color color, float antiAliasing) {
		float position = 0;
		for (int i = 0; i < 8; i++) {
			position = i * cellSize + cellSize / 2;

			if ((_North.positive & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (position, t.height), new Vector2 (position, t.height - cellSize / 2), width, color, antiAliasing);
			}
			if ((_East.positive & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (t.width, position + cellSize), new Vector2 (t.width - cellSize / 2, position + cellSize), width, color, antiAliasing);
			}
			if ((_South.positive & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (position + cellSize, 0), new Vector2 (position + cellSize, cellSize / 2), width, color, antiAliasing);
			}
			if ((_West.positive & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (0, position), new Vector2 (cellSize / 2, position), width, color, antiAliasing);
			}

			if ((_North.negative & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (position, t.height), new Vector2 (position, t.height - cellSize / 2), width, color, antiAliasing);
			}
			if ((_East.negative & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (t.width, position + cellSize), new Vector2 (t.width - cellSize / 2, position + cellSize), width, color, antiAliasing);
			}
			if ((_South.negative & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (position + cellSize, 0), new Vector2 (position + cellSize, cellSize / 2), width, color, antiAliasing);
			}
			if ((_West.negative & (byte)Mathf.Pow (2, i)) > 0) {
				drawLine (t, new Vector2 (0, position), new Vector2 (cellSize / 2, position), width, color, antiAliasing);
			}
		}

	}

	void drawVia(Texture2D t, Node n, float scale, float radius, float width, Color color, float antiAliasing) {
		Vector2 center = new Vector2 (n.Coordinates.x * scale + scale / 2, n.Coordinates.y * scale + scale / 2);

		float extraSpace = radius + width + antiAliasing;
		int bottomLeftX = Mathf.FloorToInt(Mathf.Max(0, center.x - extraSpace));
		int bottomLeftY = Mathf.FloorToInt(Mathf.Max(0, center.y - extraSpace));
		int topRightX = Mathf.CeilToInt(Mathf.Min(t.width, center.x + extraSpace));
		int topRightY = Mathf.CeilToInt(Mathf.Min(t.height, center.y + extraSpace));

		float d = 0;
		float w = width / 2;

		for (int x = bottomLeftX; x < topRightX; x++) {
			for (int y = bottomLeftY; y < topRightY; y++) {
				d = Mathf.Abs(Vector2.Distance(center, new Vector2(x, y)) - radius);

				if (d < w) {
					t.SetPixel(x, y, color);
				} else if(d - w < antiAliasing) {
					t.SetPixel(x, y, Color.Lerp(color, t.GetPixel(x, y), (d - w) / antiAliasing));
				}
			}
		}
	}

	float distanceToSegment(Vector2 a, Vector2 b, Vector2 point) {
		float segmentLength = Vector2.Distance (a, b);

		if (segmentLength == 0)
			return Vector2.Distance (a, point);

		// Consider the line extending the segment, parameterized as v + t (w - v).
		// We find projection of point p onto the line.
		// It falls where t = [(p-v) . (w-v)] / |w-v|^2

		float t = Vector2.Dot ((point - a), b - a) / (segmentLength * segmentLength);

		t = Mathf.Max (0, Mathf.Min (1, t));

		return Vector2.Distance (point, new Vector2 (a.x + t * (b.x - a.x), a.y + t * (b.y - a.y)));
	}

	double distanceSqr(Node a, Node b) {
		return Mathf.Pow (a.Coordinates.x - b.Coordinates.x, 2) + Mathf.Pow (a.Coordinates.y - b.Coordinates.y, 2);
	}

	public Texture2D GetTexture(
		ushort north,
		ushort east,
		ushort south,
		ushort west,
		int cellSize,
		Texture2D backgroundTexture,
		float lineWidth,
		Color lineColor,
		float AntiAliasing
	) {
		Texture2D t = new Texture2D(cellSize * gridSize, cellSize * gridSize);
		// Color[] background = backgroundTexture.GetPixels();

		_North.value = north;
		_East.value = east;
		_South.value = south;
		_West.value = west;

		
		return t;
	}
	public void nextTxt() {
		Material m = GetComponent<Renderer> ().material;
		Texture2D t = new Texture2D(cellSize * gridSize, cellSize * gridSize);

		_North.value = (ushort)Random.Range(0, ushort.MaxValue + 1);
		_South.value = (ushort)Random.Range(0, ushort.MaxValue + 1);
		_East.value = (ushort)Random.Range(0, ushort.MaxValue + 1);
		_West.value = (ushort)Random.Range(0, ushort.MaxValue + 1);

		drawTerminals (t, lineWidth, lineColor, antiAliasing);
		
		List<Path> paths = GetPaths ();

		foreach (Path p in paths) {
			drawPath (t, p, cellSize, lineWidth, lineColor, antiAliasing);
		}

		t.Apply ();

		t.filterMode = FilterMode.Bilinear;
		m.mainTexture = t;
	}

	void Start() {
		nextTxt ();
	}
	void Update() {
		if (Input.GetButtonDown ("Fire1")) {
			float t = Time.realtimeSinceStartup;
			nextTxt ();
			Debug.LogFormat("Total time: {0}", Time.realtimeSinceStartup - t);
		}
	}
}
