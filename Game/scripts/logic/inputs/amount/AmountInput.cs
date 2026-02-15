using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs.amount;

[GlobalClass]
public abstract partial class AmountInput : EffectInput
{
    public override object GetValue(GameEvent gameEvent) => GetAmountValue(gameEvent);
    
    protected abstract int GetAmountValue(GameEvent gameEvent);
}