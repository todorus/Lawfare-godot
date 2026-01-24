using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.targets;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class TargetCostEffect : RootEffect
{
    [Export] public Cost[] CostsPerTarget = [];

    [Export] public Effect[] Effects;

    [Export] public TargetSelector Targets;

    public override bool Applies(GameEvent gameEvent, ISubject root)
    {
        return Targets.Select(gameEvent).Any()
               && CostsPerTarget.All(cost =>
                   cost.CanMeet(gameEvent, gameEvent.Source)); // A single target must be affordable
    }

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        if (!Applies(gameEvent, root)) return [];

        var changeGroups = new List<ChangeGroup>();
        var targets = Targets.Select(gameEvent);
        for (var i = 0; i < targets.Length; i++)
        {
            var multiplier = i + 1;
            if (!CostsPerTarget.All(cost => cost.CanMeet(gameEvent, gameEvent.Source, multiplier))) break;

            var target = targets.ElementAt(i);
            var costChanges = CostsPerTarget.Select(cost => cost.Stage(gameEvent, gameEvent.Source)).Cast<IChange>()
                .ToArray();
            var targetChanges = StageTargetEffects(gameEvent, target);
            var changeGroup = targetChanges.Concat(costChanges).ToArray().ToChangeGroup();
            changeGroups.Add(changeGroup);
        }

        return changeGroups.ToArray();
    }

    private IChange[] StageTargetEffects(GameEvent gameEvent, ISubject subject)
    {
        return Effects.SelectMany(effect => effect.Stage(gameEvent, subject)).ToArray();
    }
}