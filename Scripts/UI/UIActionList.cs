using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Array = Godot.Collections.Array;

namespace battler.Scripts.UI
{
  public class UIActionList : VBoxContainer
  {
    [Signal] public delegate void ActionSelected(ActionData action);

    private PackedScene actionButtonScene;
    private UIMenuSelectArrow selectArrow;
    private bool isDisabled;
    private readonly List<UIActionButton> buttons = new List<UIActionButton>();

    public bool IsDisabled
    {
      get => isDisabled;
      set
      {
        isDisabled = value;
        foreach (var button in buttons)
        {
          button.Disabled = isDisabled;
        }
      }
    }

    public override void _Ready()
    {
      base._Ready();

      selectArrow = GetNode<UIMenuSelectArrow>("UIMenuSelectArrow");
      actionButtonScene = ResourceLoader.Load<PackedScene>($"{nameof(UIActionButton)}.tscn");
    }

    public void Setup(Battler battler)
    {
      foreach (var action in battler.Actions)
      {
        var canUseAction = battler.Stats.Energy >= action.EnergyCost;
        var actionButton = actionButtonScene.Instance<UIActionButton>();
        if (actionButton != null)
        {
          actionButton.Setup(action, canUseAction);
          actionButton.Connect("pressed", this, nameof(OnActionButtonPressed),
            new Array { action });
          actionButton.Connect("focus_entered", this, nameof(OnActionButtonFocusEntered),
            new Array { actionButton, battler.UIData.DisplayName, action.EnergyCost });
          AddChild(actionButton);
          buttons.Add(actionButton);
        }
        else
        {
          GD.PrintErr($"{nameof(actionButton)} is null");
        }
      }

      selectArrow.Position = buttons.First().RectGlobalPosition + 
                             new Vector2(0f, buttons.First().RectSize.y / 2f);
    }

    public void Focus()
    {
      buttons.First().GrabFocus();
    }

    private void OnActionButtonFocusEntered(Control button, string displayName, int energyCost)
    {
      selectArrow.MoveTo(button.RectGlobalPosition + new Vector2(0f, button.RectSize.y / 2f));
    }

    private void OnActionButtonPressed(ActionData action)
    {
      IsDisabled = true;
      EmitSignal(nameof(ActionSelected), action);
    }
  }
}
