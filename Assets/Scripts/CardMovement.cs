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
        //カードのコストとPlayerの残りManaコストを比較
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
        //一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        //DOTweenでカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        defaultParent = field;
        transform.SetParent(defaultParent);
    }
    public IEnumerator MoveToTarget(Transform target)
    {
        //現在の位置と並びを取得
        Vector3 initialPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        //一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);

        //DOTweenでカードをTargetに移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        //元の位置に戻る   
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
