using UnityEngine;
using UnityEngine.Pool;

public class ChessPool<T> where T : BaseChess
{
    private IObjectPool<T> pool;
    private T prefab;
    private Transform parentTransform;
    private ChessBoard board;
    public ChessPool(T prefab, ChessBoard board, Transform parentTransform)
    {
        this.prefab = prefab;
        this.board = board;
        this.parentTransform = parentTransform;
        pool = new ObjectPool<T>(CreateEnemy, OnGetEnemy, OnReturnEnemy, OnDestroyEnemy);
    }

    public T GetChess()
    {
        return pool.Get();
    }

    public void ReleaveChess(T chess)
    {
        pool.Release(chess);
    }

    private T CreateEnemy()
    {
        T newObstacle = GameObject.Instantiate(prefab, parentTransform);
        newObstacle.Initialize(board);
        return newObstacle;
    }

    private void OnGetEnemy(T chess)
    {
        chess.gameObject.SetActive(true);
    }

    private void OnReturnEnemy(T chess)
    {
        chess.gameObject.SetActive(false);
    }

    private void OnDestroyEnemy(T chess)
    {
        GameObject.Destroy(chess.gameObject);
    }
}
