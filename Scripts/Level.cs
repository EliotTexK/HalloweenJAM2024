using Godot;
using System;
using System.Net;

namespace COMEONANDSLAM {

enum IsBuying {
    Nothing, Bale, Hound
}

public partial class Level : Node2D {
    public static PackedScene hayBaleScene;
    public static Level SingletonInstance = null;
    [Export]
    public int Width {get; set;} = 15;
    [Export]
    public int Height {get; set;} = 10;
    public HUD HUD_Display;
    IsBuying isBuying = IsBuying.Nothing;
    public Level() {
        hayBaleScene = (PackedScene)ResourceLoader.Load("res://Scenes/HayBale.tscn");
        if (hayBaleScene == null) {
            throw new Exception("Hay bale scene couldn't be loaded by level");
        }
        StaticGameInfo.InitLevel(Width, Height);
        if (SingletonInstance == null) {
            SingletonInstance = this;
        }
    }
    public override void _EnterTree() {
        base._EnterTree();
        HUD_Display = GetNode<HUD>("HUD");
    }
    public override void _Ready() {
        base._Ready();
        StaticGameInfo.ComputeSkeletonPath();
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (isBuying == IsBuying.Nothing) {
            if (Input.IsActionJustPressed("ui_up")) {
                StaticGameInfo.UpdateAllObjects(PlayerAction.MoveUp, Vector2I.Zero);
            }
            else if (Input.IsActionJustPressed("ui_down")) {
                StaticGameInfo.UpdateAllObjects(PlayerAction.MoveDown, Vector2I.Zero);
            }
            else if (Input.IsActionJustPressed("ui_left")) {
                StaticGameInfo.UpdateAllObjects(PlayerAction.MoveLeft, Vector2I.Zero);
            }
            else if (Input.IsActionJustPressed("ui_right")) {
                StaticGameInfo.UpdateAllObjects(PlayerAction.MoveRight, Vector2I.Zero);
            }
            else if (Input.IsActionJustPressed("buy_bale")) {
                isBuying = IsBuying.Bale;
            }
            else if (Input.IsActionJustPressed("buy_hound")) {
                isBuying = IsBuying.Hound;
            }
        }
        else {
            Vector2I? relativeTarget = null;
            if (Input.IsActionJustPressed("ui_up")) {
                relativeTarget = Vector2I.Up;
            }
            else if (Input.IsActionJustPressed("ui_down")) {
                relativeTarget = Vector2I.Down;
            }
            else if (Input.IsActionJustPressed("ui_left")) {
                relativeTarget = Vector2I.Left;
            }
            else if (Input.IsActionJustPressed("ui_right")) {
                relativeTarget = Vector2I.Right;
            }
            if (relativeTarget != null) {
                if (isBuying == IsBuying.Bale) {
                    StaticGameInfo.UpdateAllObjects(PlayerAction.PlaceBale,relativeTarget.Value);
                    HUD_Display.UpdateMoney();
                }
                isBuying = IsBuying.Nothing;
            }
        }
    }

    public void AddBale(Vector2I gridPos) {
        HayBale bale = (HayBale)hayBaleScene.Instantiate();
        bale.GridPos = gridPos;
        bale.SetScreenSpaceFromGridSpace();
        AddChild(bale);
    }
}

}