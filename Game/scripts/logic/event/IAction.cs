using Godot.Collections;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.@event;

public interface IAction
{
    public Skill[] DicePools { get; }

    public Dictionary<InputLabel, EffectInput> Inputs { get; }

    public bool Applies(GameEvent gameEvent);
    
    public bool CanPerform(ISubject source);
    
    public bool CanTarget(GameEvent gameEvent, ISubject target);

    public ChangeGroup[] Stage(GameEvent gameEvent);
}