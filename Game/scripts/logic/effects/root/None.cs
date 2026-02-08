using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class None : RootEffect
{
    public override bool Applies(GameEvent gameEvent, ISubject root)
    {
        return true;
    }

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        return [];
    }
}