using Godot;
using System;
using System.Collections.Generic;

namespace COMEONANDSLAM {

public enum SkeletonPathNode {
	N,S,E,W,
	Target,
	Unexplored,
	Wall
}

public static partial class StaticGameInfo {
	public const int TILE_LENGTH = 750;
	public static WeakRef[,] Grid {get; set;}
	public static WeakRef Patrickson;
	// TODO: If we have time, consider sorting skeletons by distance to milk tanks,
	//       so that they don't collide with each other and congest. But hey, maybe
	//       congestion could be a cool game mechanic, so maybe don't sort.
	public static HashSet<WeakRef> Skeletons;
	public static HashSet<WeakRef> Hounds;
	public static HashSet<WeakRef> HerbicideDispensers;
	public static HashSet<WeakRef> HayBales; // may not be needed, since bales don't really move on their own
	public static Vector2I MilkLocation;
	public static SkeletonPathNode[,] SkeletonPath {get; set;}
	public static void Init(int width, int height) {
		Grid = new WeakRef[width, height];
		SkeletonPath = new SkeletonPathNode[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				SkeletonPath[x,y] = SkeletonPathNode.Unexplored;
			}
		}
		Skeletons = new HashSet<WeakRef>();
		Hounds = new HashSet<WeakRef>();
		HerbicideDispensers = new HashSet<WeakRef>();
		HayBales = new HashSet<WeakRef>();
	}
	public static GridObject QueryMap(Vector2I pos) {
		var wr = Grid[pos.X,pos.Y];
		if (wr == null) return null;
		var gr = wr.GetRef();
		var go = (GridObject)gr;
		return go;
	}
	public static bool IsInGridBounds(Vector2I pos) {
		return pos.X >= 0 && pos.Y >= 0 && pos.X < Grid.GetLength(0) && pos.Y < Grid.GetLength(1);
	}
	// Use BFS to find the shortest path for skeletons.
	// TODO: Potentially use Dijkstra's to incorporate the cost of destroying objects.
	// TODO: Add random weights so that different correct paths can be formed.
	public static void ComputeSkeletonPath() {
		Queue<Vector2I> frontier = new Queue<Vector2I>();
		frontier.Enqueue(MilkLocation);
		SkeletonPath[MilkLocation.X, MilkLocation.Y] = SkeletonPathNode.Target;
		while (frontier.Count > 0) {
			Vector2I front = frontier.Dequeue();
			var neighbors = new Dictionary<Vector2I, SkeletonPathNode> {
				{new Vector2I(front.X,front.Y-1), SkeletonPathNode.S},
				{new Vector2I(front.X,front.Y+1), SkeletonPathNode.N},
				{new Vector2I(front.X+1,front.Y), SkeletonPathNode.W},
				{new Vector2I(front.X-1,front.Y), SkeletonPathNode.E}
			};
			foreach (var neighbor in neighbors) {
				int nbrX = neighbor.Key.X; int nbrY = neighbor.Key.Y;
				if (IsInGridBounds(neighbor.Key) && SkeletonPath[nbrX,nbrY] == SkeletonPathNode.Unexplored) {
					var qm = QueryMap(neighbor.Key);
					if (qm is HayBale) {
						SkeletonPath[neighbor.Key.X,neighbor.Key.Y] = SkeletonPathNode.Wall;
					} else {
						frontier.Enqueue(neighbor.Key);
						SkeletonPath[neighbor.Key.X,neighbor.Key.Y] = neighbor.Value;
					}
				}
			}
		}
	}
	public static void UpdateAllObjects(/* Player Input Goes Here */) {
		// Hook this up to a callback to whatever UI is being used to select player actions
		// Call this once the player has selected their input, update all objects
		// The Game UI should "react" to the state changes (e.g. smoothly interpolate position changes, play animations),
		// which should happen in the _Process function, not as a result of calling UpdateAllObjects

		// Skelebros
		foreach (WeakRef skel_ref in Skeletons) {
			// Should be guaranteed not null
			var skel = (Skeleton)skel_ref.GetRef();
			var dest = skel.GridPos;
			if (IsInGridBounds(skel.GridPos)) {
				switch (SkeletonPath[skel.GridPos.X, skel.GridPos.Y]) {
					case SkeletonPathNode.N:
						dest = skel.GridPos + Vector2I.Up;
						break;
					case SkeletonPathNode.S:
						dest = skel.GridPos + Vector2I.Down;
						break;
					case SkeletonPathNode.E:
						dest = skel.GridPos + Vector2I.Right;
						break;
					case SkeletonPathNode.W:
						dest = skel.GridPos + Vector2I.Left;
						break;
				}
			}
			if (IsInGridBounds(dest)) {
				var atDest = QueryMap(dest);
				if (atDest == null) {
					skel.MoveOnGrid(dest);
				}
			}
		}
	}
}

}
