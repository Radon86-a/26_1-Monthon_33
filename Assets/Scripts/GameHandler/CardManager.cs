using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Prefabs & Parents")]
    [SerializeField] private Card cardPrefab;
    [SerializeField] private RectTransform handContainer; // UI用の親RectTransform

    [Header("Layout Settings")]
    [SerializeField] private float cardSpacing = 1.5f; // カード同士の間隔

    private readonly List<Card> activeCards = new List<Card>();

    /// <summary>
    /// カードIDのリストを受け取り、手札を一括生成して等間隔に配置する
    /// </summary>
    /// <param name="cardIds">生成したいカードIDのリスト</param>
    public void CreateCardsFromList(List<GameCardData> gameCardData)
    {
        // 1. 既存のカードをすべて破棄してクリア
        ClearCards();

        if (gameCardData == null) return;

        // 2. リスト内の各IDに対応するカードを生成
        for (int i = 0; i < gameCardData.Count ; i++)
        {
            Card newCard = Instantiate(cardPrefab, handContainer);
            newCard.Setup(gameCardData[i].card_id, OnCardClicked);
            activeCards.Add(newCard);
        }

        // 3. 等間隔・中央揃えに配置更新
        UpdateLayout();
    }

    /// <summary>
    /// 単一カードを追加したい場合
    /// </summary>
    public void AddCard(int cardId)
    {
        Card newCard = Instantiate(cardPrefab, handContainer);
        newCard.Setup(cardId);
        activeCards.Add(newCard);

        UpdateLayout();
    }

    /// <summary>
    /// カードがクリックされたときの共通処理
    /// </summary>
    private void OnCardClicked(int card_id)
    {
        Debug.Log($"カードがクリックされました: ID = {card_id}");
        cardPrefab.UseCard(card_id);
    }

    /// <summary>
    /// 全カードのX座標を再計算して中央揃え・等間隔に整列
    /// </summary>
    public void UpdateLayout()
    {
        int count = activeCards.Count;
        if (count == 0) return;

        // 全体の幅の中央を原点(0)に合わせるオフセット計算
        float totalWidth = (count - 1) * cardSpacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = activeCards[i].transform.localPosition;
            pos.x = startX + (i * cardSpacing);
            activeCards[i].transform.localPosition = pos;
        }
    }

    /// <summary>
    /// カードを手札から削除
    /// </summary>
    public void RemoveCard(Card card)
    {
        if (activeCards.Remove(card))
        {
            Destroy(card.gameObject);
            UpdateLayout();
        }
    }

    /// <summary>
    /// 手札の全削除
    /// </summary>
    public void ClearCards()
    {
        foreach (var card in activeCards)
        {
            if (card != null) Destroy(card.gameObject);
        }
        activeCards.Clear();
    }
    public List<GameCardData> SortCard(List<GameCardData> list)
    {
        list.Sort((a, b)=> a.card_id.CompareTo(b.card_id));
        return list;
    }
}
