using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.targets;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class SingleCostEffect : RootEffect
{
    [Export] public Cost[] Costs = [];

    [Export] public Effect[] Effects;

    [Export] public TargetSelector Targets;

    public override bool Applies(GameEvent gameEvent, ISubject root)
    {
        return Costs.All(cost => cost.CanMeet(gameEvent, root))
               && Targets.Select(gameEvent).Any();
    }

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        if (!Applies(gameEvent, root)) return [];

        var costChanges = Costs.Select(cost => cost.Stage(gameEvent, root)).Cast<IChange>().ToArray();

        var targets = Targets.Select(gameEvent);
        var targetChanges = targets.SelectMany(target => StageTargetEffects(gameEvent, target)).ToArray();

        var changeGroup = targetChanges.Concat(costChanges).ToArray().ToChangeGroup();

        return [changeGroup];
    }

    private IChange[] StageTargetEffects(GameEvent gameEvent, ISubject target)
    {
        return Effects.SelectMany(effect => effect.Stage(gameEvent, target)).ToArray();
    }
}