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
        TintMaterial = (ShaderMaterial)GetNode<CanvasGroup>("CanvasGroup").Material.Duplicate();
        HitTintTimer = new Timer();
        HitTintTimer.Timeout += OnHitTintTimeout;
        AddChild(HitTintTimer);
    }
    public void TakeDamage(int damage) {
        Health -= damage;
        Level.SingletonInstance.HUD_Display.SetMilkHealth(Health);
        if (Health <= 0) {
            Level.SingletonInstance.MilkDeath();
            StaticGameInfo.LoseCondition = true;
        }
        TintMaterial.SetShaderParameter("intensity",0.6f);
        HitTintTimer.Start(0.1);
    }
    public void OnHitTintTimeout() {
        TintMaterial.SetShaderParameter("intensity",0.0f);
    }
}

}