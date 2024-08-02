using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examples : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        /*
        if (throwingDisc)
        {
            _force = Vector3.Lerp(_force, Vector3.zero, discAddPowerFalloff * Time.deltaTime);


            Vector3 gravity = Physics.gravity * Time.deltaTime;

            float flyingSpeed = new Vector3(_velocity.x, 0, _velocity.z).magnitude;

            Vector3 liftForce = Vector3.ClampMagnitude((HandleDisc.up.normalized * flyingSpeed) * liftModifier, gravity.magnitude);




            _velocity += ((gravity + liftForce) + _force);   // allow gravity to work on our velocity vector
            HandleDisc.position += ((_velocity) * Time.deltaTime);    // move us this frame according to our speed




            float fTest = _force.magnitude;
            float lTest = liftForce.magnitude;

            if (fTest < 0.001f)
                fTest = 0f;
            if (lTest < 0.001f)
                lTest = 0f;

            //velocityText.text = "Lift Force: " + lTest + "\nAdded force: " + fTest;
            velocityText.text = "Flying Speed: " + flyingSpeed;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            InspectDisc();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!throwingDisc)
                ThrowDisc();
        }

        CharacterControl();

        */
    }
}
