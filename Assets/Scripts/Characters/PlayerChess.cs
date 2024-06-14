using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerChess : BaseChess, IPlayer
{
    public void Move(Vector3Int position)
    {
        Vector3 newPosition = board.Grid.GetCellCenterWorld(position);
        newPosition.y = yOffset;
        current3DCellPosition = position;
        current2DCellPosition.x = position.x;
        current2DCellPosition.y = position.z;
        transform.DOJump(newPosition, jumpForce, jumpNumber, duration).OnComplete(OnMoveComplete);
        AudioManager.PlayerOneShotAudio(audioSource, moveAudio);
    }

    private void OnMoveComplete()
    {
        board.ChangeTurnAction?.Invoke(TurnType.Enemy);
    }

    public void Kill(IChess chess)
    {
        current3DCellPosition = chess.GetCurrent3DCellPosition();
        current2DCellPosition = chess.GetCurrent2DCellPosition();
       
        transform.DOJump(chess.GetCurrentPosition(), jumpForce*speedScaleKill, jumpNumber, duration/speedScaleKill).OnComplete(() =>
        {
            //Set chess dead
            chess.Dead();
            //Check has another Enemy
            OnMoveComplete();
        });
        AudioManager.PlayerOneShotAudio(audioSource, killAudio);
    }

    
    public override void Dead()
    {
        base.Dead();
        EffectManager.PlayerEffect("whiteDead", transform.position, transform.rotation);    
        board.GameOverAction?.Invoke();
        gameObject.SetActive(false);
        DOTween.Kill(this);
    }
}
