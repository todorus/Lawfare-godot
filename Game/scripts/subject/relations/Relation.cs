using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.subject.relations;

[GlobalClass]
public partial class Relation : Resource, IRelation
{
    [Export] public Property Property { get; set; }

    [Export] public int Amount { get; set; }
    
    [Export] public Faction Faction { get; set; }
}

public interface IRelation
{
    public Property Property { get; }
    public int Amount { get; set; }
    
    public Faction Faction { get; }
}