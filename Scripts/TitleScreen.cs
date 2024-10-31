using Godot;
using System;

public partial class TitleScreen : Node
{
	[Export] public PackedScene SceneToLoad; // Drag and drop the scene in the editor

	public override void _Ready()
	{
		// Connect the Start button's pressed signal to the method
		GetNode<Button>("CanvasLayer/CenterContainer/TitleText/Start").Pressed += OnStartButtonPressed;
		
		// Connect the Quit button's pressed signal to the quit method
		GetNode<Button>("CanvasLayer/CenterContainer/TitleText/Quit").Pressed += OnQuitButtonPressed;
	}

	private void OnStartButtonPressed()
	{
		if (SceneToLoad != null)
		{
			GetTree().ChangeSceneToPacked(SceneToLoad);
		}
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
