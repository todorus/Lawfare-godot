using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.context;

public interface IContext
{
    public Faction[] Factions { get; }
    public Lawyer[] Lawyers { get; }
    public Witness[] Witnesses { get; }
    public Judge[] Judges { get; }
}