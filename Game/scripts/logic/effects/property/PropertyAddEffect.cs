using System.Collections.Generic;
using Godot;
using Lawfare.scripts.logic.effects.direct.property;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.modifiers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.property;

[GlobalClass]
public partial class PropertyAddEffect : PropertyEffect
{
    [Export] public AmountProvider AmountProvider;

    protected override IDiff[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        if (subject?.Quantities == null)
        {
            GD.PushWarning("PropertyAddChange.Apply: Subject or Subject.Quantities is null");
            var zeroQuantity = new Quantity
            {
                Property = Property,
                Amount = 0
            };
            return [new PropertyDiff(subject, zeroQuantity, zeroQuantity)];
        }

        var original = new Quantity
        {
            Property = Property,
            Amount = subject.Quantities.GetValue(Property)
        };
            
        int stagedValue = subject.Quantities.StageAdd(Property, amount);
            
        var updated = new Quantity
        {
            Property = Property,
            Amount = stagedValue
        };
            
        return [new PropertyDiff(subject, original, updated)];
    }
}