using Godot;
using System;

namespace COMEONANDSLAM {

public partial class MilkTank : GridObject
{
	public override void _Ready() {
        base._Ready();
        Health = 20;
        StaticGameInfo.MilkLocation = GridPos;
    }
}

}