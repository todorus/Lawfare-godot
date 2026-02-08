using Godot;
using Lawfare.scripts.logic.effects.property.amounts;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.conditions.quantity;

[GlobalClass]
public abstract partial class QuantityCondition : Resource
{
    public abstract bool Evaluate(GameEvent gameEventData, Quantity quantity);
}