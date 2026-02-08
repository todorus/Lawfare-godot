using Godot;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.diff.property;

[GlobalClass]
public abstract partial class PropertyDiffCondition : DiffCondition
{
    public override bool Evaluate(GameEvent gameEventData, IDiff diff)
    {
        if(diff is not PropertyAddEffect.PropertyDiff propertyDiff) return false;
        return EvaluatePropertyDiff(gameEventData, propertyDiff);
    }
    
    protected abstract bool EvaluatePropertyDiff(GameEvent gameEventData, PropertyAddEffect.PropertyDiff propertyDiff);
}