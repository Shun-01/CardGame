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
        //�t�B�[���h�̃J�[�h���U���\�ɂ���
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1);
        
        /*��ɃJ�[�h���o��*/
        //��D�̃J�[�h���X�g���擾
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        //�R�X�g�ȉ��̃J�[�h������΁A�J�[�h���t�B�[���h�ɏo��������
        //�����F�����X�^�[�J�[�h�Ȃ�R�X�g�̂�
        //�����F�X�y���J�[�h�Ȃ�R�X�g�Ǝg�p�\���ǂ��� (CanUseSpell)
        while (Array.Exists(handCardList, card => 
        (card.model.cost <= gameManager.enemy.manaCost)
        && (!card.IsSpell || (card.IsSpell && card.CanUseSpell()))
        ))
        {
            //�R�X�g�ȉ��̃J�[�h���X�g���擾
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card => 
            (card.model.cost <= gameManager.enemy.manaCost)
            && (!card.IsSpell || (card.IsSpell && card.CanUseSpell()))
            );
            //Field�ɏo���J�[�h��I��
            CardController selectCard = selectableHandCardList[0];
            //�J�[�h��\�ɂ���  
            selectCard.Show();
            //�X�y���J�[�h�Ȃ�g�p����
            if (selectCard.IsSpell)
            {
                StartCoroutine(CastSpell(selectCard));
            }
            else
            {
                //�J�[�h���ړ�
                StartCoroutine(selectCard.movement.MoveToField(gameManager.enemyFieldTransform));
                selectCard.model.isFieldCard = true;
                selectCard.OnField();
            }
            yield return new WaitForSeconds(1);
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        yield return new WaitForSeconds(1);

        /*�U��*/
        //�t�B�[���h�̃J�[�h���X�g���擾
        CardController[] fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        //�U���\�J�[�h������΍U�����J��Ԃ�
        while (Array.Exists(fieldCardList, card => card.model.canAttack))
        {
            //�U���\�J�[�h���擾
            CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
            //defender�̃J�[�h���擾
            CardController[] playerFieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();

            //attacker�̃J�[�h��I��
            CardController attacker = enemyCanAttackCardList[0];

            if (playerFieldCardList.Length > 0)
            {
                //defender�̃J�[�h��I��
                //�V�[���h�J�[�h�̂ݍU���Ώۂɂ���
                if (Array.Exists(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD))
                {
                    playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD);
                }
                CardController defender = playerFieldCardList[0];
                //attacker��defender���킹��
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
        //�^�[�Q�b�g/���ꂼ��̃t�B�[���h/���ꂼ���Hero��Transform���K�v
        StartCoroutine(spellCard.movement.MoveToField(movePosition));
        yield return new WaitForSeconds(0.25f);
        spellCard.UseSpell(target);
    }
}
