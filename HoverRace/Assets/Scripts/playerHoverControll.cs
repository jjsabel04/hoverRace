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
    [SerializeField] private PhysicMaterial hoverOnPhysMat;
    [SerializeField] private PhysicMaterial hoverOffPhysMat;
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

    [SerializeField] private float currentPoints;

    [SerializeField] private bool onRail;
    [SerializeField] private bool prevOnRail;
    [SerializeField] private GameObject rail;
    [SerializeField] private GameObject nearestRail;
    //[SerializeField] private float timeOnRail;

    [SerializeField] private float decel;
    [SerializeField] private bool startDecel;
    [SerializeField] private Vector3 prevDir;


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
        private bool upRight;
        private int rotDecrease;
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
        HandleStabalisation();
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
     
    void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //Debug.Log("a");
            if (!onRail)
            {
                //Debug.Log("b");
                prevOnRail = false;
            }
        }
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
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        if(nearestRail!= null)
        {
            Gizmos.DrawSphere(nearestRail.transform.GetComponent<Collider>().ClosestPointOnBounds(transform.position), .1f);
        }
    }

    private Vector3 OrientTorque(Vector3 torque)
    {
        // Quaternion's Euler conversion results in (0-360)
        // For torque, we need -180 to 180.
 
        return new Vector3
        (
            torque.x > 180f ? 180f - torque.x : torque.x,
            torque.y > 180f ? 180f - torque.y : torque.y,
            torque.z > 180f ? 180f - torque.z : torque.z
        );
    }

    void HandleInputAndHovering()
    {
        nearestRail = null;
        _groundLevel = false;
        _inAir = false;
        rotDecrease = 1;
        onRail = false;

        if(Input.GetKey(KeyCode.Space))
        {
            transform.GetComponent<Collider>().material = hoverOffPhysMat;
            Collider[] nearHover = Physics.OverlapBox(transform.position, transform.localScale*2f, transform.rotation);
            if (nearHover.Length > 1)
            {
                for (int i = 0; i < nearHover.Length; i++)
                {
                    //Debug.Log(nearHover[i].name);
                    if (nearHover[i].tag == "rail")
                    {
                        if (nearestRail == null || Vector3.Distance(transform.position,nearHover[i].transform.position) < Vector3.Distance(transform.position,nearestRail.transform.position))
                        {
                            nearestRail = nearHover[i].gameObject;
                        }
                    }
                }
                
                if(nearestRail != null)
                {
                    Vector3 closestPoint = nearestRail.transform.GetComponent<Collider>().ClosestPoint(transform.position);
                    if (closestPoint.y - 0.01f < transform.position.y)
                    {
                        if ((transform.eulerAngles.x > 290 || transform.eulerAngles.x < 70) && (transform.eulerAngles.z > 290 || transform.eulerAngles.z < 70))
                        {
                            Vector3 directionVector = (closestPoint - transform.position).normalized;
                            _rigidbody.AddForce (directionVector * 1500 * (Vector3.Distance(transform.position,closestPoint) + .3f));
                            if (prevOnRail == false)
                            {
                                transform.position = closestPoint;
                                //timeOnRail = currentPoints;
                                prevOnRail = true;
                            }
                        }
                    }
                }
            }
            nearHover = Physics.OverlapBox(transform.position, transform.localScale*1.5f, transform.rotation);
            if (nearHover.Length > 1)
            {
                for (int i = 0; i < nearHover.Length; i++)
                {
                    if (nearHover[i].tag == "rail")
                    {
                        onRail = true;
                        rail = nearHover[i].gameObject;
                        _rigidbody.drag = 10;
                        _rigidbody.angularDrag = 20;
                        rotDecrease = 40;
                        //timeOnRail += Time.deltaTime * 5;
                        //currentPoints = Mathf.FloorToInt(timeOnRail);
                        currentPoints += Time.deltaTime * 5;
                        
                        if(Mathf.Abs(_rigidbody.velocity.x) > 1f || Mathf.Abs(_rigidbody.velocity.z) > 1f)
                        {
                            _rigidbody.velocity = _rigidbody.velocity.normalized * 17f * (currentPoints / 65 + 1);
                        }
                        /*
                        //Transform targetRot = null;
                        orientation.rotation = Quaternion.LookRotation(_rigidbody.velocity, Vector3.up);
                        // Determine Quaternion 'difference'
                        // The conversion to euler demands we check each axis
                        Vector3 torqueF = OrientTorque(Quaternion.FromToRotation(this.transform.forward, orientation.transform.forward).eulerAngles);
                        Vector3 torqueR = OrientTorque(Quaternion.FromToRotation(this.transform.right, orientation.transform.right).eulerAngles);
                        Vector3 torqueU = OrientTorque(Quaternion.FromToRotation(this.transform.up, orientation.transform.up).eulerAngles);
                
                        float magF = torqueF.magnitude;
                        float magR = torqueR.magnitude;
                        float magU = torqueU.magnitude;
                
                        // Here we pick the axis with the least amount of rotation to use as our torque.
                        Vector3 torque = magF < magR ? (magF < magU ? torqueF : torqueU) : (magR < magU ? torqueR : torqueU);
                        
                        _rigidbody.AddTorque(torque * Time.fixedDeltaTime * .0001f);
                        */
                    }
                }
            }
        }
        //if(!onRail)
        //{
        //    timeOnRail = 0;
        //}
        if(!Input.GetKey(KeyCode.Space))
        {
            transform.GetComponent<Collider>().material = hoverOnPhysMat;
            if (jumpForce > 1)
            {
                jumpForce -= 0.1f;
                if(prevOnRail == true)
                {
                    //Debug.Log(jumpForce);
                    _rigidbody.AddForce(transform.up * 200 * jumpForce);
                    jumpForce -= 0.03f;
                }
            }
            else
            {
                prevOnRail = false;
                jumpForce = 1;
            }
        }
        else if (jumpForce/7 < .6f)
        {
            jumpForce += Time.deltaTime * 7;
        }

        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(corners.GetChild(i).position, transform.TransformDirection(Vector3.down), out hit, hoverDist + 3, 7))
            {
                if(hit.distance < hoverDist && !Input.GetKey(KeyCode.Space))
                {
                    Debug.DrawRay(corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                    _upForce = (hoverDist - hit.distance) * levelForce;
                    _rigidbody.AddForceAtPosition(transform.up * _upForce * jumpForce, corners.GetChild(i).position);
                    _groundLevel = true;
                    upRight = true;
                }
            }
            else
            {
                Debug.DrawRay(corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hoverDist, Color.white);
                _inAir = true;
            }
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
            
            ////////////////// deceleration ////////////////////
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                if (startDecel == false)
                {
                    decel = speed;
                    startDecel = true;
                }
                decel -= Time.deltaTime * 17500;
                if (decel < 0)
                {
                    decel = 0;
                }
                //Debug.Log(decel);
                _rigidbody.AddForce(prevDir * decel);
            }
            else
            {
                prevDir = (MovementFB + MovementLR).normalized;
                startDecel = false;
            }
            /////////////////////////////////////////////////

            if (currentPoints > 0)
            {
                //_rigidbody.AddForce();
                currentPoints -= Time.deltaTime * 15;
            }
            else
            {
                currentPoints = 0;
            }


            if (currentPoints >= 40)
            {
                _rigidbody.AddForce((MovementFB + MovementLR).normalized * speed * ((40f / 65f) + 1));
            }
            else
            {
                _rigidbody.AddForce((MovementFB + MovementLR).normalized * speed * (currentPoints / 65 + 1));
            }
            
            
            
        }
        else if(!onRail)
        {
            //prevOnRail = false;
            _rigidbody.drag = 0;
            _rigidbody.angularDrag = 3;
        }
        
        
        _rigidbody.AddTorque(orientation.up.normalized * Input.GetAxisRaw("Mouse X") * gameManager.MouseSensitivity);

        if (_inAir || onRail) // trick controlls
        {
            if(!_stabilizing)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    _rigidbody.AddTorque(orientation.right.normalized * speed / rotDecrease);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    _rigidbody.AddTorque(orientation.forward.normalized * speed / rotDecrease);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    _rigidbody.AddTorque(orientation.right.normalized * -speed / rotDecrease);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    _rigidbody.AddTorque(orientation.forward.normalized * -speed / rotDecrease);
                }
            }

            if(upRight && (transform.localEulerAngles.z <= 210 && transform.localEulerAngles.z >= 150))
            {
                currentPoints += 8; // 5 flips for 40 points (max speed) and 11 for max time
                upRight = false;
            }
            if(!upRight && (transform.localEulerAngles.z <= 30 || transform.localEulerAngles.z >= 330))
            {
                currentPoints += 8;
                upRight = true;
            }
            
        }


        if (Input.GetMouseButton(1))
        {
            // _rigidbody.drag = 15;
            // _rigidbody.angularDrag = 15;
            currentPoints = 0;
            // StartCoroutine(Boost());
            //_rigidbody.AddForce(orientation.forward.normalized * speed * boostSpeed);
        }




        if (currentPoints > 85)
        {
            currentPoints = 85;
        }
        Debug.Log(currentPoints);
    }

    void OnCollisionEnter(Collision other)
    {
        if(!_groundLevel && other.gameObject.tag != "rail")
        {
            //Debug.Log(other.gameObject.tag);
            currentPoints = 0;
        }
        //Debug.Log(currentPoints);
    }
}
