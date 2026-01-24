using Godot;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.keywords;

[GlobalClass]
public abstract partial class KeywordBase : Resource
{
    [Export] public string Description;

    [Export] public string Label;

    public abstract Trigger[] Triggers { get; protected set; }

    public abstract Quantity[] Quantities { get; protected set; }
}