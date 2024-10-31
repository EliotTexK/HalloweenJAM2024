using Godot;
using System;

namespace COMEONANDSLAM {

public partial class Skeleton : GridObject {
    public const int SKELETON_MONEY_DROP = 50; // amount of money rewarded per kill
    AnimatedSprite2D Skull;
    ShaderMaterial TintMaterial;
    Timer HitTintTimer;
	public override void _Ready() {
        base._Ready();
        StaticGameInfo.Skeletons.Add(WeakRef(this));
        Skull = GetNode<AnimatedSprite2D>("Transform/Skull");
        if (Health <= 6) {
            Skull.Frame = 1;
        }
        // Gotta duplicate so the shader applies to different enemies differently
        TintMaterial = (ShaderMaterial)GetNode<CanvasGroup>("Transform").Material.Duplicate();
        GetNode<CanvasGroup>("Transform").Material = TintMaterial;
        HitTintTimer = new Timer();
        HitTintTimer.Timeout += OnHitTintTimeout;
        AddChild(HitTintTimer);
    }
    public void TakeDamage(int damage) {
        Health -= damage;
        if (Health <= 6) {
            Skull.Frame = 1;
        }
        if (Health <= 0) {
            StaticGameInfo.Money += SKELETON_MONEY_DROP;
            Level.SingletonInstance.HUD_Display.UpdateMoney();
            Level.SingletonInstance.NotifySkelDeath();
            this.QueueFree();
        }
        TintMaterial.SetShaderParameter("intensity",0.6f);
        HitTintTimer.Start(0.1);
    }
    public void OnHitTintTimeout() {
        TintMaterial.SetShaderParameter("intensity",0.0f);
    }
    public override void _ExitTree()
    {
        base._ExitTree();
    }
}

}