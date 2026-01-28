using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;

namespace Lawfare.scripts.characters;

[GlobalClass]
public partial class Team : Resource
{
    [Export]
    public Faction Faction;
    
    [Export] private LawyerDef[] _lawyerDefs = [];

    private Lawyer[] _members;
    public Lawyer[] Members {
        set => _members = value;
        get {
            if (_members == null) 
            {
                _members = _lawyerDefs
                    .Select(def => new Lawyer(def))
                    .ToArray();
            }
            return _members;
        } 
    }
}