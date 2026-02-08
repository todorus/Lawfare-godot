using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.conditions.subject.property;

public partial class WithinRange : SubjectCondition
{
    [Export] public AmountProvider MaxAmountProvider;

    [Export] public AmountProvider MinAmountProvider;

    [Export] public Property Property;

    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        var value = subject.Read(Property, gameEventData);
        var minAmount = MinAmountProvider.GetAmount(gameEventData, subject);
        var maxAmount = MaxAmountProvider.GetAmount(gameEventData, subject);
        return Compare(value, minAmount, maxAmount);
    }

    protected bool Compare(int value, int minAmount, int maxAmount)
    {
        return value >= minAmount && value <= maxAmount;
    }
}