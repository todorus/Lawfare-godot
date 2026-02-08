using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.@case;

[GlobalClass]
public partial class Case : Resource
{
    [Export]
    public string Label = "New Case";
    [Export]
    public CaseFile ProsecutorCaseFile;
    [Export]
    public CaseFile DefenseCaseFile;

    public Testimony[] GetTestimoniesByWitnessAndFaction(WitnessDef witness, Faction faction)
    {
        CaseFile caseFile = GetCaseFileByFaction(faction);
        if (caseFile == null) return [];

        return caseFile.Testimonies.Where(testimony => testimony.Witness == witness).ToArray();
    }
    
    public CaseFile GetCaseFileByFaction(Faction faction)
    {
        if (ProsecutorCaseFile.Faction == faction)
        {
            return ProsecutorCaseFile;
        }

        if (DefenseCaseFile.Faction == faction)
        {
            return DefenseCaseFile;
        }
        return null;
    }
}