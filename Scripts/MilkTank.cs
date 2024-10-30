using Godot;
using System;

namespace COMEONANDSLAM {

public partial class MilkTank : GridObject
{
	public MilkTank() {
        Health = 20;
    }
    public override void _Ready() {
        SetGridSpaceFromScreenSpace();
        // TODO: make sure nothing is in the same tile as this
	}
}

}