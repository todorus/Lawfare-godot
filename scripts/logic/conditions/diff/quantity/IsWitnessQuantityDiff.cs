using Godot;
using Lawfare.scripts.logic.conditions.diff.subject;
using Lawfare.scripts.logic.conditions.quantity.amount;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.conditions.diff.quantity;

public partial class IsWitnessQuantityDiff : DiffCondition
{
    private static readonly IsWitnessDiffCondition WitnessCondition = new();
    
    [Export]
    private Property _property;
    [Export]
    private QuantityAmountCondition _amountCondition;
    
    public IsWitnessQuantityDiff(Property property, QuantityAmountCondition amountCondition)
    {
        _property = property;
        _amountCondition = amountCondition;
    }

    public IsWitnessQuantityDiff()
    {
    }

    public override bool Evaluate(GameEvent gameEventData, IDiff diff)
    {
        if (diff is not PropertyAddEffect.PropertyDiff propertyDiff) return false;
        if (propertyDiff.Original.Property != _property) return false;
        
        var difference = propertyDiff.Updated - propertyDiff.Original;
        return WitnessCondition.Evaluate(gameEventData, diff)
               && _amountCondition.Evaluate(gameEventData, difference);
    }
}