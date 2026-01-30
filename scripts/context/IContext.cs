using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.context;

public interface IContext
{
    public Faction[] Factions { get; }
    public Lawyer[] Lawyers { get; }
    public Witness[] Witnesses { get; }
    public Judge[] Judges { get; }
    
    public Team[] Teams { get; }

    public ISubject[] AllSubjects { get; }
    
    public Team GetTeam(ISubject subject);
    public Team GetOpposingTeam(ISubject subject);
}