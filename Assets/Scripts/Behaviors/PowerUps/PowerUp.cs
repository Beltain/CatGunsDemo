using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerUp : MonoBehaviour , IPointerDownHandler
{

    //Method to call from other classes
    public void PowerUpUnit(Unit unit)
    {
        StartCoroutine(PowerUpUnitSequence(unit));
    }

    //The main sequencer that ensure the powerup isnt destroyed before it's applied
    IEnumerator PowerUpUnitSequence(Unit unit)
    {
        //Apply the powerup
        yield return ApplyPowerUp(unit);

        //Remove the powerup from the game
        GameController.gameController.RemovePowerUp(gameObject, unit.teamIndex);
        Destroy(gameObject);

        yield return null;
    }

    //The custom power up method
    virtual public IEnumerator ApplyPowerUp(Unit unit)
    {
        yield return null;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        UIController.uiController.StartMovePowerUp(transform);
    }
}
