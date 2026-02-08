using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.conditions.subject.faction;

namespace Lawfare.scripts.logic.targets.faction;

[GlobalClass]
public partial class NonHostFactionTargets : BaseTargetSelector
{
    protected override SubjectCondition[] SubjectConditions => new SubjectCondition[]
    {
        new BelongsNotToHostFaction()
    };
}