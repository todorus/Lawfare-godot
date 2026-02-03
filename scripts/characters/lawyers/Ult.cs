using System;
using Godot;

namespace Lawfare.scripts.characters.lawyers;

public partial class Ult : GodotObject
{
    public event Action<Ult> OnChange; 
    
    private Witness[] _canElicit = [];

    public Witness[] CanElicit
    {
        get => _canElicit;
        set
        {
            _canElicit = value;
            OnChange?.Invoke(this);
        }
    }
}