using System.Collections.Generic;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.@event;

public struct GameEvent
{
    public EventType Type;
    public IAction Action;
    public ISubject Source;
    public ISubject Host;
    public ISubject Space;

    // Intermediary data
    public DiceRoll[] DiceRolls;

    public IEnumerable<HostedTrigger> Triggers => [];
    public ISubject[] Subjects => [Source, Host, Space];
    public Faction Faction;
}