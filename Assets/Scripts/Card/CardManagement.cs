﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManagement : MonoBehaviour
{
    /* 説明
    カード処理系の管理クラス
    処理はここで一括管理する
    */

    // カードを乗せるボードの取得
    public GameObject handsBord;
    Vector2 handsBordSize;
    public GameObject actionBord;
    Vector2 actionBordSize;

    // カード
    public GameObject moveCard;
    public GameObject jumpCard;
    public GameObject attackCard;
    public GameObject finishCard;
    Vector2 cardSize;

    // オリジナルの数字UI
    public GameObject originalnumUI;

    // 所持カードの種類数
    int numCardSet;

    // カード最大の所持数
    public const int numMax = 99;  

    // カードの種類
    public enum CardType
    {
        Move,
        Jump,
        Attack,
        Finish,
        Nothing,
        NumType
    }

    //初期カードの種類

    // カードの種類数配列を確保
    public struct CardData
    {
        // 前のカード 後ろのカード
        public CardBord.CardData front;
        public CardBord.CardData back;
        // 残り枚数の表示
        public GameObject numUI;
        // 所持数
        public int numHold;
    }
    public CardData[] cards = new CardData[numMax];

    // 最初のカードの配置位置
    Vector2 firstPos;
    // 現在の配置数
    int numSetting;
    // 配置時の追加空間
    public float cardSpace;
    // カード配置の間隔(m)
    float posInterval;
    // 選択中のカード
    int selectedCard;
    // 挟むカードデータを保持
    CardData tuckCard;

    // カーソルのフォーカス
    public enum CursorForcusTag
    {
        HandsBord,
        ActtionBord
    }
    CursorForcusTag cursor;

    // 更新フラグ
    public bool isUpdateData;

    // カード操作を有効にする
    public bool isControlCard;

    // マウスのコンポーネント
    MouseSystem mouse_system;

    // Rayに触れたオブジェクト
    RaycastHit[] hit;

    //初期所持カード配置関数
    public void Possession()
    {
        // ステージのカード配置
        CardBord bord = actionBord.GetComponent<CardBord>();
        bord.SetCard(Instantiate(moveCard), CardType.Move);
        bord.SetCard(Instantiate(moveCard), CardType.Move);
        bord.SetCard(Instantiate(moveCard), CardType.Move);
        bord.SetCard(Instantiate(finishCard), CardType.Finish);

    }

    // Use this for initialization
    public void Start () {
        isUpdateData = true;
        isControlCard = true;
        // サイズの取得(m)
        handsBordSize = new Vector2(handsBord.transform.lossyScale.x, handsBord.transform.lossyScale.y);
        actionBordSize = new Vector2(actionBord.transform.lossyScale.x, actionBord.transform.lossyScale.y);
        cardSize = new Vector2(attackCard.transform.localScale.x, attackCard.transform.localScale.y);

        firstPos = new Vector2(cardSize.x / 2 + cardSpace - handsBordSize.x / 2, 0.0f);

        numCardSet = 0;
        numSetting = 0;
        selectedCard = 0;

        cursor = CursorForcusTag.HandsBord;

        // 仮所持カード
        SetCard(CardType.Attack, CardType.Attack);
        SetCard(CardType.Attack, CardType.Attack);
        SetCard(CardType.Attack, CardType.Attack);
        SetCard(CardType.Attack, CardType.Attack);
        SetCard(CardType.Jump, CardType.Jump);
        SetCard(CardType.Jump, CardType.Jump);
        SetCard(CardType.Jump, CardType.Jump);

        //初期所持カード
        Possession();

        // MouseSystemコンポーネントの取得
        mouse_system = GameObject.Find("MouseSystem").GetComponent<MouseSystem>();
    }

    // Update is called once per frame
    void Update () {

        // データの更新
        if (isUpdateData) UpdateData();

        // カード操作
        if (isControlCard) ControlCard();

        // 所持カードの更新
        for (int i = 0; i < numCardSet; i++)
        {
            // オブジェクトの存在しないカードの生成
            if (/*cards[i].numHold > 0 && */cards[i].front.obj == null && cards[i].back.obj == null)
            {
                CreateCards(ref cards[i].front);
                CreateCards(ref cards[i].back);
                // 残り枚数のUIの生成
                cards[i].numUI = Instantiate(originalnumUI);
                cards[i].numUI.transform.parent = handsBord.transform;

            }

            // 枚数0所持カードの破棄
            if (cards[i].numHold == 0 && cards[i].front.obj == null && cards[i].back.obj == null)
            {
                //カードの破棄
                DestroyCards(ref cards[i].front);
                DestroyCards(ref cards[i].back);
                // 残り枚数のUIの破棄
                Destroy(cards[i].numUI);
            }

            // 残り枚数のUIの更新
            cards[i].numUI.GetComponent<TextMesh>().text = cards[i].numHold.ToString();

            if (cursor == CursorForcusTag.HandsBord)
            {
                // カードの配置
                SetCardPosition(ref cards[i]);
            }

            // UIの配置
            cards[i].numUI.transform.position = cards[i].back.obj.transform.position;

        }
        numSetting = 0;

        ////デバック用
        //Getflag();

    }

    // 設定する際に使用するデータの更新
    void UpdateData()
    {
        // 所持カードの所持数
        int numTypeHold = 0;
        for (int i = 0; i < numCardSet; i++)
        {
            if (cards[i].numHold > 0) numTypeHold++;
        };

        // カード間の距離
        if (cardSize.x * numTypeHold + cardSpace < handsBordSize.x)
            posInterval = cardSize.x + cardSpace;
        else
            posInterval = handsBordSize.x / numTypeHold;

        //isUpdateData = false;

    }

    // カード操作
    void ControlCard()
    {
        // はさむカード選択中 ===============================================================
        if (cursor == CursorForcusTag.HandsBord)
        {
            // 左クリックした座標にあるカードをつかむ
            if (Input.GetMouseButton(0))
            {
                // Rayに触れたオブジェクトをすべて取得
                hit = mouse_system.GetReyhitObjects();

                if (hit[hit.Length - 1].collider.tag == "Card")
                {
                    for (int i = 0; i < cards.Length; i++)
                    {
                        if (cards[i].front.obj != null)
                        {
                            // Rayに触れているオブジェクトとボードのカードの座標が同じ
                            if (hit[hit.Length - 1].transform.position == cards[i].front.obj.transform.position)
                            {
                                // はさむカードに設定
                                selectedCard = i;
                                tuckCard = cards[selectedCard];
                                break;
                            }
                        }
                    }
                    cursor = CursorForcusTag.ActtionBord;
                    actionBord.GetComponent<CardBord>().selectedSpace = -1;
                }
            }
        }
        // はさむ場所を選択中 ===============================================================
        else
        {
            // ActionBoardの情報を取得
            CardBord bord = actionBord.GetComponent<CardBord>();

            // 左クリックしてる
            if (Input.GetMouseButton(0))
            {
                // カードを移動
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider.tag == "Card")
                    {
                        GameObject parent_obj = hit[i].transform.parent.gameObject;
                        parent_obj.transform.position = mouse_system.GetWorldPos();
                    }
                }
            }
            // してない
            else
            {
                cursor = CursorForcusTag.HandsBord;

                // カメラからはさむカードの傾きを取得
                Vector3 pos = hit[hit.Length - 1].transform.position - GameObject.FindGameObjectWithTag("MainCamera").transform.position;

                // Rey座標を設定
                Ray ray = new Ray(hit[hit.Length - 1].transform.position, pos);
                RaycastHit select_card;

                // Rayに触れたカードをはさむ
                if (Physics.Raycast(ray, out select_card))
                {
                    if (select_card.collider.tag == "Card")
                    {
                        for (int i = 0; i < bord.cards.Length; i++)
                        {
                            //挟みたいカードとボードのカード座標が同じ
                            if (bord.cards[i].obj.transform.position == select_card.transform.position)
                            {
                                bord.selectedSpace = i;

                                //GameObject.Find("Return").GetComponent<CardReturn>().SaveCard(bord.cards, cards);

                                // 挟んだカードが同タイプ
                                if (bord.GetCardType(bord.selectedSpace) == tuckCard.front.type
                                    && bord.GetCardType(bord.selectedSpace) == tuckCard.back.type)
                                {
                                    // カードの効果を変える
                                    CardBord.CardData newCard;
                                    newCard.type = DecideTuckCard(tuckCard.front.type, tuckCard.back.type);
                                    newCard.obj = null;
                                    //新しいカードの生成
                                    CreateCards(ref newCard);
                                    if (newCard.obj != null)
                                    {
                                        // 挟まれたカードの削除
                                        bord.DeleteCard(bord.selectedSpace);
                                        // 上記の位置に新しいカード
                                        bord.TuckCard(newCard, bord.selectedSpace);
                                    }
                                    else
                                    {
                                        bord.TuckCard(tuckCard.front, bord.selectedSpace);
                                        bord.TuckCard(tuckCard.back, bord.selectedSpace + 2);
                                    }
                                    //カードの破棄
                                    Destroy(newCard.obj);

                                }
                                else
                                {
                                    bord.TuckCard(tuckCard.front, bord.selectedSpace);
                                    bord.TuckCard(tuckCard.back, bord.selectedSpace + 2);
                                }

                                // セットしたカード枚数を減らす
                                cards[selectedCard].numHold--;

                                break;
                            }
                        }
                    }
                }
            }

            if (bord.usingCard > bord.selectedSpace)
            {
                bord.selectedSpace = bord.usingCard;
            }
            else if (bord.numSet - 1 <= bord.selectedSpace)
            {
                bord.selectedSpace = bord.numSet - 1;
            }
        }

    }

    // 存在しないカードの生成
    void CreateCards(ref CardBord.CardData card)
    {
        if (card.obj == null)
        {
            switch (card.type)
            {
                case CardType.Move:
                    card.obj = Instantiate(moveCard);
                    break;
                case CardType.Jump:
                    card.obj = Instantiate(jumpCard);
                    break;
                case CardType.Attack:
                    card.obj = Instantiate(attackCard);
                    break;
                case CardType.Finish:
                    card.obj = Instantiate(finishCard);
                    break;
            }
            card.obj.transform.parent = handsBord.transform; 
        }
    }

    // カード破棄
    public void DestroyCards(ref CardBord.CardData card)
    {
            Destroy(card.obj);
    }

    // 存在する所持カードの配置
    void SetCardPosition(ref CardData card)
    {
        const float zPos = -0.1f;
        // numSetting番目の位置に配置
        card.front.obj.transform.localPosition 
            = new Vector3(firstPos.x + numSetting * posInterval, firstPos.y, zPos) / handsBord.transform.localScale.x;
        if (numSetting == selectedCard && cursor == CursorForcusTag.HandsBord)
            card.front.obj.transform.localPosition += new Vector3(0, 0, -0.1f);
        card.back.obj.transform.position = card.front.obj.transform.position + new Vector3(0.2f, 0.01f, 0.0f);
        numSetting++;
    }

    // 所持カードに加える
    void SetCard(CardType front, CardType back, int num = 1)
    {
        cards[numCardSet].front.type = front;
        cards[numCardSet].back.type = back;
        cards[numCardSet].numHold += num;
        numCardSet++;
    }

    // 挟んだ後のカードを決める
    CardType DecideTuckCard(CardType type, CardType type1)
    {
        CardType result;
        // 桁をずらす
        const int back = 10;
        switch ((int)type + (int)type1 * back)
        {
            // 仮
            case (int)CardType.Attack + (int)CardType.Attack * back:
                result = CardType.Jump;
                break;
            default:
                result = CardType.Nothing;
                break;
        }
        return result;
    }


    // カードを進める
    public CardType ActtionCard(bool isReset)
    {
        CardBord bord = actionBord.GetComponent<CardBord>();
        if (!isReset)
        {
            CardType type = bord.GetCardType();
            if (bord.usingCard < bord.numSet)
                bord.usingCard++;
            else
                type = CardType.Nothing;
            return type;
        }
        else
        {
            bord.usingCard = 0;
            return bord.GetCardType();
        }
    }

    //  カードの情報関係 /////////////////////////////////////////////////////////////////////////////////

    // ボード情報を初期状態に戻す
    public void ReturnBoard(ResetScript.INITDATA data)
    {
        // ActionBoardの情報を取得
        CardBord bord = actionBord.GetComponent<CardBord>();

        // カードの削除
        for (int i = 0; i < bord.cards.Length; i++)
        {
            if (bord.cards[i].obj != null)
                Destroy(bord.cards[i].obj);
        }

        // カードの再配置
        bord.numSet = 0;
        for (int i = 0; i < data.setCard.Length; i++)
        {
            switch (data.setCard[i].type)
            {
                case CardType.Move:
                    bord.SetCard(Instantiate(moveCard), data.setCard[i].type);
                    break;
                case CardType.Jump:
                    bord.SetCard(Instantiate(jumpCard), data.setCard[i].type);
                    break;
                case CardType.Attack:
                    bord.SetCard(Instantiate(attackCard), data.setCard[i].type);
                    break;
                case CardType.Finish:
                    bord.SetCard(Instantiate(finishCard), data.setCard[i].type);
                    break;
                default:
                    break;
            }
        }
        bord.Coordinate();

        //// カード所持数を戻す
        //for (int i = 0; i < cards.Length; i++)
        //    cards[i].numHold = data.cardNum[i];
    }

    //// ボード情報を戻す関数のオーバーロード
    //public void ReturnBoard(CardReturn.DATA data)
    //{
    //    // ActionBoardの情報を取得
    //    CardBord bord = actionBord.GetComponent<CardBord>();

    //    // カードの削除
    //    for (int i = 0; i < bord.cards.Length; i++)
    //    {
    //        if (bord.cards[i].obj != null)
    //            Destroy(bord.cards[i].obj);
    //    }

    //    // カードの再配置
    //    bord.numSet = 0;
    //    for (int i = 0; i < data.card.Length; i++)
    //    {
    //        switch (data.card[i].type)
    //        {
    //            case CardType.Move:
    //                bord.SetCard(Instantiate(moveCard), data.card[i].type);
    //                break;
    //            case CardType.Jump:
    //                bord.SetCard(Instantiate(jumpCard), data.card[i].type);
    //                break;
    //            case CardType.Attack:
    //                bord.SetCard(Instantiate(attackCard), data.card[i].type);
    //                break;
    //            case CardType.Finish:
    //                bord.SetCard(Instantiate(finishCard), data.card[i].type);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    bord.Coordinate();

    //    // カード所持数を戻す
    //    for (int i = 0; i < cards.Length; i++)
    //        cards[i].numHold = data.cardNum[i];
    //}
}
