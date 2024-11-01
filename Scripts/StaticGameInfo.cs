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


public enum PlayerAction {
	MoveUp, MoveDown, MoveLeft, MoveRight,
	Shoot, PlaceBale, PlaceHound
}

public static partial class StaticGameInfo {
	public const int TILE_LENGTH = 750;
	public static WeakRef[,] Grid {get; set;}
	public static WeakRef Player;
	public static int Money = 1000;
	public static bool LoseCondition = false;
	// TODO: If we have time, consider sorting skeletons by distance to milk tanks,
	//       so that they don't collide with each other and congest. But hey, maybe
	//       congestion could be a cool game mechanic, so maybe don't sort.
	public static HashSet<WeakRef> Skeletons;
	public static HashSet<WeakRef> Hounds;
	public static HashSet<WeakRef> HerbicideDispensers;
	public static HashSet<WeakRef> HayBales; // may not be needed, since bales don't really move on their own
	public static Vector2I MilkLocation;
	public static SkeletonPathNode[,] SkeletonPath {get; set;}
	public static void InitLevel(int width, int height) {
		Grid = new WeakRef[width, height];
		SkeletonPath = new SkeletonPathNode[width, height];
		Skeletons = new HashSet<WeakRef>();
		Hounds = new HashSet<WeakRef>();
		HerbicideDispensers = new HashSet<WeakRef>();
		HayBales = new HashSet<WeakRef>();
		LoseCondition = false;
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
		for (int x = 0; x < SkeletonPath.GetLength(0); x++) {
			for (int y = 0; y < SkeletonPath.GetLength(1); y++) {
				SkeletonPath[x,y] = SkeletonPathNode.Unexplored;
			}
		}
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
	public static SkeletonPathNode[,] ComputeProspectiveSkeletonPath(Vector2I new_wall) {
		SkeletonPathNode[,] prospective = new SkeletonPathNode[SkeletonPath.GetLength(0), SkeletonPath.GetLength(1)];
		Queue<Vector2I> frontier = new Queue<Vector2I>();
		frontier.Enqueue(MilkLocation);
		for (int x = 0; x < prospective.GetLength(0); x++) {
			for (int y = 0; y < prospective.GetLength(1); y++) {
				prospective[x,y] = SkeletonPathNode.Unexplored;
			}
		}
		prospective[MilkLocation.X, MilkLocation.Y] = SkeletonPathNode.Target;
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
				if (IsInGridBounds(neighbor.Key) && prospective[nbrX,nbrY] == SkeletonPathNode.Unexplored) {
					var qm = QueryMap(neighbor.Key);
					if (qm is HayBale || neighbor.Key == new_wall) {
						prospective[neighbor.Key.X,neighbor.Key.Y] = SkeletonPathNode.Wall;
					} else {
						frontier.Enqueue(neighbor.Key);
						prospective[neighbor.Key.X,neighbor.Key.Y] = neighbor.Value;
					}
				}
			}
		}
		return prospective;
	}
	public static bool CheckIfBlocked(Vector2I new_wall) {
		var prospective = ComputeProspectiveSkeletonPath(new_wall);
		foreach (WeakRef skel_ref in Skeletons) {
			var skel = (Skeleton)skel_ref.GetRef();
			if (skel == null) {
				Skeletons.Remove(skel_ref);
				continue;
			}
			if (prospective[skel.GridPos.X,skel.GridPos.Y] == SkeletonPathNode.Unexplored) {
				return true;
			}
		}
		return false;
	}
	public static void UpdateAllObjects(PlayerAction playerAction, Vector2I relativeTarget) {
		// Hook this up to a callback to whatever UI is being used to select player actions
		// Call this once the player has selected their input, update all objects
		// The Game UI should "react" to the state changes (e.g. smoothly interpolate position changes, play animations),
		// which should happen in the _Process function, not as a result of calling UpdateAllObjects
		if (LoseCondition) return;

		// Player
		if (Player != null) {
			var player = (Patrickson)Player.GetRef();
			var player_dest = player.GridPos;
			switch (playerAction) {
				case PlayerAction.MoveUp:
					player_dest = player.GridPos + Vector2I.Up;
					break;
				case PlayerAction.MoveDown:
					player_dest = player.GridPos + Vector2I.Down;
					break;
				case PlayerAction.MoveRight:
					player_dest = player.GridPos + Vector2I.Right;
					break;
				case PlayerAction.MoveLeft:
					player_dest = player.GridPos + Vector2I.Left;
					break;
				case PlayerAction.PlaceBale:
					if (Money < 80) return; // not enough money don't take turn
					if (QueryMap(player.GridPos + relativeTarget) != null) return; // something is in the way, don't take turn
					var new_bale_pos = player.GridPos + relativeTarget;
					if (CheckIfBlocked(new_bale_pos)) return;
					Money -= 80;
					Level.SingletonInstance.AddBale(new_bale_pos);
					ComputeSkeletonPath();
					break;
				case PlayerAction.PlaceHound:
					if (Money < 500) return; // not enough money don't take turn
					if (QueryMap(player.GridPos + relativeTarget) != null) return; // something is in the way, don't take turn
					Money -= 500;
					Level.SingletonInstance.AddHound(player.GridPos + relativeTarget);
					break;
			}
			if (IsInGridBounds(player_dest)) {
				var atDest = QueryMap(player_dest);
				if (atDest == null) {
					player.MoveOnGrid(player_dest);
				}
				else if (atDest is Skeleton) {
					var skel = (Skeleton)atDest;
					skel.TakeDamage(2);
				}
				else if (atDest is Patrickson) {
					// do nothing
				}
				else {
					return; // don't take turn, can't move there
				}
			}
		}

		foreach (WeakRef hound_ref in Hounds) {
			var hound = (Hound)hound_ref.GetRef();
			if (hound == null) {
				Skeletons.Remove(hound_ref);
				continue;
			}
			var pos = hound.GridPos;
			var neighbors = new Dictionary<Vector2I, SkeletonPathNode> {
				{new Vector2I(pos.X,pos.Y-1), SkeletonPathNode.S},
				{new Vector2I(pos.X,pos.Y+1), SkeletonPathNode.N},
				{new Vector2I(pos.X+1,pos.Y), SkeletonPathNode.W},
				{new Vector2I(pos.X-1,pos.Y), SkeletonPathNode.E}
			};
			foreach (Vector2I nbr in neighbors.Keys) {
				if (IsInGridBounds(nbr)) {
					var atNbr = QueryMap(nbr);
					if (atNbr is Skeleton) {
						var skel = (Skeleton)atNbr;
						skel.TakeDamage(3);
					}
				}
			}
		}

		// Skelebros
		foreach (WeakRef skel_ref in Skeletons) {
			var skel = (Skeleton)skel_ref.GetRef();
			if (skel == null) {
				Skeletons.Remove(skel_ref);
				continue;
			}
			var dest = skel.GridPos;
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
			if (IsInGridBounds(dest)) {
				var atDest = QueryMap(dest);
				if (atDest == null) {
					skel.MoveOnGrid(dest);
				}
				else if (atDest is Patrickson) {
					var player = (Patrickson)atDest;
					player.TakeDamage(2);
				}
				else if (atDest is Hound) {
					var hound = (Hound)atDest;
					hound.TakeDamage(2);
				}
				else if (atDest is MilkTank) {
					var milk = (MilkTank)atDest;
					milk.TakeDamage(2);
				}
			}
		}
	}
}

}
