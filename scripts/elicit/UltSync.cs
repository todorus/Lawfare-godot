using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.elicit;

public partial class UltSync : Node
{
    [Export]
    private Context _context;
    private Context Context
    {
        get => _context;
        set
        {
            _context = value;
            Sync();
        }
    }
    
    public void Sync()
    {
        if(_context == null) return;

        foreach (var lawyer in _context.Lawyers)
        {
            lawyer.Ult.CanElicit = _context.Witnesses
                .Where(witness => CanElicitStatements(lawyer, witness))
                .ToArray();
        }
    }
    
    private bool CanElicitStatements(Lawyer lawyer, Witness witness)
    {
        var gameEvent = new GameEvent
        {
            Context = _context,
            Source = lawyer,
            Target = witness,
            Type = EventType.UltSync
        };
        return lawyer.ElicitStatementRequirements
            .Any(requirement => requirement.Evaluate(gameEvent, lawyer, witness));
    }
}