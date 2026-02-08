using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class MultiEffect : RootEffect
{
    [Export] public RootEffect[] Effects = [];

    public override bool Applies(GameEvent gameEvent, ISubject root)
    {
        return Effects.Any(effect => effect.Applies(gameEvent, root));
    }

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        return Effects
            .Where(effect => effect.Applies(gameEvent, root))
            .SelectMany(effect => effect.Stage(gameEvent, root))
            .ToArray();
    }
}