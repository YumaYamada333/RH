using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBord : MonoBehaviour {

    // 最大セット枚数
    public const int numSetMax = 50;

    // 配置したカード
    public struct CardData
    {
        public CardManagement.CardType type;
        public GameObject obj;
    }
    public CardData[] cards = new CardData[numSetMax];

    // ボード上のカード数
    public int numSet;

    // カードのサイズ
    Vector2 cardSize;

    // 中心のカード
    int centerCard;

    // 使用中カード
    public int usingCard;

    // 選択中のスペース
    public int selectedSpace;

	// Use this for initialization
	void Start () {
        cardSize = new Vector2(0.8f, 1.0f);
        centerCard = 0;
        selectedSpace = 0;
        //numSet = 0;
        usingCard = 0;
	}
	
	// Update is called once per frame
	void Update () {
        centerCard = usingCard;
        // カードの座標設定
		for (int i = 0; i < numSetMax; i++)
        {
            //条件を満たしたら処理中でもスルー
            if (cards[i].obj == null) break;

            const float zPos = -0.1f;
            cards[i].obj.transform.localPosition = new Vector3((i - centerCard) * cardSize.x, 0.0f, zPos) / transform.localScale.x;
            if ((selectedSpace == i) && selectedSpace >= 0)
                cards[i].obj.transform.localPosition += new Vector3(0, 0, -0.3f);

            //スクロールボタンが押されたら
            if (Input.GetButton("CardScroll"))
            {
                cards[i].obj.transform.Translate(Input.GetAxis("CardScroll"), 0, 0);
            }
        }

        // 使用済みカードの非表示化
        if (true)
        {
            cards[usingCard - 1].obj.SetActive(false);
            cards[usingCard].obj.SetActive(true);
        }
	}

    // ボードにカードをセットする
    public bool SetCard(GameObject obj, CardManagement.CardType type)
    {
        cards[numSet].obj = obj;
        cards[numSet].obj.transform.parent = transform;
        cards[numSet].type = type;
        numSet++;
        return true;
    }

    // カードを削除する
    public bool DeleteCard(int no)
    {
        Destroy(cards[no].obj);
        cards[no].type = CardManagement.CardType.Nothing;  
        numSet--;
        for (int i = no; i < numSetMax - 1; i++)
        {
            cards[i] = cards[i + 1];
        }

        return true;
    }

    // カードを変更する
    public bool ChangeCard(CardData card, int no)
    {
        Destroy(cards[no].obj);
        cards[no] = card;

        return true;
    }

    public bool TuckCard(CardData card, int posNo)
    {
        CardData temp;
        temp = cards[posNo + 1];
        cards[posNo + 1] = cards[posNo];
        for (int i = posNo + 2; i < numSetMax - 1; i++)
        {
            CardData temp2 = cards[i];
            cards[i] = temp;
            temp = temp2;
        }
        
        cards[posNo].obj = Instantiate(card.obj);
        cards[posNo].obj.transform.parent = transform;
        cards[posNo].obj.transform.localScale = card.obj.transform.localScale;
        cards[posNo].type = card.type;
        numSet++;
        return true;
    }

    // 使用中カードの取得
    public CardManagement.CardType GetCardType(int no = -1)
    {
        if (no == -1)
            return cards[usingCard].type;
        else
            return cards[no].type;
    }
}
