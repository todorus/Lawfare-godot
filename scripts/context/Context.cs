using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.context;

public abstract partial class Context : Node, IContext
{
    public abstract Faction[] Factions { get; }
    public abstract Lawyer[] Lawyers { get; }
    public abstract Witness[] Witnesses { get; }
    public abstract Judge[] Judges { get; }
}