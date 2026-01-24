using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.@event.count;

[GlobalClass]
public partial class HasSourceAndNonSourceFactionPawns : EventCondition
{
    public override bool Evaluate(GameEvent context)
    {
        var hasSourceFactionPawn = false;
        var hasNonSourceFactionPawn = false;

        foreach (var subject in context.Subjects)
        {
            if (!subject.CanHaveFaction)
                continue;

            if (subject.Allegiances.Contains(context.Faction))
                hasSourceFactionPawn = true;
            else
                hasNonSourceFactionPawn = true;

            if (hasSourceFactionPawn && hasNonSourceFactionPawn)
                return true;
        }

        return false;
    }
}