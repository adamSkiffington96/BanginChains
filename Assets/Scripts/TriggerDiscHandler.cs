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
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = true;

            print("Disc now grounded");

            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        print(collision.collider.tag);
    }

    private void Update()
    {
        if (golfer.StateThrown())
        {
            if (!grounded)
                CurrentDisc.transform.Rotate(new Vector3(0, rotateDiscSpeed * Time.deltaTime, 0));
        }
        
    }
}
