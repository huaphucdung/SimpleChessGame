using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleChess : BaseChess, IObstacle
{
    public override void Dead()
    {
        base.Dead();
        EffectManager.PlayerEffect("whiteDead", transform.position, transform.rotation);
        board.obstaclePool.ReleaveChess(this);
        DOTween.Kill(this);
    }

    public override void Reset()
    {
        board.obstaclePool.ReleaveChess(this);
        DOTween.Kill(this);
    }
}
