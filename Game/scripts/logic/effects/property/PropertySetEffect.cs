using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.property;

[GlobalClass]
public partial class PropertySetEffect : PropertyEffect
{
    [Export] public AmountProvider AmountProvider;

    protected override IDiff[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var currentAmount = subject?.Quantities?.GetValue(Property) ?? 0;
        var newAmount = AmountProvider.GetAmount(gameEvent, subject);
        return
        [
            new PropertyAddEffect.PropertyDiff(
                subject,
                new Quantity
                {
                    Property = Property,
                    Amount = currentAmount
                },
                new Quantity
                {
                    Property = Property,
                    Amount = newAmount
                }
            )
        ];
    }
}