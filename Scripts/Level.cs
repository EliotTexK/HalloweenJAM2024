using Godot;
using System;
using System.Net;

namespace COMEONANDSLAM {


public partial class Level : Node2D {
    [Export]
    public int Width {get; set;} = 15;
    [Export]
    public int Height {get; set;} = 10;
    public Level() {
        StaticGameInfo.Init(Width, Height);
    }
    public override void _Ready() {
        StaticGameInfo.ComputeSkeletonPath();
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Input.IsActionJustPressed("ui_up")) {
            StaticGameInfo.UpdateAllObjects();
        }
    }
}

}