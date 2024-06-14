using AYellowpaper.SerializedCollections;
using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Manager Settings:")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManger;
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private LevelDesign levelDesign;

    [Header("Camera Settings:")]
    [SerializeField] private CinemachineVirtualCamera CVCamera;
    [SerializeField] private CinemachineInputProvider CInputProvider;
    [SerializeField] private int numberTurnToSpawn = 3;

    public ChessBoard chessBoard { get; private set; }
    public CinemachinePOV cameraPOV { get; private set; }
    private TurnType currentTurn;
    private int playerTurnCounter;
    private int currentLevel;

    private bool isNewGame;
    private bool isReturnMenuGame;

    private void Awake()
    {
        cameraPOV = CVCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Start()
    {
        Application.targetFrameRate = 90;
        InputManager.Initialize();
        InputManager.EnableInput();
        CInputProvider.XYAxis = InputActionReference.Create(InputManager.playerInput.Look);
        chessBoard = GetComponent<ChessBoard>();
        chessBoard.Initialize(this);
        uiManager.Initialize();
        uiManager.SetDefault();
        audioManger.Initilize();
        effectManager.Initialize();
        MainMenu();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    #region Game State Methods
    private void MainMenu()
    {
        isReturnMenuGame = false;
        //Show Menu UI
        cameraPOV.m_HorizontalAxis.Value = 45;
        InputManager.ActiveInputRotation(true);
        uiManager.ChangeUI<MainMenuUI>(new MainMenuUIData { startGameAction = StartGame, settingAction = SettingMenu });
        AudioManager.PlayAudio(AudioManager.MusicSource, AudioManager.BackgroundAudio);
    }

    private void StartGame()
    {
        if (isNewGame) return;
        DOTween.KillAll();
        isNewGame = true;
        currentLevel = 0;
        playerTurnCounter = 0;
        uiManager.ChangeScreen(SetNewGame, GamePlaying);
    }

    private void SetNewGame()
    {
        AudioManager.StopPlay(AudioManager.MusicSource);
        //Reset Board
        chessBoard.ResetBoard();
        //Set Default
        chessBoard.SetDefault();
        //Spawn Enemy by level
        UpdateLevel();
        cameraPOV.m_HorizontalAxis.Value = 45;
        uiManager.ChangeUI<GameplayUI>(new GameplayUIData { pauseGameAction = PauseGame });
        uiManager.UpdateData(new GameplayUIUpdateData { level = currentLevel });
        ChangeTurn(TurnType.Player);
    }

    private void GamePlaying()
    {
        isNewGame = false;
        InputManager.ActiveInputClicked(true);
        InputManager.ActiveInputRotation(true);
        chessBoard.AddGameAction(ChangeTurn, GameOver);
    }

    private void PauseGame()
    {
        //Can replay
        InputManager.ActiveInputClicked(false);
        InputManager.ActiveInputRotation(false);
        uiManager.ChangeUI<PauseMenuUI>(new PauseMenuUIData { continueGameAction = CancelPause, replayGameAction = Replay, menuAction = ReturnMenu, settingAction = SettingMenu });
        chessBoard.RemoveGameAction();
    }

    private void CancelPause()
    {
        //Change into gameplay UI
        uiManager.ChangeUI<GameplayUI>(new GameplayUIData { pauseGameAction = PauseGame });
        uiManager.UpdateData(new GameplayUIUpdateData { level = currentLevel });
        GamePlaying();
    }

    private void GameOver()
    {
        //End game and can replay
        InputManager.ActiveInputClicked(false);
        InputManager.ActiveInputRotation(false);
        DOTween.KillAll();
        int bestLevel = PlayerPrefs.GetInt("HighScore", 0);

        if(bestLevel < currentLevel)
        {
            PlayerPrefs.SetInt("HighScore", currentLevel);
            bestLevel = currentLevel;
        }

        uiManager.ChangeUI<GameOverUI>(new GameOverUIData { replayGameAction = Replay, menuGameAction = ReturnMenu, currentLevel = currentLevel, bestLevel = bestLevel});
        chessBoard.RemoveGameAction();
    }

    private void ReturnMenu()
    {
        if (isReturnMenuGame) return;
        DOTween.KillAll();
        isReturnMenuGame = true;
        //Reset Board
        chessBoard.ResetBoard();
        uiManager.ChangeScreen(MainMenu, null);
    }

    private void Replay()
    {
        chessBoard.RemoveGameAction();
        //Start game
        StartGame();
    }

    private void SettingMenu()
    {
        uiManager.ChangeUI<SettingUI>(new SettingUIData
        {
            isMuteMussic = AudioManager.IsMuteMusic(),
            isMuteVfs = AudioManager.IsMuteVFS(),
            musicAudioChange = AudioManager.ChangeMusicAudioSetting,
            vfsAudioChange = AudioManager.ChangeVfsAudioSetting,
            previousUIAction = uiManager.ChangePreviousUI
        });
    }
    #endregion

    #region Main Methods
    public void ChangeTurn(TurnType turn)
    {
        currentTurn = turn;

        switch (currentTurn)
        {
            case TurnType.Player:
                DoPlayerTurn();
                break;
            case TurnType.Enemy:
                DoEnemyTurn();
                break;
        }
    }

    private void DoPlayerTurn()
    {
        //Player Turn
        chessBoard.PlayerAction();
        if (playerTurnCounter >= numberTurnToSpawn)
        {
            //Spawn obstacle
            SpawnObstacle();

            return;
        }
        playerTurnCounter++;
    }

    private void DoEnemyTurn()
    {
        //Enemy Turn
        if (chessBoard.CheckHasAnyEnemmy())
        {
            chessBoard.EnemyAction();
        }
        else
        {
            UpdateLevel();
            ChangeTurn(TurnType.Player);
        }
    }

    private void SpawnObstacle()
    {
        chessBoard.SpawnObstacle(ResetCounter);
    }

    private void ResetCounter()
    {
        playerTurnCounter = 0;
    }

    public void UpdateLevel()
    {
        currentLevel++;
        uiManager.UpdateData(new GameplayUIUpdateData { level = currentLevel });
        chessBoard.SpawnEnemy(levelDesign.GetAllEnemyTypeByLevel(currentLevel));
    }
    #endregion
}

public enum TurnType
{
    Player,
    Enemy
}

[Serializable]
public class LevelDesign
{
    [SerializedDictionary("Level", "Enemies")]
    [SerializeField] private SerializedDictionary<int, ChessType[]> levelDictionary;

    public ChessType[] GetAllEnemyTypeByLevel(int level)
    {
        if (levelDictionary.ContainsKey(level))
        {
            return levelDictionary[level];
        }
        return levelDictionary[-1];
    }

}