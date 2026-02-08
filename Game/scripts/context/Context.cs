using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.subject;
using Lawyer = Lawfare.scripts.characters.lawyers.Lawyer;
using Team = Lawfare.scripts.characters.lawyers.Team;

namespace Lawfare.scripts.context;

public abstract partial class Context : Node, IContext
{
    public abstract Faction[] Factions { get; }
    public abstract Lawyer[] Lawyers { get; }
    public abstract Witness[] Witnesses { get; }
    public abstract Judge[] Judges { get; }
    public abstract Team[] Teams { get; }
    public Team GetTeam(ISubject subject) => Teams.FirstOrDefault(team => team.Members.Contains(subject));
    public Team GetOpposingTeam(ISubject subject) => Teams.FirstOrDefault(team => !team.Members.Contains(subject));

    public ISubject[] AllSubjects =>
        new ISubject[]{}
            .Concat(Lawyers)
            .Concat(Judges)
            .Concat(Witnesses)
            .ToArray();
}