using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChess
{
    void Initialize(ChessBoard chessBoard);
    void Spawn(Vector3Int position);
    Vector2Int GetCurrent2DCellPosition();
    Vector3Int GetCurrent3DCellPosition();
    Vector3 GetCurrentPosition();
    void Dead();
    void Reset();
}

public interface IObstacle
{
}

public interface ICharacter
{
    void Move(Vector3Int position);

    void Kill(IChess chess);
}

public interface IPlayer : ICharacter
{
}

public interface IEnemy : ICharacter
{
    void AIMove();
}
