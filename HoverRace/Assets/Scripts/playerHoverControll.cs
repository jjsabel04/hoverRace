using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class playerHoverControll : MonoBehaviour
{
    [Header("Hoverboard Properties")]
    [SerializeField] private float speed = 2;
    [SerializeField] private float levelForce = 150f;
    [SerializeField] private float hoverDist = 1;
    [SerializeField] private float stability = 0.3f;
    [SerializeField] private float stabilitySpeed = 2.0f;
    [SerializeField] private float boostSpeed = 2.0f;
    [SerializeField] private float boostDelay = 2.0f;
    [Header("Sound")]
    [SerializeField] private float engineAcceleration = 10f;
    [SerializeField] private float minEngineSound = .05f;
    [SerializeField] private float maxEngineSound = .6f;
    [SerializeField] private AudioSource shipSound;
    [FormerlySerializedAs("__corners")]
    [Space(5)]
    [Header("Hoverboard Points")]
    [SerializeField] private Transform corners;
    [SerializeField] private Transform orientation;


    #region Private Vars
        private bool _groundLevel;
        private bool _inAir = true;
        private Rigidbody _rigidbody;
        private float _upForce = 1;
        private bool _stabilizing;
        private GameManager gameManager;
        private Vector3 MovementFB;
        private Vector3 MovementLR;
        private float jumpForce;
        #endregion

    private void Awake()
    {   
        shipSound.Play();
        _rigidbody = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        GetPoints();
    }
   
     private void LateUpdate ()
     {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            _stabilizing = true;
            HandleStabalisation();
        }
        else
        {
            _stabilizing = false;
        }
        HandleAudio();
     }
     

    void FixedUpdate()
    {
        HandleInputAndHovering();
    }

    private void HandleStabalisation()
    {
        // this stability feature makes landing tricks alot easier, at this point i have fixed the previous problem
        // by other means that work in all situations and so stabilization may be used as a feature for an easier mode
        Vector3 predictedUp = Quaternion.AngleAxis(
            _rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilitySpeed,
            _rigidbody.angularVelocity
            ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        _rigidbody.AddTorque(torqueVector * stabilitySpeed);
    }

    private void HandleAudio()
    {
        shipSound.pitch = Mathf.Clamp(_rigidbody.velocity.magnitude / 
                                      engineAcceleration , minEngineSound, maxEngineSound);
    }

    private void GetPoints()
    {
        corners.GetChild(0).position = new Vector3(transform.position.x + transform.localScale.x/2,transform.position.y, transform.position.z + transform.localScale.z/2);
        corners.GetChild(1).position = new Vector3(transform.position.x + transform.localScale.x/2,transform.position.y, transform.position.z - transform.localScale.z/2);
        corners.GetChild(2).position = new Vector3(transform.position.x - transform.localScale.x/2,transform.position.y, transform.position.z + transform.localScale.z/2);
        corners.GetChild(3).position = new Vector3(transform.position.x - transform.localScale.x/2,transform.position.y, transform.position.z - transform.localScale.z/2);
    }

    void HandleInputAndHovering()
    {

        _groundLevel = false;
        _inAir = false;
        if(!Input.GetKey(KeyCode.Space))
        {
            if (jumpForce > 1)
            {
                jumpForce -= 0.1f;
            }
            else
            {
                jumpForce = 1;
            }
            for (int i = 0; i < 4; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(corners.GetChild(i).position, transform.TransformDirection(Vector3.down), out hit, hoverDist + 3))
                {
                    if(hit.distance < hoverDist)
                    {
                        Debug.DrawRay(corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                        _upForce = (hoverDist - hit.distance) * levelForce;
                        _rigidbody.AddForceAtPosition(transform.up * _upForce * jumpForce, corners.GetChild(i).position);
                        _groundLevel = true;
                    }
                }
                else
                {
                    Debug.DrawRay(corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hoverDist, Color.white);
                    _inAir = true;
                }
            }
        }
        else if (jumpForce/7 < .6f)
        {
            jumpForce += Time.deltaTime * 7;
        }

        if (_groundLevel)
        {
            _rigidbody.drag = 7;
            _rigidbody.angularDrag = 7;
            MovementFB = Vector3.zero;
            MovementLR = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                //_rigidbody.AddForce(orientation.forward.normalized * speed);
                MovementFB += transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                //_rigidbody.AddForce(orientation.right.normalized * -speed);
                MovementLR += -transform.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                //_rigidbody.AddForce(orientation.forward.normalized * -speed);
                MovementFB += -transform.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                //_rigidbody.AddForce(orientation.right.normalized * speed);
                MovementLR += transform.right;
            }
            
            _rigidbody.AddForce((MovementFB + MovementLR).normalized * speed);
            
            
            
        }
        else
        {
            _rigidbody.drag = 0;
            _rigidbody.angularDrag = 3;
        }
        
        
        _rigidbody.AddTorque(orientation.up.normalized * Input.GetAxisRaw("Mouse X") * gameManager.MouseSensitivity);

        if (_inAir && !_stabilizing)
        {
            
            if (Input.GetKey(KeyCode.W))
            {
                _rigidbody.AddTorque(orientation.right.normalized * speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                _rigidbody.AddTorque(orientation.forward.normalized * speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _rigidbody.AddTorque(orientation.right.normalized * -speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                _rigidbody.AddTorque(orientation.forward.normalized * -speed);
            }
        }


        if (Input.GetMouseButton(1))
        {
            // _rigidbody.drag = 15;
            // _rigidbody.angularDrag = 15;
            // StartCoroutine(Boost());
            _rigidbody.AddForce(orientation.forward.normalized * speed * boostSpeed);
        }

        IEnumerator Boost()
        {
            yield return new WaitForSeconds(boostDelay);
            _rigidbody.drag = 7;
            _rigidbody.angularDrag = 7;
        }
    }
}
