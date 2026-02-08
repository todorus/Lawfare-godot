using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.@event;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.root;
using Lawfare.scripts.logic.@event;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.triggers;

[GlobalClass]
public partial class Trigger : Resource
{
    [Export(PropertyHint.ResourceType, "EventCondition")]
    public EventCondition[] EventConditions = [];

    [Export(PropertyHint.ResourceType, "SubjectCondition")]
    public SubjectCondition[] HostConditions = [];

    [Export] public RootEffect RootEffect = new None();

    [Export] public EventType Type;

    [Export] public string Label { get; set; }

    public bool Applies(GameEvent gameEvent)
    {
        return gameEvent.Type == Type
               && EventConditions.All(condition => condition.Evaluate(gameEvent))
               && HostConditions.All(condition => condition.Evaluate(gameEvent, gameEvent.Host))
               && RootEffect.Applies(gameEvent, gameEvent.Host);
    }

    public ChangeGroup[] Stage(GameEvent gameEventData)
    {
        return RootEffect.Stage(gameEventData, gameEventData.Host);
    }
}