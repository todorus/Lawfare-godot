using System;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.subject;
using Lawyer = Lawfare.scripts.characters.lawyers.Lawyer;
using Team = Lawfare.scripts.characters.lawyers.Team;

namespace Lawfare.scripts.context;

public interface IContext
{
    public Faction[] Factions { get; }
    public Lawyer[] Lawyers { get; }
    public Witness[] Witnesses { get; }
    public Judge[] Judges { get; }

    public event Action<InitiativeTrackState> InitiativeTrackChanged;
    
    public InitiativeTrackState InitiativeTrack { get; set; }
    
    public Team[] Teams { get; }

    public ISubject[] AllSubjects { get; }
    
    public Team GetTeam(ISubject subject);
    public Team GetOpposingTeam(ISubject subject);
}