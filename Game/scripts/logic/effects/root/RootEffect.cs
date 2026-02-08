using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public abstract partial class RootEffect : Resource, IRootEffect
{
    public abstract bool Applies(GameEvent gameEvent, ISubject root);
    public abstract ChangeGroup[] Stage(GameEvent gameEvent, ISubject root);
}