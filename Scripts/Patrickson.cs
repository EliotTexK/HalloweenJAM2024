using Godot;
using System;

namespace COMEONANDSLAM {

public partial class Patrickson : GridObject {
    [Export] public PackedScene SpawnOnDeath;
    ShaderMaterial TintMaterial;
    Timer HitTintTimer;
	public override void _Ready() {
        base._Ready();
        Health = 20;
        Level.SingletonInstance.HUD_Display.SetHealth(Health);
        StaticGameInfo.Player = WeakRef(this);
        // Gotta duplicate so the shader applies to different enemies differently
        TintMaterial = (ShaderMaterial)GetNode<CanvasGroup>("Transform").Material.Duplicate();
        GetNode<CanvasGroup>("Transform").Material = TintMaterial;
        HitTintTimer = new Timer();
        HitTintTimer.Timeout += OnHitTintTimeout;
        AddChild(HitTintTimer);
    }
    public void TakeDamage(int damage) {
        Health -= damage;
        Level.SingletonInstance.HUD_Display.SetHealth(Health);
        if (Health <= 0) {
            Node2D newInstance = (Node2D)SpawnOnDeath.Instantiate();
            GetParent().AddChild(newInstance);
            Level.SingletonInstance.HealthDeath();
            newInstance.GlobalPosition = GlobalPosition;
            StaticGameInfo.LoseCondition = true;
            StaticGameInfo.Player = null;
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