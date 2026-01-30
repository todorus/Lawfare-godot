using Godot;
using Lawfare.scripts.logic.effects.property.amounts;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.conditions.quantity.amount;

public abstract partial class QuantityAmountCondition : QuantityCondition
{
    [Export]
    private AmountProvider _amountProvider;
    
    public override bool Evaluate(GameEvent gameEventData, Quantity quantity)
    {
        var threshold = _amountProvider.GetAmount(gameEventData, null);
        var amount = quantity.Amount;
        return Evaluate(threshold, amount);
    }
    
    protected abstract bool Evaluate(int threshold, int amount);
}