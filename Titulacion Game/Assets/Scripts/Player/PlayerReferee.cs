using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReferee : MonoBehaviour
{
    public enum ActualTargetState
    {
        LIFE,
        WIN,
        DEATH
    }

    [SerializeField]
    public ActualTargetState GameState;


    public Transform respawnPoint;
    public float respawnTime = 5.0f;

    public PlayerController scrPlayerController;

    /// <summary>
    /// Start in LIFE
    /// </summary>
    void Start()
    {
        GameState = ActualTargetState.LIFE;
    }

    /// <summary>
    /// If fltLife is <= 0 is death and destoy gameObject
    /// </summary>
    void Update()
    {
        switch (GameState)
        {
            case ActualTargetState.LIFE:
                if (scrPlayerController.fltLife <= 0f)
                {
                    GameState = ActualTargetState.DEATH;

                }
                break;
            case ActualTargetState.DEATH:
                gameObject.SetActive(false);
                Invoke(nameof(RespawnPlayer), respawnTime);
                break;
        }

    }

    private void RespawnPlayer()
    {

        scrPlayerController.fltLife = 100f;

        transform.position = respawnPoint.position;

        gameObject.SetActive(true);

        GameState = ActualTargetState.LIFE;
    }
}
