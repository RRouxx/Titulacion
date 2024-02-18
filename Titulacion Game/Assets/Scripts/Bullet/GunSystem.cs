using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class GunSystem : MonoBehaviour
{
    //bullet 
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    private PlayerInput playerInput;
    private InputAction shootAction;
    private InputAction reloadAction;

    //Recoil
    public Rigidbody playerRb;
    public float recoilForce;

    //bools 
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug fixing
    public bool allowInvoke = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        shootAction = playerInput.actions["Shoot"];
        reloadAction = playerInput.actions["Reload"];

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void OnEnable()
    {
        shootAction.performed += _ => Shoot();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => Shoot();

    }

    private void Update()
    {
        MyInput();
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    /// <summary>
    /// 
    /// </summary>
    private void MyInput()
    {

        if (reloadAction.triggered && bulletsLeft < magazineSize && !reloading) Reload();
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    /// <summary>
    /// Find the exact hit position using a raycast
    /// check if ray hits something
    /// Calculate direction from attackPoint to targetPoint
    /// Calculate spread
    /// Calculate new direction with spread
    /// Instantiate bullet/projectile
    /// Rotate bullet to shoot direction
    /// Add forces to bullet
    /// Instantiate muzzle flash, if you have one
    /// Invoke resetShot function (if not already invoked), with your timeBetweenShooting
    /// Add recoil to player (should only be called once)
    /// if more than one bulletsPerTap make sure to repeat shoot function
    /// </summary>
    private void Shoot()
    {
        readyToShoot = false;

        //
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //
        currentBullet.transform.forward = directionWithSpread.normalized;

        //
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        //
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //
            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    /// <summary>
    /// Allow shooting and invoking again
    /// </summary>
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    /// <summary>
    /// Invoke ReloadFinished function with your reloadTime as delay
    /// </summary>
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    /// <summary>
    /// Fill magazine
    /// </summary>
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
