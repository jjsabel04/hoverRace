using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHoverControll : MonoBehaviour
{
    [Header("Hoverboard Properties")]
    [SerializeField] private float __speed = 2;
    [SerializeField] private float __rotSpeed = 10f;
    [SerializeField] private float __levelForce = 150f;
    [SerializeField] private float hoverDist = 1;
    [SerializeField] private float Stability = 0.3f;
    [SerializeField] private float Stabilityspeed = 2.0f;
    [Header("Sound")]
    [SerializeField] private float EngineAcceleration = 10f;
    [SerializeField] private float minEngineSound = .05f;
    [SerializeField] private float maxEngineSound = .6f;
    [SerializeField] private AudioSource _shipSound;
    [Space(5)]
    [Header("Hoverboard Points")]
    [SerializeField] private Transform __corners;
    [SerializeField] private Transform __Orientaion;


    #region Private Vars
        private bool groundLevel;
        private float timeUngrounded;
        private bool inAir = true;
        private Rigidbody __rb;
        private float __upForce = 1;
        #endregion
    
    void Start()
    {
        _shipSound.Play();  
        __rb = GetComponent<Rigidbody>();
        __corners.GetChild(0).position = new Vector3(transform.position.x + transform.localScale.x/2,transform.position.y, transform.position.z + transform.localScale.z/2);
        __corners.GetChild(1).position = new Vector3(transform.position.x + transform.localScale.x/2,transform.position.y, transform.position.z - transform.localScale.z/2);
        __corners.GetChild(2).position = new Vector3(transform.position.x - transform.localScale.x/2,transform.position.y, transform.position.z + transform.localScale.z/2);
        __corners.GetChild(3).position = new Vector3(transform.position.x - transform.localScale.x/2,transform.position.y, transform.position.z - transform.localScale.z/2);
    }
   
     void LateUpdate () {
         // this stability feature makes landing tricks alot easier, at this point i have fixed the previous problem by other means that work in all situations and so stabilization may be used as a feature for an easier mode
         Vector3 predictedUp = Quaternion.AngleAxis(
             __rb.angularVelocity.magnitude * Mathf.Rad2Deg * Stability / Stabilityspeed,
             __rb.angularVelocity
         ) * transform.up;
         Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
         __rb.AddTorque(torqueVector * Stabilityspeed * Stabilityspeed);
         

         _shipSound.pitch = Mathf.Clamp(__rb.velocity.magnitude / 
                                        EngineAcceleration , minEngineSound, maxEngineSound);
     }
    

    void FixedUpdate()
    {
        groundLevel = false;
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(__corners.GetChild(i).position, transform.TransformDirection(Vector3.down), out hit, hoverDist))
            {
                Debug.DrawRay(__corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                __upForce = (hoverDist - hit.distance) * __levelForce;
                //rb.AddForce(Vector3.up * upForce);
                __rb.AddForceAtPosition(Vector3.up * __upForce, __corners.GetChild(i).position);
                groundLevel = true;
            }
            else
            {
                Debug.DrawRay(__corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hoverDist, Color.white);
                
            }
        }

        if (groundLevel == false)
        {
            __rb.drag = 0;
            __rb.angularDrag = 3;
            timeUngrounded += Time.deltaTime;
            if (timeUngrounded >= .5f)
            {
                inAir = true;
            }
        }
        else
        {
            __rb.drag = 7;
            __rb.angularDrag = 7;
            if (Input.GetKey(KeyCode.W))
            {
                __rb.AddForce(__Orientaion.forward.normalized * __speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                __rb.AddForce(__Orientaion.right.normalized * -__speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                __rb.AddForce(__Orientaion.forward.normalized * -__speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                __rb.AddForce(__Orientaion.right.normalized * __speed);
            }
            inAir = false;
            //__rb.AddTorque(0, Input.GetAxisRaw("Mouse X") * __rotSpeed, 0);
            timeUngrounded = 0;
        }

        //__rb.AddTorque(0, Input.GetAxisRaw("Mouse X") * __rotSpeed, 0);
        __rb.AddTorque(__Orientaion.up.normalized * Input.GetAxisRaw("Mouse X") * __rotSpeed);

        if (inAir)
        {
            
            if (Input.GetKey(KeyCode.W))
            {
                __rb.AddTorque(__Orientaion.right.normalized * __speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                __rb.AddTorque(__Orientaion.forward.normalized * __speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                __rb.AddTorque(__Orientaion.right.normalized * -__speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                __rb.AddTorque(__Orientaion.forward.normalized * -__speed);
            }
        }
    }
}
