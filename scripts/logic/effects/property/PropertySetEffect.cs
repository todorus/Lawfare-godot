using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.property;

[GlobalClass]
public partial class PropertySetEffect : PropertyEffect
{
    [Export] public AmountProvider AmountProvider;

    protected override IChange[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var desiredAmount = AmountProvider.GetAmount(gameEvent, subject);
        var currentAmount = subject?.Quantities?.Get(Property) ?? 0;
        var delta = desiredAmount - currentAmount;
        return
        [
            new PropertyAddEffect.PropertyAddChange
            {
                Property = Property,
                Amount = delta,
                Subject = subject
            }
        ];
    }
}