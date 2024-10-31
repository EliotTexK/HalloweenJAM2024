using Godot;
using System;

public partial class DeathScreen : Node
{
    [Export] public NodePath RestartButtonPath;

	public override void _Ready()
	{
		// Connect the Start button's pressed signal to the method
		GetNode<Button>(RestartButtonPath).Pressed += OnRestartPressed;
	}

	private void OnRestartPressed()
	{
        GetTree().ReloadCurrentScene();
	}
}
