using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;

public class EnemyChess : BaseChess, IEnemy
{
    [SerializeField] private ChessType chessType;
    public ChessType GetChessType => chessType;
    protected event Action OnFinishMoveAction;

    public override void Spawn(Vector3Int position)
    {
        base.Spawn(position);
        board.enemies.Add(this);
        OnFinishMoveAction += board.OnEnemyFinish;
    }

    public virtual void AIMove()
    {
    }

    public virtual void Kill(IChess chess)
    {
        current3DCellPosition = chess.GetCurrent3DCellPosition();
        current2DCellPosition = chess.GetCurrent2DCellPosition();
        AudioManager.PlayerOneShotAudio(audioSource, killAudio);
    }

    public virtual void Move(Vector3Int position)
    {
        current3DCellPosition = position;
        current2DCellPosition.x = position.x;
        current2DCellPosition.y = position.z;
        AudioManager.PlayerOneShotAudio(audioSource, moveAudio);
    }

    protected virtual void OnMoveComplete()
    {
        OnFinishMoveAction?.Invoke();
    }

    public override void Dead()
    {
        base.Dead();
        EffectManager.PlayerEffect("blackDead", transform.position, transform.rotation);
        board.enemies.Remove(this);
        board.enemyPool.ReleaveChess(this, chessType);
        OnFinishMoveAction -= board.OnEnemyFinish;
        DOTween.Kill(this);
    }

    public override void Reset()
    {
        board.enemyPool.ReleaveChess(this, chessType);
        OnFinishMoveAction -= board.OnEnemyFinish;
        DOTween.Kill(this);
    }

    protected virtual Vector3Int GetRamdomMove()
    {
        return Vector3Int.zero;
    }

    protected virtual bool CheckCanMoveThisCell(Vector3Int position)
    {
        Vector3 cellCenter = board.Grid.GetCellCenterWorld(position);
        Vector2Int moveCell = new Vector2Int(position.x, position.z);
        //Check has any chess in this cell
        foreach(IChess enemyChess in board.enemies)
        {
            if (enemyChess.GetCurrent2DCellPosition() == moveCell) return false;
        }

        return (board.IsInBoard(position));
    }

    protected virtual bool CanChessMate()
    {
        return true;
    }
}
