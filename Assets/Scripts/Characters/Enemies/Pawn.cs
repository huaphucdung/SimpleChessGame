using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Pawn : EnemyChess
{
    [SerializeField] private AudioClip promoteAudio;
    private readonly Vector2Int MoveRule = new Vector2Int(0, -1);
    private readonly Vector2Int[] MoveKill = new Vector2Int[] { new Vector2Int(1, -1), new Vector2Int(-1, -1) };
    public override void AIMove()
    {
        if (CanChessMate())
        {
            Kill(board.PlayerChess);
            return;
        }

        //Kill Move
        foreach (Vector2Int move in MoveKill)
        {
            Vector2Int direction = GetCurrent2DCellPosition() + move;
            Vector3Int killPosition = new Vector3Int(direction.x, 0, direction.y);

            Vector3 cellPosition = board.Grid.GetCellCenterWorld(killPosition);

            if(!CheckCanMoveThisCell(killPosition)) continue;

            Collider[] chessColider = Physics.OverlapBox(cellPosition, Vector3.one * 0.5f, Quaternion.identity, board.ChessLayer);
            foreach (Collider colider in chessColider)
            {
                //Check cell has obstcle
                IObstacle obstacle = colider.GetComponent<IObstacle>();
                if (obstacle != null)
                {
                    Kill(colider.GetComponent<IChess>());
                    return;
                }
            }
        }

        //Normal Move
        Vector2Int normalMove = current2DCellPosition + MoveRule;
        Vector3Int normalMove3D = new Vector3Int(normalMove.x, 0, normalMove.y);
        Vector3 cellCenter = board.Grid.GetCellCenterWorld(normalMove3D);
        if (!Physics.CheckBox(cellCenter, Vector3.one * 0.5f, Quaternion.identity, board.ChessLayer) && CheckCanMoveThisCell(normalMove3D))
        {
            Move(normalMove3D);
            return;
        }
    }

    public override void Move(Vector3Int position)
    {
        //Normal Move
        base.Move(position);
        Vector3 newPosition = board.Grid.GetCellCenterWorld(position);
        newPosition.y = yOffset;
        transform.DOMove(newPosition, duration).OnComplete(OnMoveComplete);
    }

    public override void Kill(IChess chess)
    {
        base.Kill(chess);
        transform.DOJump(chess.GetCurrentPosition(), jumpForce, jumpNumber, duration / speedScaleKill).OnComplete(() =>
        {
            //Set chess dead
            chess.Dead();
            //Check has another Enemy
            OnMoveComplete();
        });
    }

    protected override bool CanChessMate()
    {
        if (!CheckCanMoveThisCell(board.GetPlayerCurrent3DCell)) return false;
        Vector2Int playerCell = board.GetPlayerCurrent2DCell;
        Vector2Int direction = playerCell - GetCurrent2DCellPosition();

        return (direction.y == -1 && Mathf.Abs(direction.x) == 1);
    }

    protected override void OnMoveComplete()
    {
        if (current2DCellPosition.y == 0)
        {
            UpGradeChess();
        }
    }

    //Upgrade Pawn
    private void UpGradeChess()
    {
        AudioManager.PlayerOneShotAudio(AudioManager.VFXSource, promoteAudio);
        int random = Random.Range(1, System.Enum.GetNames(typeof(ChessType)).Length - 1);       
        //Remove this pawn
        Dead();
        //Spawn
        EnemyChess newEnemy = board.enemyPool.GetEnemy((ChessType)random);
        newEnemy.Spawn(GetCurrent3DCellPosition());
    }

}
