using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.conditions.subject.property;

[GlobalClass]
public abstract partial class PropertyCondition : SubjectCondition
{
    [Export] public AmountProvider AmountProvider;

    [Export] public bool CanUseMarket;

    [Export] public Property Property;

    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        var subjectValue = subject.Read(Property, gameEventData);
        var amount = AmountProvider.GetAmount(gameEventData, subject);
        return Compare(subjectValue, amount);
    }

    protected abstract bool Compare(int value, int amount);
}