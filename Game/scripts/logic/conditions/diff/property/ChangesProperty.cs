using Godot;
using Lawfare.scripts.logic.effects.direct.property;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.conditions.diff.property;

[GlobalClass]
public partial class ChangesProperty : PropertyDiffCondition
{
    [Export] 
    private Property _property;
    
    protected override bool EvaluatePropertyDiff(GameEvent gameEventData, PropertyDiff propertyDiff)
    {
        return propertyDiff.Updated.Property == _property;
    }
}