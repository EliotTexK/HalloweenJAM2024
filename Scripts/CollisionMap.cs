using Godot;
using System;

namespace COMEONANDSLAM {

public static partial class StaticCollisionMap {
	public static WeakRef[,] Map {get; set;}
	public static void Init(int width, int height) {
		Map = new WeakRef[width, height];
	}
}

}