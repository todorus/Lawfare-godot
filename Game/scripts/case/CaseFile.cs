using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.@case;

[GlobalClass]
public partial class CaseFile : Resource
{
    [Export]
    public Faction Faction;
    [Export]
    public Witness[] Witnesses = [];
    [Export]
    public Testimony[] Testimonies = [];
}