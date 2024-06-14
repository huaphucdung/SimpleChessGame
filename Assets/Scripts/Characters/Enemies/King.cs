using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : EnemyChess
{
    private readonly Vector2Int[] MoveRule = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0),
                                                                new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};
    public override void AIMove()
    {
        if (CanChessMate())
        {
            Kill(board.PlayerChess);
            return;
        }

        //Random Move
        Vector2Int temp;
        Vector3Int position = -Vector3Int.one;
        while (!CheckCanMoveThisCell(position))
        {
            Vector2Int direction = MoveRule[Random.Range(0, MoveRule.Length)];
            temp = GetCurrent2DCellPosition() + direction;
            position = new Vector3Int(temp.x, 0, temp.y);

            Vector3 cellPosition = board.Grid.GetCellCenterWorld(position);

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
        transform.DOJump(chess.GetCurrentPosition(), jumpForce*speedScaleKill, jumpNumber, duration/speedScaleKill).OnComplete(() =>
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
        
        int distance = Mathf.RoundToInt(Vector2Int.Distance(GetCurrent2DCellPosition(), playerCell));
        return (distance == 1);
    }
}
