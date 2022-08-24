using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;

    [SerializeField] TextMeshProUGUI playerHeroHpText, enemyHeroHpText;

    [SerializeField] TextMeshProUGUI playerManaCostText, enemyManaCostText;

    public TextMeshProUGUI timeCountText;

    public void HideResultPanel()
    {
        resultPanel.SetActive(false);
    }
    public void ShowManaCost(int playerManaCost, int enemyManaCost)
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }
    public void UpdateTime(int timeCount)
    {
        timeCountText.text = timeCount.ToString();

    }
    public void ShowHeroHp(int playerHeroHp, int enemyHeroHp)
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }
    public void ShowResultPanel(int heroHp)
    {
        resultPanel.SetActive(true);
        if (heroHp <= 0)
        {
            resultText.text = "LOSE";
        }
        else
        {
            resultText.text = "WIN";
        }
    }
    public void ShowDrawResult()
    {
        resultPanel.SetActive(true);
        resultText.text = "DRAW";
    }
}
