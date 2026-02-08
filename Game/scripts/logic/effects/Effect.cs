using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.@event;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects;

[GlobalClass]
public abstract partial class Effect : Resource, IEffect
{
    [Export] public EventCondition[] EventConditions = [];

    [Export] public SubjectCondition[] SubjectConditions = [];

    public IDiff[] Stage(GameEvent gameEvent, ISubject subject)
    {
        if (!EventConditions.All(condition => condition.Evaluate(gameEvent))) return [];
        if (!SubjectConditions.All(condition => condition.Evaluate(gameEvent, subject))) return [];
        return StageInternal(gameEvent, subject);
    }

    protected abstract IDiff[] StageInternal(GameEvent gameEvent, ISubject subject);
}