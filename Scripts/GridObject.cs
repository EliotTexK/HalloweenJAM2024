using Godot;
using System;

namespace COMEONANDSLAM {

public partial class GridObject : Node2D
{
	public Vector2I GridPos;
	public int Health;
	public override void _Ready() {
	}
	public override void _Process(double delta) {
		float moveSpeed = 5.0f;  // Controls the speed of motion
		float easing = 0.05f;    // Controls the curve's shape (smaller values make it start slower)
		
		// Grid space converted to world space
		Vector2 DesiredPos = GridPos * StaticGameInfo.TILE_LENGTH;

		// Calculate distance and apply easing
		float distance = Position.DistanceTo(DesiredPos);
		float easeFactor = 1.0f / (1.0f + Mathf.Exp(-moveSpeed * (distance - easing)));

		// Move towards target
		Position = Position.Lerp(DesiredPos, easeFactor * (float)delta);
	}
	// Align from screen space to grid space
	public void SetGridSpaceFromScreenSpace() {
		GridPos.X = (int)(Position.X / StaticGameInfo.TILE_LENGTH);
		GridPos.Y = (int)(Position.Y / StaticGameInfo.TILE_LENGTH);
	}
}

}
