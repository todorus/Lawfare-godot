using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.cards;

public partial class Card(Action action) : GodotObject, IAction
{
    public string Label => action.Label;

    public Skill[] DicePools => action.DicePools;
    
    public bool CanPerform(ISubject source) => action.CanPerform(source);

    public bool CanTarget(GameEvent gameEvent, ISubject target) => action.CanTarget(gameEvent, target);

    public bool Applies(GameEvent gameEvent)
        => action.Applies(gameEvent);

    public ChangeGroup[] Stage(GameEvent gameEvent) =>
        action.Stage(gameEvent);
}