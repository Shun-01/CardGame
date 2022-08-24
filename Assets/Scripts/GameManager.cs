using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GamePlayerManager player;
    public GamePlayerManager enemy;
    [SerializeField] AI enemyAI;
    [SerializeField] UIManager uiManager;
    [SerializeField] CardController cardPrefab;

    public Transform playerHandTransform, playerFieldTransform, enemyHandTransform, enemyFieldTransform;

    public bool isPlayerTurn;
    
    int timeCount;

    int turnCount;

    public Transform playerHero, enemyHero;

    List<int> playerDeck = new List<int>();
    List<int> enemyDeck = new List<int>();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        for (int i = 0; i<10; i++)
        {
            playerDeck.Add(UnityEngine.Random.Range(1, 7));
            enemyDeck.Add(UnityEngine.Random.Range(1, 7));
        }
        for (int i = 10; i < 20; i++)
        {
            playerDeck.Add(UnityEngine.Random.Range(7, 12));
            enemyDeck.Add(UnityEngine.Random.Range(7, 12));
        }
        uiManager.HideResultPanel();
        player.Init(playerDeck);
        enemy.Init(enemyDeck);
        timeCount = 20;
        turnCount = 0;
        uiManager.UpdateTime(timeCount);
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
    }
    public void Restart()
    {
        //handとFieldのカードを削除
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }
        turnCount = 0;
        StartGame();
    }
    void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            GiveCardToHand(player.deck, playerHandTransform);
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
    }
    void GiveCardToHand(List<int> deck, Transform hand)
    {
        if (deck.Count == 0)
        {
            return;
        }
        if (turnCount < 8)
        {
            int cardID = deck[0];
            deck.RemoveAt(0);
            CreateCard(cardID, hand);
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                int cardID = deck[0];
                deck.RemoveAt(0);
                CreateCard(cardID, hand);
            }
        }
        turnCount++;
    }
    void CreateCard(int cardID, Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
    }
    void TurnCalc()
    {
        StopAllCoroutines();
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(enemyAI.EnemyTurn());
        }
    }

    IEnumerator CountDown()
    {
        timeCount = 20;
        uiManager.UpdateTime(timeCount);
        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1); //1秒待機
            timeCount--;
            uiManager.UpdateTime(timeCount);
        }
        ChangeTurn();
    }
    public CardController[] GetEnemyFieldCard(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        return playerFieldTransform.GetComponentsInChildren<CardController>();
    }
    public CardController[] GetFriendFieldCard(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        return enemyFieldTransform.GetComponentsInChildren<CardController>();
    }
    public void OnClickTurnEndButton()
    {
        if (isPlayerTurn)
        {
            ChangeTurn();
        }
    }
    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);

        if (isPlayerTurn)
        {
            player.IncreaseManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
        }
        else
        {
            if (turnCount > 2)
            {
                enemy.IncreaseManaCost();
            }
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        TurnCalc();
    }
    public void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList)
        {
            card.SetCanAttack(canAttack);
        }
    }
    void PlayerTurn()
    {
        //フィールドのカードを攻撃可能表示にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);
    }
    public void CardsBattle(CardController attacker, CardController defender) 
    {
        attacker.Attack(defender);
        defender.Attack(attacker);
        attacker.CheckAlive();
        defender.CheckAlive();
    }

    public void AttackHero(CardController attacker)
    {
        if (attacker.model.isPlayerCard)
        {
            enemy.heroHp -= attacker.model.at;
        }
        else
        {
            player.heroHp -= attacker.model.at;
        }
        attacker.SetCanAttack(false);
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }
    public void HealHero(CardController healer)
    {
        if (healer.model.isPlayerCard)
        {
            player.heroHp += healer.model.at;
        }
        else
        {
            enemy.heroHp += healer.model.at;
        }
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }
    public void CheckHeroHp()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
    }
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(heroHp);
    }
    public void ShowDrawResult()
    {
        StopAllCoroutines();
        uiManager.ShowDrawResult();
    }
    
}
