using System.Linq;
using Godot;

namespace battler.Scripts.UI
{
  public class UIEnergyBar : HBoxContainer
  {
    private readonly PackedScene uiEnergyPoint = ResourceLoader.Load<PackedScene>(
      $"res://Scenes/UI/{nameof(UIEnergyPoint)}.tscn");

    private int maxValue;
    private int value;
    private int selectedCount;

    public int SelectedCount
    {
      get => selectedCount;
      set
      {
        var oldValue = selectedCount;
        selectedCount = Mathf.Clamp(value, 0, maxValue);
        if (selectedCount > oldValue)
        {
          foreach (var i in Enumerable.Range(oldValue, selectedCount))
          {
            GetChild<UIEnergyPoint>(i).Select();
          }
        }
        else
        {
          foreach (var i in Enumerable.Range(selectedCount, oldValue - selectedCount))
          {
            GetChild<UIEnergyPoint>(i).Deselect();
          }
        }
      }
    }

    public int Value
    {
      get => value;
      set
      {
        var oldValue = this.value;
        this.value = Mathf.Clamp(value, 0, maxValue);
        if (this.value > oldValue)
        {
          foreach (var i in Enumerable.Range(oldValue, this.value - oldValue))
          {
            GetChild<UIEnergyPoint>(i).Appear();
          }
        }
        else
        {
          foreach (var i in Enumerable.Range(this.value, oldValue - this.value))
          {
            GetChild<UIEnergyPoint>(i).Disappear();
          }
        }
      }
    }

    public void Setup(int maxEnergy, int energy)
    {
      maxValue = maxEnergy;
      value = energy;
      foreach (var unused in Enumerable.Range(0, maxValue))
      {
        var energyPoint = uiEnergyPoint.Instance<UIEnergyPoint>();
        AddChild(energyPoint);
      }
    }
  }
}
