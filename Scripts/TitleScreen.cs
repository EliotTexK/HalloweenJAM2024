using Godot;
using System;

public class StartButton : Control
{
	[Export] public PackedScene SceneToLoad; // Drag and drop the scene in the editor

	public override void _Ready()
	{
		// Connect the Start button's pressed signal to the method
		GetNode<Button>("StartButton").Connect("pressed", this, nameof(OnStartButtonPressed));
		
		// Connect the Quit button's pressed signal to the quit method
		GetNode<Button>("QuitButton").Connect("pressed", this, nameof(OnQuitButtonPressed));
	}

	private void OnStartButtonPressed()
	{
		if (SceneToLoad != null)
		{
			GetTree().ChangeSceneTo(SceneToLoad);
		}
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
