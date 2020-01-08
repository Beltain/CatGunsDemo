using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerUpObject : MonoBehaviour
{
    public GameObject powerUp;

    private void OnTriggerEnter(Collider other)
    {
        int selectableLayermask = 1 << 8;
        if (other.gameObject.GetComponent<Unit>() != null)
        {
            GameController.gameController.AddPowerUpToTeam(other.gameObject.GetComponent<Unit>().teamIndex, powerUp);
            GameController.gameController.RemoveWorldPowerUp(gameObject);
            Destroy(gameObject);
        }
    }

}
