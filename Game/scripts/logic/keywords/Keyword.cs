using Godot;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.keywords;

[GlobalClass]
public partial class Keyword : KeywordBase
{
    [Export] public override Trigger[] Triggers { get; protected set; } = [];

    [Export] public override Quantity[] Quantities { get; protected set; } = [];
    
    public override ElicitStatement[] ElicitStatementRequirements
    {
        get => [];
        protected set { }
    }
}