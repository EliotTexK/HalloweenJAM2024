using Godot;
using System;

namespace COMEONANDSLAM {

public partial class GridObject : Node2D
{
	public Vector2I GridPos;
	public int Health;
	public override void _Ready(){
		base._Ready();
		SetGridSpaceFromScreenSpace();
		StaticGameInfo.Grid[GridPos.X, GridPos.Y] = WeakRef(this);
	}
	public override void _Process(double delta) {
		// Grid space converted to world space
		Vector2 DesiredPos = GridPos * StaticGameInfo.TILE_LENGTH;

		// Move towards target
		GlobalPosition = GlobalPosition.Lerp(DesiredPos, (float)delta * 10f);
	}
	// Align from screen space to grid space
	public void SetGridSpaceFromScreenSpace() {
		GridPos.X = (int)(Position.X / StaticGameInfo.TILE_LENGTH);
		GridPos.Y = (int)(Position.Y / StaticGameInfo.TILE_LENGTH);
	}

	public void SetScreenSpaceFromGridSpace() {
		GlobalPosition = GridPos * StaticGameInfo.TILE_LENGTH;
	}
	public override void _ExitTree() {
		StaticGameInfo.Grid[GridPos.X, GridPos.Y] = null;
		base._ExitTree();
    }
	public void MoveOnGrid(Vector2I pos) {
		StaticGameInfo.Grid[GridPos.X, GridPos.Y] = null;
		GridPos = pos;
		StaticGameInfo.Grid[GridPos.X, GridPos.Y] = WeakRef(this);
	}

}

}
