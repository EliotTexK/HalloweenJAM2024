using Godot;
using System;

namespace COMEONANDSLAM {

public partial class MilkTank : GridObject
{
	public MilkTank() {
        Health = 20;
        SetGridSpaceFromScreenSpace();
    }
}

}