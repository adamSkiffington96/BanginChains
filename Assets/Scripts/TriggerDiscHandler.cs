using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDiscHandler : MonoBehaviour
{
    public FlyingDiscRB golfer;

    private bool grounded = false;

    public float rotateDiscSpeed = 2000f;

    private GameObject CurrentDisc;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        CurrentDisc = golfer.CurrentDisc;
    }

    private void OnCollisionStay(Collision collision)
    {
        // We are now using the rigidbody's natural movement rather than scripted
        if (!grounded)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                grounded = true;

                print("Disc now grounded");

                rb.interpolation = RigidbodyInterpolation.None;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }
        
        print(collision.collider.tag);
    }

    private void Update()
    {
        // Simplest way to simulate the disc spinning during flight. We could vary the spin rate depending on the throw force/velocity and drag
        if (golfer.StateThrown())
        {
            if (!grounded)
                CurrentDisc.transform.Rotate(new Vector3(0, rotateDiscSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            grounded = false;
        }
    }
}
