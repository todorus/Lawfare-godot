using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.conditions.quantity.property;

[GlobalClass]
public partial class HasPropertyCondition : QuantityCondition
{
    [Export]
    private Property _property;
    
    public override bool Evaluate(GameEvent gameEventData, Quantity quantity)
    {
        return quantity.Property == _property;
    }
}