using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public IEnumerator EnemyTurn()
    {
        if (gameManager.enemy.deck.ToArray().Length <= 0 && gameManager.player.deck.ToArray().Length <= 0
            && gameManager.enemyHandTransform.GetComponentsInChildren<CardController>().Length <= 0 && gameManager.playerHandTransform.GetComponentsInChildren<CardController>().Length <= 0)
        {
            gameManager.ShowDrawResult();
        }
        //フィールドのカードを攻撃可能にする
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1);
        
        /*場にカードを出す*/
        //手札のカードリストを取得
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        //コスト以下のカードがあれば、カードをフィールドに出し続ける
        //条件：モンスターカードならコストのみ
        //条件：スペルカードならコストと使用可能かどうか (CanUseSpell)
        while (Array.Exists(handCardList, card => 
        (card.model.cost <= gameManager.enemy.manaCost)
        && (!card.IsSpell || (card.IsSpell && card.CanUseSpell()))
        ))
        {
            //コスト以下のカードリストを取得
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card => 
            (card.model.cost <= gameManager.enemy.manaCost)
            && (!card.IsSpell || (card.IsSpell && card.CanUseSpell()))
            );
            //Fieldに出すカードを選択
            CardController selectCard = selectableHandCardList[0];
            //カードを表にする  
            selectCard.Show();
            //スペルカードなら使用する
            if (selectCard.IsSpell)
            {
                StartCoroutine(CastSpell(selectCard));
            }
            else
            {
                //カードを移動
                StartCoroutine(selectCard.movement.MoveToField(gameManager.enemyFieldTransform));
                selectCard.model.isFieldCard = true;
                selectCard.OnField();
            }
            yield return new WaitForSeconds(1);
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        yield return new WaitForSeconds(1);

        /*攻撃*/
        //フィールドのカードリストを取得
        CardController[] fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        //攻撃可能カードがあれば攻撃を繰り返す
        while (Array.Exists(fieldCardList, card => card.model.canAttack))
        {
            //攻撃可能カードを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
            //defenderのカードを取得
            CardController[] playerFieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();

            //attackerのカードを選択
            CardController attacker = enemyCanAttackCardList[0];

            if (playerFieldCardList.Length > 0)
            {
                //defenderのカードを選択
                //シールドカードのみ攻撃対象にする
                if (Array.Exists(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD))
                {
                    playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD);
                }
                CardController defender = playerFieldCardList[0];
                //attackerとdefenderを戦わせる
                StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                gameManager.CardsBattle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.movement.MoveToTarget(gameManager.playerHero.transform));
                yield return new WaitForSeconds(0.25f);
                gameManager.AttackHero(attacker);
                yield return new WaitForSeconds(0.25f);
                gameManager.CheckHeroHp();
            }
            fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        gameManager.ChangeTurn();
    }
    IEnumerator CastSpell(CardController spellCard)
    {
        CardController target = null;
        Transform movePosition = null;
        switch (spellCard.model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                target = gameManager.GetEnemyFieldCard(spellCard.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.HEAL_FRIEND_CARD:
                target = gameManager.GetFriendFieldCard(spellCard.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                movePosition = gameManager.playerFieldTransform;
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                movePosition = gameManager.enemyFieldTransform;
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                movePosition = gameManager.playerHero;
                break;
            case SPELL.HEAL_FRIEND_HERO:
                movePosition = gameManager.enemyHero;
                break;
        }
        //ターゲット/それぞれのフィールド/それぞれのHeroのTransformが必要
        StartCoroutine(spellCard.movement.MoveToField(movePosition));
        yield return new WaitForSeconds(0.25f);
        spellCard.UseSpell(target);
    }
}
