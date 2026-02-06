using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.root;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.cards;

[GlobalClass]
public partial class Action : Resource, IAction
{
    [Export]
    public string Label { get; private set; }
    
    [Export]
    public Skill[] DicePools { get; private set; } = [];
    
    [Export]
    public Cost[] Costs { get; private set; } = [];
    
    [ExportGroup("Conditions")]
    [Export]
    public SubjectCondition[] SourceConditions { get; private set; } = [];
    [Export]
    public SubjectCondition[] TargetConditions { get; private set; } = [];
    
    [ExportGroup("Effects")]
    [Export]
    public RootEffect Effect { get; set; }

    public bool CanPerform(ISubject source)
    {
        var gameEvent = new GameEvent { Source = source };
        return Costs.All(cost => cost.CanMeet(gameEvent, source)) 
            && SourceConditions.All(condition => condition.Evaluate(gameEvent, source));
    }
    
    public bool CanTarget(ISubject target)
    {
        var gameEvent = new GameEvent { Target = target };
        return TargetConditions.All(condition => condition.Evaluate(gameEvent, target));
    }

    public bool Applies(GameEvent gameEvent) =>
        Costs.All(cost => cost.CanMeet(gameEvent, gameEvent.Source))
        && SourceConditions.All(condition => condition.Evaluate(gameEvent, gameEvent.Source))
        && TargetConditions.All(condition => condition.Evaluate(gameEvent, gameEvent.Target))
        && (Effect?.Applies(gameEvent, gameEvent.Source) ?? false);

    public ChangeGroup[] Stage(GameEvent gameEvent)
    {
        var costChangegroup = Costs
            .Select(cost => cost.Stage(gameEvent, gameEvent.Source))
            .Cast<IDiff>().ToArray().ToChangeGroup();
        var effectDiffs = Effect?.Stage(gameEvent, gameEvent.Source) ?? [];
        return new[]{costChangegroup}.Concat(effectDiffs).ToArray();
    }
}