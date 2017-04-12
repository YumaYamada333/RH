﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // カード操作のインターフェース
    public CardManagement cardManage;

    // カード一枚あたりの時間(s)
    public float CPS;

    //// カード間のインターバルタイム
    //public float spaceTime;

    // カード時間
    float cardTime;

    // ゲームの状態
    public enum GameState
    {
        SetCard,
        Acttion
    }
    GameState gameState;

    public AudioClip OK;

    // Use this for initialization
    void Start () {
        gameState = GameState.SetCard;

	}
	
	// Update is called once per frame
	void Update () {
        // 仮　アクションとカードセットを切り替える
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    gameState++;
        //    if (gameState == GameState.Acttion + 1) gameState = GameState.SetCard;
        //    AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        //    audioSource.PlayOneShot(OK);

        //}
       
        switch (gameState)
        {
            // カードセット時の処理
            case GameState.SetCard:

                // カードセットの操作を受けつけるようにする
                cardManage.isControlCard = true;
                //cardManage.ActtionCard(true);
                break;

            // アクション時の処理
            case GameState.Acttion:
                // カードセットの操作を受け付けないようにする
                //cardManage.isControlCard = false;
                cardTime += Time.deltaTime;
                if (cardTime > CPS)
                {
                    GameObject.Find("unitychan").GetComponent<PlayerAction>().ActionPlay(cardManage.ActtionCard(false));
                    //cardManage.ActtionCard(false);
                    cardTime = 0.0f;
                }
                break;
        }

    }

    public void Play()
    {
        gameState++;
        if (gameState == GameState.Acttion + 1) gameState = GameState.SetCard;
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(OK);
    }

    public GameState GetGameState()
    {
        return gameState;
    }
}
