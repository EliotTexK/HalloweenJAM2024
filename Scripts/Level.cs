using Godot;
using System;
using System.Net;

namespace COMEONANDSLAM {

enum IsBuying {
    Nothing, Bale, Hound
}

public partial class Level : Node2D {
    public static PackedScene hayBaleScene;
    public static PackedScene houndScene;
    public static PackedScene healthDeathScene;
    public static PackedScene milkDeathScene;
    public static PackedScene victoryScene;
    public static Level SingletonInstance = null;
    [Export]
    public int startingMoney;
    [Export]
    public PackedScene NextLevel;
    [Export]
    public int Width {get; set;} = 15;
    [Export]
    public int Height {get; set;} = 10;
    public HUD HUD_Display;
    public int SkeletonsLeft;
    IsBuying isBuying = IsBuying.Nothing;
    public Level() {
        hayBaleScene = (PackedScene)ResourceLoader.Load("res://Scenes/HayBale.tscn");
        houndScene = (PackedScene)ResourceLoader.Load("res://Scenes/Hound.tscn");
        healthDeathScene = (PackedScene)ResourceLoader.Load("res://Scenes/HealthDeath.tscn");
        milkDeathScene = (PackedScene)ResourceLoader.Load("res://Scenes/MilkDeath.tscn");
        victoryScene = (PackedScene)ResourceLoader.Load("res://Scenes/WinScreen.tscn");
        if (hayBaleScene == null) {
            throw new Exception("Hay bale scene couldn't be loaded by level");
        }
        StaticGameInfo.InitLevel(Width, Height);
        SingletonInstance = this;
    }
    public override void _EnterTree() {
        base._EnterTree();
        HUD_Display = GetNode<HUD>("HUD");
        StaticGameInfo.Money = startingMoney;
    }
    public override void _Ready() {
        base._Ready();
        StaticGameInfo.ComputeSkeletonPath();
        SkeletonsLeft = StaticGameInfo.Skeletons.Count;
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
            else if (Input.IsActionJustPressed("pass_turn")) {
                StaticGameInfo.UpdateAllObjects(PlayerAction.PassTurn, Vector2I.Zero);
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
                if (isBuying == IsBuying.Hound) {
                    StaticGameInfo.UpdateAllObjects(PlayerAction.PlaceHound,relativeTarget.Value);
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

    public void AddHound(Vector2I gridPos) {
        Hound hound = (Hound)houndScene.Instantiate();
        hound.GridPos = gridPos;
        hound.SetScreenSpaceFromGridSpace();
        AddChild(hound);
    }

    public void HealthDeath() {
        DeathScreen hd = (DeathScreen)healthDeathScene.Instantiate();
        AddChild(hd);
    }

    public void MilkDeath() {
        DeathScreen md = (DeathScreen)milkDeathScene.Instantiate();
        AddChild(md);
    }

    public void NotifySkelDeath(){
        SkeletonsLeft -= 1;
        if (SkeletonsLeft <= 0) {
            Slide v = (Slide)victoryScene.Instantiate();
            v.Next = NextLevel;
            AddChild(v);
        }
    }
}

}