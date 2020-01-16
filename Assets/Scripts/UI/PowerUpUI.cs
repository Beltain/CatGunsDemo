using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class PowerUpUI : MonoBehaviour , IPointerDownHandler
{
    public Color activeColor;
    public Color disabledColor;
    public Image powerUpButton;
    public Image cooldownIndicator;
    public TextMeshProUGUI counter;
    public GameController.PowerUp powerUpType;
    public bool usable = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(usable)GameController.gameController.ApplyPowerUpToTeam(powerUpType, CombatController.combatController.combat.playerTeamIndex);
    }

    public void SetCooldown(float amount)
    {
        cooldownIndicator.fillAmount = amount;
    }

    public void SetCounter(int amount)
    {
        if (amount == 0) counter.text = "";
        else counter.text = "x" + amount.ToString();
    }


    public void SetPressable(bool state)
    {
        if (state)
        {
            powerUpButton.color = activeColor;
            usable = true;
        }
        else
        {
            powerUpButton.color = disabledColor;
            usable = false;
        }
    }

}
