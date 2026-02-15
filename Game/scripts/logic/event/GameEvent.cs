using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.@event;

public partial struct GameEvent
{
    public EventType Type;
    public IAction Action;
    public ISubject Source;
    public ISubject Target;
    
    public ISubject Host;
    public ISubject Space;
    
    public IContext Context;

    public Dictionary<InputLabel, EffectInput> Inputs;

    // Intermediary data
    public DiceRoll[] DiceRolls;

    public IEnumerable<HostedTrigger> Triggers => Subjects.SelectMany(subject => subject.Triggers);
    public ISubject[] Subjects => Context.AllSubjects;

    public Faction Faction;

    public Resolution Resolution;
}