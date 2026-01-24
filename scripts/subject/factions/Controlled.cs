using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.board.factions;

public class Controlled
{
    private readonly HashSet<ICharacter> _pawns = new();

    public IEnumerable<ICharacter> Pawns => _pawns;

    public void Add(ISubject subject)
    {
        if (subject is ICharacter pawn)
        {
            _pawns.Add(pawn);
            return;
        }

        throw new Exception("Subject type not supported");
    }

    public void Remove(ISubject subject)
    {
        if (subject is ICharacter pawn)
        {
            _pawns.Remove(pawn);
            return;
        }

        throw new Exception("Subject type not supported");
    }

    public IEnumerable<ICharacter> PawnsWithConditions(SubjectCondition[] conditions)
    {
        var gameEventData = new GameEvent();
        return _pawns.Where(pawn => conditions.All(condition => condition.Evaluate(gameEventData, pawn)));
    }

    public IEnumerable<ICharacter> PawnsWithKeywords(Keyword[] keywords)
    {
        return _pawns.Where(pawn => keywords.All(keyword => pawn.Keywords.Contains(keyword)));
    }

    public IEnumerable<ICharacter> PawnsWithKeyword(Keyword keyword)
    {
        return _pawns.Where(pawn => pawn.Keywords.Contains(keyword));
    }
}