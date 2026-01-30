using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.diff;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.@event.diff.property;

[GlobalClass]
public partial class HasDiffWith : EventCondition
{
    [Export] 
    private DiffCondition[] _conditions; 
    
    public override bool Evaluate(GameEvent gameEvent)
    {
        var resolution = gameEvent.Resolution;
        if (resolution == null) return false;
        
        return resolution.Changes.Any(diff => _conditions.All(condition => condition.Evaluate(gameEvent, diff)));  
    }
}