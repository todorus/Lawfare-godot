using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.diff;
using Lawfare.scripts.logic.conditions.quantity.amount;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;
using IsHostQuantityDiff = Lawfare.scripts.logic.conditions.diff.quantity.IsHostQuantityDiff;

namespace Lawfare.scripts.logic.conditions.@event.diff.property;

[GlobalClass]
public partial class HasHostQuantityDiff : EventCondition
{
    [Export]
    private Property _property;
    [Export]
    private QuantityAmountCondition _amountCondition;
    
    public override bool Evaluate(GameEvent gameEvent)
    {
        if(gameEvent.Resolution == null) return false;
        var condition = new IsHostQuantityDiff(_property, _amountCondition);
        return gameEvent.Resolution.Changes.Any(diff => condition.Evaluate(gameEvent, diff));
    }
}