using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaultParent;

    public bool isDraggable;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //�J�[�h�̃R�X�g��Player�̎c��Mana�R�X�g���r
        CardController card = GetComponent<CardController>();
        if (card.model.isPlayerCard && GameManager.instance.isPlayerTurn && !card.model.isFieldCard && card.model.cost <= GameManager.instance.player.manaCost)
        {
            isDraggable = true;
        }
        else if (card.model.isPlayerCard && GameManager.instance.isPlayerTurn && card.model.isFieldCard && card.model.canAttack)
        {
            isDraggable = true;
        }
        else
        {
            isDraggable = false;
        }

        if (!isDraggable)
        {
            return;
        }
        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }
        transform.SetParent(defaultParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public IEnumerator MoveToField(Transform field)
    {
        //��x�e��Canvas�ɕύX����
        transform.SetParent(defaultParent.parent);
        //DOTween�ŃJ�[�h���t�B�[���h�Ɉړ�
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        defaultParent = field;
        transform.SetParent(defaultParent);
    }
    public IEnumerator MoveToTarget(Transform target)
    {
        //���݂̈ʒu�ƕ��т��擾
        Vector3 initialPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        //��x�e��Canvas�ɕύX����
        transform.SetParent(defaultParent.parent);

        //DOTween�ŃJ�[�h��Target�Ɉړ�
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        //���̈ʒu�ɖ߂�   
        transform.DOMove(initialPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
        if (this != null)
        {
            transform.SetParent(defaultParent);
            transform.SetSiblingIndex(siblingIndex);
        }
    }
    private void Start()
    {
        defaultParent = transform.parent;
    }
}
