using Godot;
using System;

namespace COMEONANDSLAM {

public partial class HayBale : GridObject
{
	public HayBale() {
        base._Ready();
        Health = 10;
        SetGridSpaceFromScreenSpace();
    }
}

}