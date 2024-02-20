using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public float trapDamage;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerReferee>())  {
            other.GetComponent<PlayerReferee>().TakeDamage(trapDamage);
        }
    }
}
