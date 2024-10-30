using Godot;
using System;

namespace COMEONANDSLAM {

public partial class HayBale : GridObject
{
	public HayBale() {
        Health = 10;
    }
    public override void _Ready() {
        SetGridSpaceFromScreenSpace();
	}
}

}