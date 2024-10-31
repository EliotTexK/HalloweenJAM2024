using Godot;
using System;

namespace COMEONANDSLAM {

public partial class Skeleton : GridObject
{
	public override void _Ready() {
        base._Ready();
        Health = 20;
        StaticGameInfo.Skeletons.Add(WeakRef(this));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        StaticGameInfo.Skeletons.Remove(WeakRef(this));
    }
}

}