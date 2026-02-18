using System.Linq;
using Godot;
using Godot.Collections;
using Lawfare.scripts.logic.conditions.@event;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.triggers;

[GlobalClass]
public partial class Trigger : Resource, IAction
{

    [Export] public string Label { get; set; }
    
    [ExportGroup("Conditions")]
    [Export] public EventType Type;
    
    [Export(PropertyHint.ResourceType, "EventCondition")]
    public EventCondition[] EventConditions = [];

    [Export(PropertyHint.ResourceType, "SubjectCondition")]
    public SubjectCondition[] HostConditions = [];

    public Skill[] DicePools { get; }

    [ExportGroup("Effect")]
    [Export]
    public Dictionary<InputLabel, EffectInput> Inputs { get; private set; } = new();
    
    [Export]
    public Effect[] Effects { get; private set; } = [];

    public bool Applies(GameEvent gameEvent)
    {
        return gameEvent.Type == Type
               && EventConditions.All(condition => condition.Evaluate(gameEvent))
               && HostConditions.All(condition => condition.Evaluate(gameEvent, gameEvent.Host));
    }

    public bool CanPerform(ISubject source) => true;
    public bool CanTarget(GameEvent gameEvent, ISubject target) => true;

    public ChangeGroup[] Stage(GameEvent gameEvent)
    {
        return Effects
            .SelectMany(effect => effect.Stage(gameEvent))
            .ToArray();
    }
}