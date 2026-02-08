using Godot;

namespace Lawfare.scripts.logic.keywords;

[GlobalClass]
public partial class KeywordConfig : Resource
{
    [Export] public Keyword Grunt { get; private set; }

    [Export] public Keyword Leader { get; private set; }

    [Export] public Keyword TrainingCenter { get; private set; }

    [Export] public Keyword Principle { get; private set; }

    [Export] public Keyword Headquarters { get; private set; }
}