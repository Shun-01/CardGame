using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//�U������鑤
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /*�U��*/
        //attacker�̃J�[�h��I��
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        //defender�̃J�[�h��I��
        CardController defender = GetComponent<CardController>();

        if (attacker == null || defender == null)
        {
            return;
        }
        if(attacker.model.isPlayerCard == defender.model.isPlayerCard)
        {
            return;
        }

        //�G�t�B�[���h�̃V�[���h�J�[�h�ȊO�͍U���ł��Ȃ�
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCard(attacker.model.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHIELD) && defender.model.ability != ABILITY.SHIELD)
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            //attacker��defender���킹��
            GameManager.instance.CardsBattle(attacker, defender);
        }
        
    }
}
