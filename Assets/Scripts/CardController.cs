using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView view; //�\��(view)�Ɋւ��邱�Ƃ𑀍�
    public CardModel model; //�f�[�^(model)�Ɋւ��邱�Ƃ𑀍�
    public CardMovement movement; //�ړ�(movement)�Ɋւ��邱�Ƃ𑀍�

    GameManager gameManager;
    public bool IsSpell 
    { 
        get { return model.spell != SPELL.NONE; } 
    }
    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
        gameManager = GameManager.instance;
    }
    public void Init(int cardID, bool isPlayer)
    {
        model = new CardModel(cardID, isPlayer);
        view.SetCard(model);
    }
    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetCanAttack(false);
        view.ShowSelectablePanel(false);
    }
    public void Heal(CardController friendCard)
    {
        model.Heal(friendCard);
        friendCard.RefreshView();
    }
    public void Show()
    {
        view.Show();
    }
    public void RefreshView()
    {
        view.Refresh(model);
    }
    public void SetCanAttack(bool canAttack)
    {
        model.canAttack = canAttack;
        view.ShowSelectablePanel(canAttack);
    }
    public void OnField()
    {
        GameManager.instance.ReduceManaCost(model.cost, model.isPlayerCard);
        model.isFieldCard = true;
        if (model.ability == ABILITY.INIT_ATTACKABLE)
        {
            SetCanAttack(true);
        }
    }
    public void CheckAlive()
    {
        if (model.isAlive)
        {
            RefreshView();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void UseSpell(CardController target)
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                //�G1�̂��U������
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard == model.isPlayerCard)
                {
                    return;
                }
                Attack(target);
                target.CheckAlive();
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                //����t�B�[���h�̑S�ẴJ�[�h�ɍU������
                CardController[] enemyCards = gameManager.GetEnemyFieldCard(this.model.isPlayerCard);
                foreach (CardController enemyCard in enemyCards)
                {
                    Attack(enemyCard);
                }
                foreach (CardController enemyCard in enemyCards)
                {
                    enemyCard.CheckAlive();
                }
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                gameManager.AttackHero(this);
                break;
            case SPELL.HEAL_FRIEND_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard != model.isPlayerCard)
                {
                    return;
                }
                Heal(target);
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                CardController[] friendCards = gameManager.GetFriendFieldCard(this.model.isPlayerCard);
                foreach (CardController friendCard in friendCards)
                {
                    Heal(friendCard);
                }
                break;
            case SPELL.HEAL_FRIEND_HERO:
                gameManager.HealHero(this);
                break;
            case SPELL.NONE:
                return;
        }
        GameManager.instance.ReduceManaCost(model.cost, model.isPlayerCard);
        Destroy(this.gameObject);
    }
    public bool CanUseSpell() //�X�y���J�[�h���g�p�\���ǂ������f����
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
            case SPELL.DAMAGE_ENEMY_CARDS:
                CardController[] enemyCards = gameManager.GetEnemyFieldCard(this.model.isPlayerCard);
                if (enemyCards.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.DAMAGE_ENEMY_HERO:
            case SPELL.HEAL_FRIEND_HERO:
                return true;
            case SPELL.HEAL_FRIEND_CARD:
            case SPELL.HEAL_FRIEND_CARDS:
                CardController[] friendCards = gameManager.GetFriendFieldCard(this.model.isPlayerCard);
                if (friendCards.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.NONE:
                return false;
        }
        return false;
    }
}
