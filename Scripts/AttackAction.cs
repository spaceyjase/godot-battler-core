﻿using System.Threading.Tasks;
using battler.Scripts.Formulas;
using Godot;

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
    return Formulas.CalculateBaseDamage(data as AttackActionData, actor, target);
  }

  protected override async Task<bool> ApplyImpl()
  {
    foreach (var target in targets)
    {
      var hitChance = Formulas.CalculateHitChance(data as AttackActionData, actor, target);
      var damage = CalculatePotentialDamageFor(target);
      var hit = new Hit(damage, hitChance);
      
      target.TakeHit(hit);
    }
    
    await ToSignal(Engine.GetMainLoop(), "idle_frame");

    return await base.ApplyImpl();
  }
}