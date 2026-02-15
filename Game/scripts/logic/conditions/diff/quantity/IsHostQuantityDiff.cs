using Godot;
using Lawfare.scripts.logic.conditions.diff.subject;
using Lawfare.scripts.logic.conditions.quantity.amount;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.direct.property;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.conditions.diff.quantity;

[GlobalClass]
public partial class IsHostQuantityDiff : DiffCondition
{
    private static IsHostDiffCondition _hostCondition = new();
    
    [Export]
    private Property _property;
    [Export]
    private QuantityAmountCondition _amountCondition;
    
    public IsHostQuantityDiff(Property property, QuantityAmountCondition amountCondition)
    {
        _property = property;
        _amountCondition = amountCondition;
    }

    public IsHostQuantityDiff()
    {
    }

    public override bool Evaluate(GameEvent gameEventData, IDiff diff)
    {
        if (diff is not PropertyDiff propertyDiff) return false;
        if (propertyDiff.Original.Property != _property) return false;
        
        var difference = propertyDiff.Updated - propertyDiff.Original;
        return _hostCondition.Evaluate(gameEventData, diff)
            && _amountCondition.Evaluate(gameEventData, difference);
    }
}