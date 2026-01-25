using Godot;
using Lawfare.scripts.board.factions;

namespace Lawfare.scripts.characters;

[GlobalClass]
public partial class Team : Resource
{
    [Export]
    public Faction Faction;
    
    [Export]
    public Lawyer[] Members = [];
}