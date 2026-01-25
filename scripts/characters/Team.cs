using Godot;
using Lawfare.scripts.board.factions;

namespace Lawfare.scripts.characters;

[GlobalClass]
public partial class Team : GodotObject
{
    [Export]
    public Faction Faction;
    
    [Export]
    public Lawyer[] Members = [];
}