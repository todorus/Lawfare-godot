using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.effects.targets;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class TargetedEffect : RootEffect
{
    [ExportCategory("Targeting")]
    [Export]
    public SubjectProvider TargetProvider { get; private set; }
    
    [ExportCategory("Conditions")]
    [Export]
    public SubjectCondition[] SourceConditions { get; private set; } = [];
    [Export]
    public SubjectCondition[] TargetConditions { get; private set; } = [];
    [Export]
    public SubjectCondition[] SpaceConditions { get; private set; } = [];
    
    [ExportCategory("Effects")]
    [Export] 
    public Effect[] Effects = [];
    
    public override bool Applies(GameEvent gameEvent, ISubject root)
    {
        var targets = TargetProvider.GetSubjects(gameEvent);
        var source = gameEvent.Source;
        var space = gameEvent.Space;

        return SourceConditions.All(condition => condition.Evaluate(gameEvent, source))
               && targets.Any(target => TargetConditions.All(condition => condition.Evaluate(gameEvent, target)))
               && SpaceConditions.All(condition => condition.Evaluate(gameEvent, space));
    }

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        var targets = TargetProvider.GetSubjects(gameEvent);
        var changeGroups = targets.Select(target => StageForTarget(gameEvent, target)).ToArray();
        return changeGroups;
    }
    
    private ChangeGroup StageForTarget(GameEvent gameEvent, ISubject target)
    {
        var targetedEvent = gameEvent with { Target = target };
        var changes = Effects.SelectMany(effect => effect.Stage(targetedEvent, target)).ToArray();
        return changes.ToChangeGroup();
    }
}