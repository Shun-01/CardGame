using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    public List<int> deck = new List<int>();

    public int heroHp;

    public int manaCost, defaultManaCost;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        heroHp = 10;
        manaCost = 5;
        defaultManaCost = 2;
    }
    public void IncreaseManaCost()
    {
        defaultManaCost++;
        manaCost = manaCost+defaultManaCost;
    }
}
