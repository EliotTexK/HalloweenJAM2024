using Godot;
using System;

namespace COMEONANDSLAM {

public partial class HayBale : GridObject {
	public override void _Ready() {
        base._Ready();
        Health = 10;
        SetGridSpaceFromScreenSpace();
    }
}

}