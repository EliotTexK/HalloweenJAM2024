using Godot;
using System;
using System.Net;

namespace COMEONANDSLAM {

public partial class Level : Node2D {
    [Export]
    public int Width {get; set;} = 15;
    public int Height {get; set;} = 10;
    public override void _Ready() {
        StaticCollisionMap.Init(Width, Height);
    }
}

}