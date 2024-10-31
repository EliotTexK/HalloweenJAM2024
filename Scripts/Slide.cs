using Godot;
using System;

public partial class Slide : Node
{
	[Export] public PackedScene Next; // Drag and drop the scene in the editor
    [Export] public NodePath NextButtonPath;

	public override void _Ready()
	{
		// Connect the Start button's pressed signal to the method
		GetNode<Button>(NextButtonPath).Pressed += OnNextPressed;
	}

	private void OnNextPressed()
	{
		if (Next != null)
		{
			GetTree().ChangeSceneToPacked(Next);
		}
	}
}
