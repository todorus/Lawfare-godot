using Godot;
using Lawfare.scripts.logic.conditions.subject.property;
using Lawfare.scripts.logic.conditions.subject.type;
using Lawfare.scripts.subject.quantities;
using StaticAmount = Lawfare.scripts.logic.effects.property.amounts.StaticAmount;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.targets.release;

[GlobalClass]
public partial class ReleaseTargetSelector : BaseTargetSelector
{
    [Export] public PropertyConfig PropertyConfig;

    [Export] public int MinCorruptionLevel { get; set; }

    [Export] public int MaxCorruptionLevel { get; set; } = 5;

    protected override SubjectCondition[] SubjectConditions =>
    [
        new IsHost(),
        new GreaterOrEqualTo
        {
            AmountProvider = new StaticAmount { Amount = PropertyConfig.Urge.Maximum },
            Property = PropertyConfig.Urge
        },
        new WithinRange
        {
            MinAmountProvider = new StaticAmount { Amount = MinCorruptionLevel },
            MaxAmountProvider = new StaticAmount { Amount = MaxCorruptionLevel },
            Property = PropertyConfig.CorruptionLevel
        }
    ];
}