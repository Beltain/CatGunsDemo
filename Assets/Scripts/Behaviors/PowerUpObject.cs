using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerUpObject : MonoBehaviour
{
    public GameObject powerUp;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;   
    }

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

    private void Update()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * 35f, 0f));
        transform.position = startPos + (new Vector3(0f, Mathf.Abs(Mathf.Sin(Time.time * 5f)) * 1.5f, 0f));
    }

}
