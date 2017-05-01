using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReturn : MonoBehaviour
{
    public struct DATA
    {
        public CardBord.CardData[] card;        // ひとつ前のActionBoard情報
        public int[] cardNum;                   // 使ったカードの配列番号
    }

    [SerializeField]
    private int dataNum = 3;              // データの保持数
    private DATA[] cardData;              // データの保存場所
    private int returnNum;                // 巻き戻し回数

    // Use this for initialization
    void Start()
    {
        //非表示にしておく
        gameObject.SetActive(false);

        // ボード情報を保存する領域を作成
        cardData = new DATA[dataNum];
        for (int i = 0; i < cardData.Length; i++)
        {
            cardData[i].card = new CardBord.CardData[CardBord.numSetMax];
            cardData[i].cardNum = new int[CardManagement.numMax];
            for (int j = 0; j < cardData.Length; j++)
            {
                cardData[i].card[j].type = CardManagement.CardType.Nothing;
            }
        }
        returnNum = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        // ひとつ前の情報が存在する
        if (returnNum > 0 && cardData[returnNum - 1].card != null)
        {
            GameObject.Find("CardManager").GetComponent<CardManagement>().ReturnBoard(cardData[returnNum - 1]);
            returnNum--;
        }
    }

    // 現在のボード情報を保存
    public void SaveCard(CardBord.CardData[] A_card, CardManagement.CardData[] H_card, GameObject tuckCard1 = null, GameObject tuckCard2 = null)
    {
        // カードを保存する領域がある
        if (returnNum < dataNum)
        {
            for (int i = 0; i < A_card.Length; i++)
            {
                // ActionBoardのカード情報を保存
                cardData[returnNum].card[i].obj = A_card[i].obj;
                if (cardData[returnNum].card[i].obj != null)
                    cardData[returnNum].card[i].type = A_card[i].type;
                else
                    cardData[returnNum].card[i].type = CardManagement.CardType.Nothing;
            }

            // HandsBoardのカード所持数を保存
            for (int j = 0; j < H_card.Length; j++)
                cardData[returnNum].cardNum[j] = H_card[j].numHold;

            returnNum++;
        }
        else
        {
            SwapCard(A_card, H_card);
        }
    }

    // ボード情報の上書き
    private void SwapCard(CardBord.CardData[] A_card, CardManagement.CardData[] H_card)
    {
        // １番古い情報を上書き
        for (int i = 0; i < cardData.Length - 1; i++)
        {
            for (int j = 0; j < A_card.Length; j++)
            {
                // ActionBoardのカード情報を保存
                cardData[i].card[j].obj = cardData[i + 1].card[j].obj;
                if (cardData[i + 1].card[j].obj != null)
                    cardData[i].card[j].type = cardData[i + 1].card[j].type;
                else
                    cardData[i].card[j].type = CardManagement.CardType.Nothing;
            }
        }

        for (int i = 0; i < A_card.Length; i++)
        {
            // ActionBoardのカード情報を保存
            cardData[cardData.Length - 1].card[i].obj = A_card[i].obj;
            if (cardData[cardData.Length - 1].card[i].obj != null)
                cardData[cardData.Length - 1].card[i].type = A_card[i].type;
            else
                cardData[cardData.Length - 1].card[i].type = CardManagement.CardType.Nothing;
        }

        // HandsBoardのカード所持数を保存
        for (int j = 0; j < H_card.Length; j++)
            cardData[cardData.Length - 1].cardNum[j] = H_card[j].numHold;
    }
}
