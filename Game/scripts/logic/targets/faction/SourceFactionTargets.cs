using Godot;
using Lawfare.scripts.logic.conditions.subject.faction;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.targets.faction;

[GlobalClass]
public partial class SourceFactionTargets : BaseTargetSelector
{
    protected override SubjectCondition[] SubjectConditions => new SubjectCondition[]
    {
        new BelongsToSourceFaction()
    };
}