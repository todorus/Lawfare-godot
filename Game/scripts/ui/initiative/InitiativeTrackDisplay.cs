using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.ui.character.portrait;

namespace Lawfare.scripts.ui.initiative;

public partial class InitiativeTrackDisplay : Control
{
    
    [Signal]
    public delegate void OnCurrentChangedEventHandler(Lawyer current);
    
    private void OnCurrent(IHasInitiative obj) => EmitSignalOnCurrentChanged(obj as Lawyer);
    
    [Export]
    private PackedScene _portraitScene;
    
    [Export]
    private float _slotDistance = 100f;
    [Export]
    private float _stackDistance = 20f;
    
    private Dictionary<ICharacter, PortraitDisplay> _portraitInstances = new();
    
    public void Move(Move move)
    {
        // _track.Move(move.Lawyer, move.Initiative);
    }

    public override void _Ready()
    {
        base._Ready();
        // _track.CurrentChanged += OnCurrent;
        // _track.SlotsChanged += OnSlotsChanged;
    }

    public void SeedFromContext(Context context)
    {
        _portraitInstances.Clear();
    }
    
    private void OnSlotsChanged(IReadOnlyList<InitiativeSlot> slots)
    {
        // TODO update the UI to reflect the new slots
        // Loop through the slots and display the entities and their delays
        // Slots have a set distance from each other
        // When multiple entities they are stacked on top of each other in the same slot, with the current entity on top
        // so they would have a smaller distance between them to show they are in the same slot

        for(int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            for(int j = 0; j < slot.Row.Count; j++)
            {
                var entity = slot.Row[j];
                if(_portraitInstances.TryGetValue(entity as ICharacter, out var portrait))
                {
                    // Position the portrait based on the slot index and stack 
                    var x = Size.X - (i * _slotDistance + j * _stackDistance) - portrait.Size.X;
                    portrait.Position = new Vector2(x, 0);
                }
            }
        }
    }

}