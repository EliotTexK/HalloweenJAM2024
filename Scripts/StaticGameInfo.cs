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
	public static WeakRef[] Skeletons;
	public static WeakRef[] Hounds;
	public static WeakRef[] HerbicideDispensers;
	public static WeakRef[] HayBales; // may not be needed, since bales don't really move on their own
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
		ComputeSkeletonPath();
	}
	public static GridObject QueryMap(Vector2I pos) {
		return (GridObject)Grid[pos.X, pos.Y].GetRef().AsGodotObject();
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
					if (QueryMap(neighbor.Key) is HayBale) {
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
	}
}

}
