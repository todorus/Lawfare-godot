using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.targets;

[GlobalClass]
public abstract partial class SubjectProvider : Resource
{
    public abstract ISubject[] GetSubjects(GameEvent gameEvent);
}