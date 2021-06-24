using battler.Scripts.Events;
using Godot;

namespace battler.Scripts.UI
{
  public class UIBattlerHUD : TextureRect
  {
    private UILifeBar lifeBar;
    private UIEnergyBar energyBar;
    private Label label;

    public override void _Ready()
    {
      base._Ready();

      lifeBar = GetNode<UILifeBar>("UILifeBar");
      energyBar = GetNode<UIEnergyBar>("UIEnergyBar");
      label = GetNode<Label>("Label");

      EventBus.Instance.Connect(nameof(EventBus.CombatActionHovered), this, 
        nameof(OnEvents_CombatActionHovered));
      EventBus.Instance.Connect(nameof(EventBus.PlayerTargetSelectionDone), this,
        nameof(OnEvents_PlayerTargetSelectionDone));
    }

    public void Setup(Battler battler)
    {
      label.Text = battler.UIData.DisplayName;

      var stats = battler.Stats;
      lifeBar.Setup(stats.Health, stats.MaxHealth);
      energyBar.Setup(stats.MaxEnergy, stats.Energy);

      stats.Connect(nameof(BattlerStats.HealthChanged), this, nameof(OnBattlerStats_HealthChanged));
      stats.Connect(nameof(BattlerStats.EnergyChanged), this, nameof(OnBattlerStats_EnergyChanged));
    }

    private void OnBattlerStats_HealthChanged(float oldValue, float newValue)
    {
      lifeBar.TargetValue = newValue;
    }

    private void OnBattlerStats_EnergyChanged(float oldValue, float newValue)
    {
      energyBar.Value = (int)newValue;
    }

    private void OnEvents_CombatActionHovered(string displayName, int energyCost)
    {
      if (label.Text == displayName)
      {
        energyBar.SelectedCount = energyCost;
      }
    }

    private void OnEvents_PlayerTargetSelectionDone()
    {
      energyBar.SelectedCount = 0;
    }
  }
}
