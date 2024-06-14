using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] private float deplayClick = 0.5f;
    [Header("Board Settings:")]
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask cellLayer;
    [SerializeField] private LayerMask chessLayer;
    [SerializeField] private int row;
    [SerializeField] private int col;

    [Header("Chess Player Settings:")]
    [SerializeField] private PlayerChess playerChess;

    [Header("Chess Enemy Settings:")]
    public EnemyPool enemyPool;

    [Header("Chess Obstacle Settings:")]
    public ObstaclePool obstaclePool;

    
    public Vector3Int GetPlayerCurrent3DCell => playerChess.GetCurrent3DCellPosition();
    public Vector2Int GetPlayerCurrent2DCell => playerChess.GetCurrent2DCellPosition();

    public List<IChess> chesses { get; private set; }
    public List<IEnemy> enemies { get; private set; }
    public List<Vector2Int> cellFillPosition { get; private set; }
    public Grid Grid => grid;
    public PlayerChess PlayerChess => playerChess;
    public LayerMask ChessLayer => chessLayer;
    public Action<TurnType> ChangeTurnAction { get; set; }
    public Action GameOverAction { get; set; }
    

    private float curretnTimeDeplay;

    public void Initialize(GameManager manager)
    {
        chesses = new List<IChess>();
        enemies = new List<IEnemy>();
        cellFillPosition = new List<Vector2Int>();
        playerChess.Initialize(this);
        enemyPool.Initlize(this);
        obstaclePool.Initialize(this);
        curretnTimeDeplay = Time.time;
    }

    public void AddGameAction(Action<TurnType> changeTurn, Action gameOver)
    {
        ChangeTurnAction += changeTurn;
        GameOverAction += gameOver; 
    }

    public void RemoveGameAction()
    {
        ChangeTurnAction = null;
        GameOverAction = null;
    }

    #region Set up game Methods
    public void SetDefault()
    {
        //Add Player in list
        playerChess.gameObject.SetActive(true);
        playerChess.Spawn(Vector3Int.zero);
    }

    public void ResetBoard()
    {
        foreach(IChess chess in chesses)
        {
            chess?.Reset();
        }
        chesses.Clear();
        enemies.Clear();
    }
    #endregion

    #region Gameplay Methods
    private void AddInput()
    {
        InputManager.playerInput.Click.performed += OnClick;
    }

    private void RemoveInput()
    {
        InputManager.playerInput.Click.performed -= OnClick;
    }

    public void SpawnObstacle(Action resetCounter)
    {
        //When Full Chess
        if (chesses.Count >= 25 / 2) return;
        ObstacleChess obstacleChess = obstaclePool.GetObstacleChess();
        if(obstacleChess == null) return;
        Vector3Int newCellPostion = GetRandonCellPosition();
        obstacleChess.Spawn(newCellPostion);
        resetCounter?.Invoke();
    }

    public void SpawnEnemy(ChessType[] types)
    {
        if (types == null) return;
        foreach (ChessType type in types)
        {
            //When Full Chess
            if (chesses.Count >= 25) return;
            //When not place for pawn
            if (type == ChessType.Pawn && CheckPawnPawnEnemy()) continue;

            EnemyChess newEnemy = enemyPool.GetEnemy(type);

            if (newEnemy == null) return;
            Vector3Int newCellPostion = (type != ChessType.Pawn)? GetRandonCellPosition() : GetRandonCellPositionPawn();
            newEnemy.Spawn(newCellPostion);
        }
    }

    public void PlayerAction()
    {
        AddInput();
    }

    public void EnemyAction()
    {
        foreach (IEnemy enemy in enemies)
        {
            enemy.AIMove();
        }
        ChangeTurnAction?.Invoke(TurnType.Player);
    }
    #endregion


    #region Caculate Methods
    public bool IsInBoard(Vector3Int cell)
    {
        return !(cell.x < 0 || cell.x >= row || cell.z < 0 || cell.z >= col);
    }

    public bool CheckCellIsHasChess(Vector2Int cell)
    {
        return (GetFillCell().Contains(cell));
    }

    public bool CheckPawnPawnEnemy()
    {
        for (int i = 0; i < 5; i++)
        {
            if (!CheckCellIsHasChess(new Vector2Int(i,4))) return false;
        }
        return true;
    }

    private List<Vector2Int> GetFillCell()
    {
        cellFillPosition.Clear();
        foreach (IChess chess in chesses)
        {
            cellFillPosition.Add(chess.GetCurrent2DCellPosition());
        }
        return cellFillPosition;
    }

    private Vector2Int RandomVector2Int()
    {
        Vector2Int cellRandom = Vector2Int.zero;
        cellRandom.x = UnityEngine.Random.Range(0, row);
        cellRandom.y = UnityEngine.Random.Range(0, col);
        return cellRandom;
    }

    public Vector3Int GetRandonCellPosition()
    {
        Vector2Int newCellPosition;
        do
        {
            newCellPosition = RandomVector2Int();
        } while (CheckCellIsHasChess(newCellPosition));
        return new Vector3Int(newCellPosition.x, 0, newCellPosition.y);
    }

    public Vector3Int GetRandonCellPositionPawn()
    {
        Vector2Int newCellPosition;
        do
        {
            newCellPosition = RandomVector2Int();
            newCellPosition.y = 4;
        } while (CheckCellIsHasChess(newCellPosition));
        return new Vector3Int(newCellPosition.x, 0, newCellPosition.y);
    }

    public bool CheckHasAnyEnemmy()
    {
        return (enemies.Count > 0);
    }
    #endregion


    #region Callback Methods
    private void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (Time.time < curretnTimeDeplay) return;

        Ray ray = Camera.main.ScreenPointToRay(InputManager.playerInput.CastPosition.ReadValue<Vector2>());

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, cellLayer))
        {
            Vector3Int cellClick = grid.WorldToCell(hit.point);

            //Check is on board
            if (!IsInBoard(cellClick)) return;
            //Check distance
            int distance = Mathf.RoundToInt(Vector2Int.Distance(playerChess.GetCurrent2DCellPosition(), new Vector2Int(cellClick.x, cellClick.z)));
            if (distance != 1)
            {
                AudioManager.PlayerOneShotAudio(AudioManager.VFXSource, AudioManager.WrongMoveAudio);
                return;
            }

            Vector3 cellCenter = grid.GetCellCenterWorld(cellClick);

            //Check has any chess in this cell
            Collider[] chessColider = Physics.OverlapBox(cellCenter, Vector3.one * 0.5f, Quaternion.identity, chessLayer);
            foreach (Collider colider in chessColider)
            {
                //Check cell has obstacle
                IObstacle obstacle = colider.GetComponent<IObstacle>();
                if (obstacle != null)
                {
                    AudioManager.PlayerOneShotAudio(AudioManager.VFXSource, AudioManager.WrongMoveAudio);
                    return;
                }
                //Check cell has Enemy
                IEnemy enemy = colider.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    playerChess.Kill(colider.GetComponent<IChess>());
                    curretnTimeDeplay = Time.time + deplayClick;
                    RemoveInput();
                    return;
                }
            }
            //Chess move
            playerChess.Move(cellClick);
            curretnTimeDeplay = Time.time + deplayClick;
            RemoveInput();
        }
    }
    #endregion
}

[Serializable]
public class EnemyPool
{
    [SerializedDictionary("EnemyType", "Prefab")]
    [SerializeField] private SerializedDictionary<ChessType, EnemyChess> enemyDictionary;
    [SerializeField] private Transform enemyParent;
    public Dictionary<ChessType, ChessPool<EnemyChess>> enemyPoolDictionary;

    public void Initlize(ChessBoard board)
    {
        enemyPoolDictionary = new Dictionary<ChessType, ChessPool<EnemyChess>>();
        foreach (KeyValuePair<ChessType, EnemyChess> data in enemyDictionary)
        {
            enemyPoolDictionary.Add(data.Key, new ChessPool<EnemyChess>(data.Value, board, enemyParent));
        }
    }

    public EnemyChess GetEnemy(ChessType type)
    {
        if (enemyPoolDictionary.ContainsKey(type))
        {
            return enemyPoolDictionary[type].GetChess();
        }
        return null;
    }

    public void ReleaveChess(EnemyChess enemy, ChessType type)
    {
        if (enemyPoolDictionary.ContainsKey(type))
        {
            enemyPoolDictionary[type].ReleaveChess(enemy);
        }
    }
}

[Serializable]
public class ObstaclePool
{
    [SerializeField] private ObstacleChess obstacleChessPrefab;
    [SerializeField] private Transform obstacleParent;

    private ChessPool<ObstacleChess> obstaclePool;

    public void Initialize(ChessBoard chessBoard)
    {
        obstaclePool = new ChessPool<ObstacleChess>(obstacleChessPrefab, chessBoard, obstacleParent);
    }

    public ObstacleChess GetObstacleChess()
    {
        return obstaclePool.GetChess();
    }

    public void ReleaveChess(ObstacleChess obstacle)
    {
        obstaclePool.ReleaveChess(obstacle);
    }
}
