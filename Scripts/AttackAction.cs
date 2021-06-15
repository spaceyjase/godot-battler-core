using System.Threading.Tasks;
using battler.Scenes;
using Godot;
using Godot.Collections;

namespace battler.Scripts
{
  public class AttackAction : Action
  {
    private int[] hits;

    public AttackAction()
    {
    }

    public AttackAction(ActionData data, Battler actor, Battler[] targets) : base(data, actor, targets)
    {
    }

    private int CalculatePotentialDamageFor(Battler target)
    {
      return Formulas.Formulas.CalculateBaseDamage(data as AttackActionData, actor, target);
    }

    protected override async Task<bool> ApplyImpl()
    {
      var anim = actor.BattlerAnimation;
      foreach (var target in targets)
      {
        var hitChance = Formulas.Formulas.CalculateHitChance(data as AttackActionData, actor, target);
        var damage = CalculatePotentialDamageFor(target);
        var hit = new Hit(damage, hitChance);
        
        anim.Connect(nameof(BattlerAnim.Triggered), this, nameof(OnBattlerAnimTriggered), new Array{ target, hit });
        
        anim.Play("attack");
        await ToSignal(target, nameof(Battler.AnimationFinished));
      }
      
      await ToSignal(Engine.GetMainLoop(), "idle_frame");

      return await base.ApplyImpl();
    }

    private void OnBattlerAnimTriggered(Battler target, Hit hit)
    {
      target.TakeHit(hit);
    }
  }
}