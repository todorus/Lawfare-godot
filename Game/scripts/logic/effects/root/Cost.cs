using Godot;
using Lawfare.scripts.logic.effects.direct.property;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class Cost : Resource
{
    [Export] public AmountProvider AmountProvider;

    [Export] public Property Property;

    public bool CanMeet(GameEvent gameEvent, ISubject subject, int multiplier = 1)
    {
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        var subjectValue = subject.Available(Property, gameEvent);
        return subjectValue >= amount * multiplier;
    }

    public PropertyDiff Stage(GameEvent gameEvent, ISubject subject)
    {
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        var oldAmount = subject.Quantities.GetValue(Property);
        var newAmount = subject.Quantities.StageAdd(Property, amount);
        return new PropertyDiff(
            subject,
            new Quantity
            {
                Property = Property,
                Amount = oldAmount
            },
            new Quantity
            {
                Property = Property,
                Amount = newAmount
            }
        );
    }
}