using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class BasicEffect : RootEffect
{
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
        var source = gameEvent.Source;
        var target = gameEvent.Target;
        var space = gameEvent.Space;

        return SourceConditions.All(condition => condition.Evaluate(gameEvent, source))
               && TargetConditions.All(condition => condition.Evaluate(gameEvent, target))
               && SpaceConditions.All(condition => condition.Evaluate(gameEvent, space));
    }

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        var changes = Effects.SelectMany(effect => effect.Stage(gameEvent, root)).ToArray();
        var changeGroup = changes.ToChangeGroup();
        return [changeGroup];
    }
}