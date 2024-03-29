using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Subfusil,
    Fusil,
    Pistola,
    Escopeta,
    RPG,
    Sniper
}

public class BulletController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletDecal;

    private float speed = 50f;
    private float timeToDestroy = 3f;


    public Vector3 target { get; set; }
    public bool hit { get; set; }

    public WeaponType weaponType;


    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
        SetWeaponStats();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (!hit && Vector3.Distance(transform.position, target) < .01f)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contact = other.GetContact(0);
        GameObject.Instantiate(bulletDecal, contact.point + contact.normal * .0001f, Quaternion.LookRotation(contact.normal));
        Destroy(gameObject);
    }

    private void SetWeaponStats()
    {
        switch (weaponType)
        {
            case WeaponType.Subfusil:

                
                break;
            case WeaponType.Fusil:
 

                break;
            case WeaponType.Pistola:


                break;
            case WeaponType.Escopeta:
                

                break;
            case WeaponType.RPG:
                

                break;
            case WeaponType.Sniper:
                

                break;        }
    }
}
