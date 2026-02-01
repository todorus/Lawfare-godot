using System.Linq;
using Godot;
using Lawfare.scripts.@case;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.context.court;

[GlobalClass]
public partial class DocketEntry : Resource
{
    [Export]
    public Case Case;
    
    [Export]
    public JudgeDef[] JudgesDef = [];
    
    private Judge[] _judges;
    public new Judge[] Judges
    {
        get
        {
            if (_judges == null)
            {
                _judges = JudgesDef
                    .Select(def => new Judge(def))
                    .ToArray();
            }
            return _judges;
        }
    }
    
    [Export]
    public new Team Prosecution;
    
    [Export]
    public new Team Defense;
    
    [Export]
    private WitnessDef[] _witnessDefs;

    private Witness[] _witnesses;
    public new Witness[] Witnesses 
    {
        get
        {
            if (_witnesses == null)
            {
                _witnesses = _witnessDefs
                    .Select(def => new Witness(def))
                    .ToArray();
            }
            return _witnesses;
        }
    }
}