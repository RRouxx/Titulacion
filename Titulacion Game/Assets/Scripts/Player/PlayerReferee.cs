using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [Header ("Life")]
    [SerializeField]
    public float currentLife = 100f;
    [SerializeField]
    public float maxLife = 100f;


    [Header("Life")]
    [SerializeField]
    public Image barLife;
    public Text textLife;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnTime = 5.0f;

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
        RefreshUI();

        switch (GameState)
        {
            case ActualTargetState.LIFE:
                if (currentLife <= 0f)
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

        currentLife = 100f;

        transform.position = respawnPoint.position;

        gameObject.SetActive(true);

        GameState = ActualTargetState.LIFE;
    }

    private void RefreshUI()
    {

        barLife.fillAmount = currentLife / maxLife;
        textLife.text = currentLife.ToString("f0");
    }
    public void TakeDamage(float damage)
    {

        currentLife -= damage;
    }
}
