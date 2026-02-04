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
            lawyer.Ult?.Update(lawyer, _context);
        }
    }
}