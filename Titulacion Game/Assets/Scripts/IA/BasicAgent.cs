using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using static PlayerReferee;

[RequireComponent(typeof(Rigidbody))]
public class BasicAgent : MonoBehaviour
{
    [SerializeField] public float m_speed, m_maxVel, m_maxSteerForce, radius;
    public Transform target;
    public float wanderDisplacement, wanderRadius;

    Rigidbody rb;

    public Vector3? nextWanderPosition;

    public float fltLife = 100f;


    Collider[] perceibed;


    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        //perceibed = Physics.OverlapSphere(explosionArea.position, explosionRadius);

    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Gives access to max speed of agent
    /// </summary>
    /// <returns>float m_speed</returns>
    public float getSpeed()
    {
        return m_speed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float getMaxVel()
    {
        return m_maxVel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float getMaxSteerForce()
    {
        return m_maxSteerForce;
    }

    /// <summary>
    /// Damage bullets & explosion (AOE)
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        

    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(explosionArea.position, explosionRadius);
    }

}