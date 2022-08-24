using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//攻撃される側
public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /*攻撃*/
        //attackerのカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        
        if (attacker == null)
        {
            return;
        }
        //敵フィールドにシールドカードがあれば攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCard(attacker.model.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHIELD))
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            //attackerがHeroに攻撃する
            GameManager.instance.AttackHero(attacker);
            GameManager.instance.CheckHeroHp();
        }
        
    }
}
