using Godot;
using System;

namespace COMEONANDSLAM {

public partial class HayBale : GridObject {
    ShaderMaterial TintMaterial;
    Timer HitTintTimer;
	public override void _Ready() {
        base._Ready();
        Health = 20;
        Level.SingletonInstance.HUD_Display.SetHealth(Health);
        // Gotta duplicate so the shader applies to different enemies differently
        TintMaterial = (ShaderMaterial)GetNode<CanvasGroup>("CanvasGroup").Material.Duplicate();
        GetNode<CanvasGroup>("CanvasGroup").Material = TintMaterial;
        HitTintTimer = new Timer();
        HitTintTimer.Timeout += OnHitTintTimeout;
        AddChild(HitTintTimer);
    }
    public void TakeDamage(int damage) {
        Health -= damage;
        Level.SingletonInstance.HUD_Display.SetHealth(Health);
        if (Health <= 0) {
            QueueFree();
        }
        TintMaterial.SetShaderParameter("intensity",0.6f);
        HitTintTimer.Start(0.1);
    }
    public void OnHitTintTimeout() {
        TintMaterial.SetShaderParameter("intensity",0.0f);
    }
}

}