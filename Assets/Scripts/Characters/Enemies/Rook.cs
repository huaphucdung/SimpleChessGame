using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : EnemyChess
{
    private readonly Vector2Int[] MoveRule = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0)};
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
            temp = GetCurrent2DCellPosition() + direction * Random.Range(1, 4);
            position = new Vector3Int(temp.x, 0, temp.y);


            Vector2Int checkCell = GetCurrent2DCellPosition();
            //Check has any chess on chessmate road
            while (checkCell != temp)
            {
                checkCell += direction;
                Vector3Int cellCenter = new Vector3Int(checkCell.x, 0, checkCell.y);
                Vector3 cellPosition = board.Grid.GetCellCenterWorld(new Vector3Int(checkCell.x, 0, checkCell.y));

                if (!CheckCanMoveThisCell(cellCenter)) break;

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
        }
        Move(position);
    }

    public override void Move(Vector3Int position)
    {
        base.Move(position);
        //Normal Move
        Vector3 newPosition = board.Grid.GetCellCenterWorld(position);
        newPosition.y = yOffset;
        transform.DOMove(newPosition, duration).OnComplete(OnMoveComplete);
    }

    public override void Kill(IChess chess)
    {
        base.Kill(chess);
        transform.DOMove(chess.GetCurrentPosition(), duration/speedScaleKill).OnComplete(() =>
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

        //Check player on cross
        if (direction.x != 0 && direction.y != 0) return false;

        Vector2Int directionNormalize = direction;
        directionNormalize.x /= (directionNormalize.x != 0) ? (Mathf.Abs(direction.x)) : 1;
        directionNormalize.y /= (directionNormalize.y != 0) ? (Mathf.Abs(direction.y)) : 1;

        Vector2Int temp = GetCurrent2DCellPosition() + directionNormalize;

        //Check has any chess on chessmate road
        while (temp != playerCell)
        {
            Vector3 cellCenter = board.Grid.GetCellCenterWorld(new Vector3Int(temp.x, 0, temp.y));
            if (Physics.CheckBox(cellCenter, Vector3.one * 0.5f, Quaternion.identity, board.ChessLayer)) return false;
            temp += directionNormalize;
        }
        return true;
    }
}
