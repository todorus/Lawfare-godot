using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.quantity;
using Lawfare.scripts.logic.effects.direct.property;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.diff.property;

[GlobalClass]
public partial class BasePropertyDiffCondition : PropertyDiffCondition
{
    [Export]
    private QuantityCondition[] _conditions = [];
    
    protected override bool EvaluatePropertyDiff(GameEvent gameEventData, PropertyDiff propertyDiff)
    {
        var difference = propertyDiff.Updated - propertyDiff.Original;
        return _conditions.All(condition => condition.Evaluate(gameEventData, difference));
    }
}