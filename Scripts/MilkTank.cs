using Godot;
using System;

namespace COMEONANDSLAM {

public partial class MilkTank : GridObject {
    ShaderMaterial TintMaterial;
    Timer HitTintTimer;
	public override void _Ready() {
        base._Ready();
        Health = 20;
        Level.SingletonInstance.HUD_Display.SetMilkHealth(Health);
        StaticGameInfo.MilkLocation = GridPos;
    }
    public void TakeDamage(int damage) {
        Health -= damage;
        Level.SingletonInstance.HUD_Display.SetMilkHealth(Health);
        if (Health <= 0) {
            //Node2D newInstance = (Node2D)SpawnOnDeath.Instantiate();
            // GetParent().AddChild(newInstance);
            // newInstance.GlobalPosition = GlobalPosition;
            // QueueFree();
        }
        TintMaterial.SetShaderParameter("intensity",0.6f);
        HitTintTimer.Start(0.1);
    }
    public void OnHitTintTimeout() {
        TintMaterial.SetShaderParameter("intensity",0.0f);
    }
}

}