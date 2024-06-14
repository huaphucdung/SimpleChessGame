using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public enum ChessType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class BaseChess : MonoBehaviour, IChess
{
    [SerializeField] protected float yOffset = 1.2f;
    [Header("Smooth Animation Setttings:")]
    [SerializeField] protected float duration = 0.3f;
    [SerializeField] protected float jumpForce = 1f;
    [SerializeField] protected int jumpNumber = 1;
    [SerializeField] protected float speedScaleKill = 2f;

    [Header("Sound Setttings:")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip moveAudio;
    [SerializeField] protected AudioClip killAudio;

    protected ChessBoard board;
    protected Vector3Int current3DCellPosition;
    protected Vector2Int current2DCellPosition;

    private CinemachineImpulseSource impulseSource;

    public void Initialize(ChessBoard chessBoard)
    {
        board = chessBoard;
        current2DCellPosition = Vector2Int.zero;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public virtual void Spawn(Vector3Int position)
    { 
        //Set Position
        Vector3 newPosition = board.Grid.GetCellCenterWorld(position);
        newPosition.y = yOffset;
        transform.position = newPosition;

        //Change current cell position
        current3DCellPosition = position;
        current2DCellPosition.x = position.x;
        current2DCellPosition.y = position.z;
        
        // Set the initial scale to 0
        transform.localScale = Vector3.zero;
        // Animate the scale to 1 over the specified duration
        transform.DOScale(Vector3.one, duration);
        
        //Add In list
        board.chesses.Add(this);
    }

    public Vector3Int GetCurrent3DCellPosition()
    {
        return current3DCellPosition;
    }

    public Vector2Int GetCurrent2DCellPosition()
    {
        return current2DCellPosition;
    }

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    public virtual void Dead()
    {
        board.chesses.Remove(this);
        impulseSource?.GenerateImpulse();
    }

    public virtual void Reset()
    {
        
    }
}
