using Godot;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.elicit;

[GlobalClass]
public partial class Goal : KeywordBase
{
    public override Trigger[] Triggers { get; protected set; } = [];
    public override Quantity[] Quantities { get; protected set; } = [];

    [Export] public override ElicitStatement[] ElicitStatementRequirements { get; protected set; } = [];
}