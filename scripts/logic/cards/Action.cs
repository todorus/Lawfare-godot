using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.root;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.cards;

[GlobalClass]
public partial class Action : Resource, IAction
{
    [Export]
    public string Label { get; private set; }
    
    [Export]
    public Skill[] DicePools { get; private set; } = [];
    [Export]
    public RootEffect Effect { get; set; }
    
    public bool Applies(GameEvent gameEvent) =>
        Effect?.Applies(gameEvent, gameEvent.Source) ?? false;

    public ChangeGroup[] Stage(GameEvent gameEvent)
    {
        return Effect?.Stage(gameEvent, gameEvent.Source) ?? [];
    }
}