using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.root;

[GlobalClass]
public partial class BasicEffect : RootEffect
{
    [Export] 
    public Effect[] Effects = [];

    public override bool Applies(GameEvent gameEvent, ISubject root) => true;

    public override ChangeGroup[] Stage(GameEvent gameEvent, ISubject root)
    {
        var changes = Effects.SelectMany(effect => effect.Stage(gameEvent, gameEvent.Target)).ToArray();
        var changeGroup = changes.ToChangeGroup();
        return [changeGroup];
    }
}