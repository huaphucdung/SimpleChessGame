using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Knight : EnemyChess
{
    private readonly Vector2Int[] MoveRule = new Vector2Int[] {new Vector2Int(1,2), new Vector2Int(-1, 2), new Vector2Int(1, -2), new Vector2Int(-1, -2),
                                                                new Vector2Int(2,1), new Vector2Int(-2,1), new Vector2Int(2,-1), new Vector2Int(-2,-1)};
    public override void AIMove()
    {
        if (CanChessMate())
        {
            Kill(board.PlayerChess);
            return;
        }
        //Random move cell
        Vector2Int temp;
        Vector3Int position = -Vector3Int.one;
        while (!CheckCanMoveThisCell(position))
        {
            temp = GetCurrent2DCellPosition() + MoveRule[Random.Range(0, MoveRule.Length)];
            position = new Vector3Int(temp.x, 0, temp.y);

            if (!CheckCanMoveThisCell(position)) continue;

            Vector3 cellCenter = board.Grid.GetCellCenterWorld(position);
            //Check has any chess in this cell
            Collider[] chessColider = Physics.OverlapBox(cellCenter, Vector3.one * 0.5f, Quaternion.identity, board.ChessLayer);
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
        Move(position);
    }

    public override void Move(Vector3Int position)
    {
        base.Move(position);
        //Normal Move
        Vector3 newPosition = board.Grid.GetCellCenterWorld(position);
        newPosition.y = yOffset;
        transform.DOJump(newPosition, jumpForce, jumpNumber, duration).OnComplete(OnMoveComplete);
    }

    public override void Kill(IChess chess)
    {
        base.Kill(chess);
        transform.DOJump(chess.GetCurrentPosition(), jumpForce * speedScaleKill, jumpNumber, duration / speedScaleKill).OnComplete(() =>
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
        Vector2Int result = playerCell - GetCurrent2DCellPosition();

        return MoveRule.Contains(result);
    }
}
