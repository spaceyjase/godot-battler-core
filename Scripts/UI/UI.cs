using Godot;

namespace battler.Scripts.UI
{
  public class UI : CanvasLayer
  {
    private readonly PackedScene uiCombatResultsPanel = ResourceLoader.Load<PackedScene>(
      $"res://Scenes/UI/{nameof(UICombatResultPanel)}.tscn");

    public void OnCombarDemo_CombatEnded(string message)
    {
      var widget = uiCombatResultsPanel.Instance<UICombatResultPanel>();
      widget.Text = message;
      AddChild(widget);
    }
  }
}
