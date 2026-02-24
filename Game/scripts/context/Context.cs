using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.subject;
using Lawyer = Lawfare.scripts.characters.lawyers.Lawyer;
using Team = Lawfare.scripts.characters.lawyers.Team;

namespace Lawfare.scripts.context;

public abstract partial class Context : Node, IContext
{
    public abstract Faction[] Factions { get; }
    public abstract Lawyer[] Lawyers { get; }
    public abstract Witness[] Witnesses { get; }
    public abstract Judge[] Judges { get; }
    public abstract Team[] Teams { get; }
    
    private Dictionary<Team, Lawyer> _speakers = new();
    public Lawyer GetSpeaker(Team team) => _speakers.ContainsKey(team) ? _speakers[team] : null;

    public Team GetTeam(ISubject subject) => Teams.FirstOrDefault(team => team.Members.Contains(subject));
    public Team GetOpposingTeam(ISubject subject) => Teams.FirstOrDefault(team => !team.Members.Contains(subject));

    public event Action<InitiativeTrackState> InitiativeTrackChanged;
    
    [Signal]
    public delegate void ActiveLawyerChangedEventHandler(Lawyer lawyer);
    
    [Signal]
    public delegate void ActiveHandChangedEventHandler(Card[] hand);
    
    [Signal]
    public delegate void SpeakerChangedEventHandler(Team team, Lawyer lawyer);
    
    private Lawyer _activeLawyer;

    public Lawyer ActiveLawyer
    {
        get => _activeLawyer;
        set
        {
            var previousLawyer = _activeLawyer;
            _activeLawyer = value;
            
            var team = GetTeam(value);
            if (team != null)
            {
                _speakers[team] = value;
                EmitSignalSpeakerChanged(team, value);
            }
            
            if (previousLawyer == value) return;
            
            EmitSignalActiveLawyerChanged(value);
            var actions = value?.Actions ?? [];
            var cards = actions.Select(action => new Card(action)).ToArray();
            EmitSignalActiveHandChanged(cards);
        }
    }

    private InitiativeTrackState _initiativeTrack;
    public InitiativeTrackState InitiativeTrack
    {
        get => _initiativeTrack;
        set 
        {
            _initiativeTrack = value;
            InitiativeTrackChanged?.Invoke(value);

            var slots = _initiativeTrack.Slots;
            
            if(slots == null || slots.Length == 0 || slots[0].Occupant == null) ActiveLawyer = null;
            ActiveLawyer = _initiativeTrack.Slots[0].Occupant as Lawyer;
        }
    }

    public ISubject[] AllSubjects =>
        new ISubject[]{}
            .Concat(Lawyers)
            .Concat(Judges)
            .Concat(Witnesses)
            .ToArray();
}