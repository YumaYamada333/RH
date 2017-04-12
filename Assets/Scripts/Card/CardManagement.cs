using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManagement : MonoBehaviour {

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
    const int numMax = 99;  

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

    // カードの種類数配列を確保
    struct CardData
    {
        // 前のカード 後ろのカード
        public CardBord.CardData front;
        public CardBord.CardData back;
        // 残り枚数の表示
        public GameObject numUI;
        // 所持数
        public int numHold;
    }
    CardData[] cards = new CardData[numMax];

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
    enum CursorForcusTag
    {
        HandsBord,
        ActtionBord
    }
    CursorForcusTag cursor;

    // 更新フラグ
    public bool isUpdateData;

    // カード操作を有効にする
    public bool isControlCard;

    // Use this for initialization
    void Start () {
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
        SetCard(CardType.Attack, CardType.Attack, 10);
        SetCard(CardType.Move, CardType.Move);
        SetCard(CardType.Jump, CardType.Jump);
        SetCard(CardType.Jump, CardType.Jump);


        // ステージの仮のmoveカード配置
        CardBord bord = actionBord.GetComponent<CardBord>();
        bord.SetCard(Instantiate(moveCard), CardType.Move);
        bord.SetCard(Instantiate(moveCard), CardType.Move);
        bord.SetCard(Instantiate(moveCard), CardType.Move);
        bord.SetCard(Instantiate(finishCard), CardType.Finish);
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
            if (cards[i].numHold > 0 && cards[i].front.obj == null && cards[i].back.obj == null)
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
                DestroyCards(ref cards[i].front);
                DestroyCards(ref cards[i].back);
                // 残り枚数のUIの破棄
                Destroy(cards[i].numUI);
            }

            // 残り枚数のUIの更新
            cards[i].numUI.GetComponent<TextMesh>().text = cards[i].numHold.ToString();

            // カードの配置
            SetCardPosition(ref cards[i]);

            // UIの配置
            cards[i].numUI.transform.position = cards[i].back.obj.transform.position;
        }
        numSetting = 0;
    }

    // 設定する際に使用するデータの更新
    void UpdateData()
    {
        // 所持カードの所持数
        int numTypeHold = 0;
        for (int i = 0; i< numCardSet; i++)
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
        if (cursor == CursorForcusTag.HandsBord)
        {

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedCard++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedCard--;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                cursor = CursorForcusTag.ActtionBord;
                tuckCard = cards[selectedCard];
            }
            //actionBord.GetComponent<CardBord>().selectedSpace = -1;
        }
        else
        {
            CardBord bord = actionBord.GetComponent<CardBord>();
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                bord.selectedSpace++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                bord.selectedSpace--;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                cursor = CursorForcusTag.HandsBord;

                // 挟んだカードが同タイプ
                if (bord.GetCardType(bord.selectedSpace) == tuckCard.front.type
                    && bord.GetCardType(bord.selectedSpace) == tuckCard.back.type)
                {
                    // カードの効果を変える
                    CardBord.CardData newCard;
                    newCard.type = DecideTuckCard(tuckCard.front.type, tuckCard.back.type);
                    newCard.obj = null;
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

                }
                else
                {
                    bord.TuckCard(tuckCard.front, bord.selectedSpace);
                    bord.TuckCard(tuckCard.back, bord.selectedSpace + 2);
                }

                // セットしたカード枚数を減らす
                cards[selectedCard].numHold--;
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
    void DestroyCards(ref CardBord.CardData card)
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
}
