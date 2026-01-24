using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.property.amounts.property;

[GlobalClass]
public abstract partial class PropertyAmountProvider : AmountProvider
{
    [Export] private Property Property;

    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        var propertyValue = subject.Read(Property, gameEvent);
        return CountProperty(propertyValue);
    }

    protected abstract int CountProperty(int propertyValue);
}