using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//�U������鑤
public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /*�U��*/
        //attacker�̃J�[�h��I��
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        
        if (attacker == null)
        {
            return;
        }
        //�G�t�B�[���h�ɃV�[���h�J�[�h������΍U���ł��Ȃ�
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCard(attacker.model.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHIELD))
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            //attacker��Hero�ɍU������
            GameManager.instance.AttackHero(attacker);
            GameManager.instance.CheckHeroHp();
        }
        
    }
}
