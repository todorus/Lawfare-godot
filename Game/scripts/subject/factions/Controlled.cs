using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.board.factions;

public class Controlled
{
    private readonly HashSet<ISubject> _pawns = new();

    public IEnumerable<ISubject> Pawns => _pawns;

    public void Add(ISubject subject)
    {
        if (subject is ISubject pawn)
        {
            _pawns.Add(pawn);
            return;
        }

        throw new Exception("Subject type not supported");
    }

    public void Remove(ISubject subject)
    {
        if (subject is ISubject pawn)
        {
            _pawns.Remove(pawn);
            return;
        }

        throw new Exception("Subject type not supported");
    }

    public IEnumerable<ISubject> PawnsWithConditions(SubjectCondition[] conditions)
    {
        var gameEventData = new GameEvent();
        return _pawns.Where(pawn => conditions.All(condition => condition.Evaluate(gameEventData, pawn)));
    }

    public IEnumerable<ISubject> PawnsWithKeywords(Keyword[] keywords)
    {
        return _pawns.Where(pawn => keywords.All(keyword => pawn.Keywords.Contains(keyword)));
    }

    public IEnumerable<ISubject> PawnsWithKeyword(Keyword keyword)
    {
        return _pawns.Where(pawn => pawn.Keywords.Contains(keyword));
    }
}