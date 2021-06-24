using System.Collections.Generic;
using Godot;
using battler.Scripts.Events;
using Array = Godot.Collections.Array;

namespace battler.Scripts.UI
{
  public class UIActionList : VBoxContainer
  {
    [Signal] public delegate void ActionSelected(ActionData action);

    private readonly PackedScene actionButtonScene = ResourceLoader.Load<PackedScene>(
      $"res://Scenes/UI/{nameof(UIActionButton)}.tscn");
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
    }

    public void Setup(Battler battler)
    {
      foreach (var action in battler.Actions)
      {
        var canUseAction = battler.Stats.Energy >= action.EnergyCost;
        var actionButton = actionButtonScene.Instance<UIActionButton>();
        
        buttons.Add(actionButton);
        AddChild(actionButton);
        actionButton.Setup(action, canUseAction);
          
        actionButton.Connect("pressed", this, nameof(OnActionButtonPressed),
          new Array { action });
          
        actionButton.Connect("focus_entered", this, nameof(OnActionButtonFocusEntered),
          new Array { actionButton, battler.UIData.DisplayName, action.EnergyCost });
      }

      selectArrow.Position = buttons[0].RectGlobalPosition +
                             new Vector2(0f, buttons[0].RectSize.y / 2f);
    }

    public void Focus()
    {
      buttons[0].GrabFocus();
    }

    private void OnActionButtonFocusEntered(Control button, string displayName, int energyCost)
    {
      selectArrow.MoveTo(button.RectGlobalPosition + new Vector2(0f, button.RectSize.y / 2f));
      EventBus.Instance.EmitSignal(
        nameof(EventBus.CombatActionHovered), displayName, energyCost);
    }

    private void OnActionButtonPressed(ActionData action)
    {
      IsDisabled = true;
      EmitSignal(nameof(ActionSelected), action);
    }
  }
}
