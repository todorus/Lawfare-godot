using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.@event;

public struct GameEvent
{
    public EventType Type;
    public IAction Action;
    public ISubject Source;
    public ISubject Target;
    
    public ISubject Host;
    public ISubject Space;
    
    public IContext Context;

    // Intermediary data
    public DiceRoll[] DiceRolls;

    public IEnumerable<HostedTrigger> Triggers => [];
    public ISubject[] Subjects => new[]{
        Source, Target, Host, Space
    }.Where(subject => subject != null).ToArray();

    public Faction Faction;
}