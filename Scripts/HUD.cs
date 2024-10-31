using Godot;
using System;

namespace COMEONANDSLAM {

public partial class HUD : CanvasLayer {
    Control heartMask;
    Control milkMask;
    Label moneyLabel;
    public override void _Ready() {
        base._Ready();
        heartMask = GetNode<Control>("HeartMask");
        milkMask = GetNode<Control>("MilkMask");
        moneyLabel = GetNode<Label>("MoneyLabel");
        UpdateMoney();
    }
    public void SetHealth(int amount) {
        heartMask.SetSize(new Vector2(heartMask.Size.Y * amount/2,heartMask.Size.Y));
    }
    public void SetMilkHealth(int amount) {
        milkMask.SetSize(new Vector2(milkMask.Size.Y * amount/2,milkMask.Size.Y));
    }
    public void UpdateMoney() {
        moneyLabel.Text = StaticGameInfo.Money.ToString();
    }
}

}
