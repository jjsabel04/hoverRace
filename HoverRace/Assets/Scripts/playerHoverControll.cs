using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHoverControll : MonoBehaviour
{

    private Rigidbody rb;
    private float upForce = 1;
    public float speed = 2;
    public Transform corners;

    private float hoverDist = 1;

    private bool groundLevel;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        corners.GetChild(0).position = new Vector3(transform.position.x + transform.localScale.x/2,transform.position.y, transform.position.z + transform.localScale.z/2);
        corners.GetChild(1).position = new Vector3(transform.position.x + transform.localScale.x/2,transform.position.y, transform.position.z - transform.localScale.z/2);
        corners.GetChild(2).position = new Vector3(transform.position.x - transform.localScale.x/2,transform.position.y, transform.position.z + transform.localScale.z/2);
        corners.GetChild(3).position = new Vector3(transform.position.x - transform.localScale.x/2,transform.position.y, transform.position.z - transform.localScale.z/2);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(corners.GetChild(i).position, transform.TransformDirection(Vector3.down), out hit, hoverDist))
            {
                Debug.DrawRay(corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                upForce = (hoverDist - hit.distance) * 100;
                //rb.AddForce(Vector3.up * upForce);
                rb.AddForceAtPosition(Vector3.up * upForce, corners.GetChild(i).position);
                groundLevel = true;
            }
            else
            {
                Debug.DrawRay(corners.GetChild(i).position, transform.TransformDirection(Vector3.down) * hoverDist, Color.white);
                groundLevel = false;
            }
        }

        if (groundLevel == true)
        {
            rb.drag = 7;
            rb.angularDrag = 7;
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.forward * speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.right * -speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(transform.forward * -speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(transform.right * speed);
            }
        }
        else
        {
            rb.drag = .01f;
            rb.angularDrag = .01f;
        }
    }
}
