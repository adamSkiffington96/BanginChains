
# Project Title

A brief description of what this project does and who it's for


## Screenshots

![App Screenshot](https://via.placeholder.com/468x300?text=App+Screenshot+Here)


## Features

- Light/dark mode toggle
- Live previews
- Fullscreen mode
- Cross platform


## Snippets

</details>

<details>
<summary><code>FlyingDiscRB.cs</code></summary>

```
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyingDiscRB : MonoBehaviour
{
    public float speed = 5f;
    //public float drag = 1f;
    public float liftModifier = 1f;

    public Transform DiscHandle;

    public float rotateCharacterSpeed = 4f;

    private bool _thrown = false;

    public float curveAmount = 0f;

    public Text _debugText;

    public float _OrientSpeed = 1f;

    public float tiltSpeed = 1f;

    public float maxCurve = 6f;

    public TextMeshProUGUI AngleText;

    public GameObject tiltIndicator;

    public GameObject CurrentDisc;

    public float rotateDiscSpeed = 0f;

    public float getDiscSpeed = 8f;

    private bool positionReset = true;

    public Rigidbody rb;

    public float distToGround = 1f;

    public float curvePower = 0.2f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            //  Moving the disc back to the player to make a new throw
            //  It also creates an effect, pulling the disc back into your hand rather than it instantly re-appearing
            if (!positionReset)
            {
                if ((DiscHandle.position - transform.position).magnitude > 0.5f)
                {
                    DiscHandle.position = Vector3.Lerp(DiscHandle.position, transform.position, Time.deltaTime * getDiscSpeed);
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    Rethrow();
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            ThrowDisc();
        }

        // If disc is thrown, apply lift and angular momentum on the disc, or 'curve'

        // If disc is not thrown, we can change the angle (hyzer/anhyzer)

        if (_thrown) {
            CalcLift();
        }
        else {
            TiltAngle();
        }

        // Display of disc angle

        UIAngle();

        // Allow control of character when not changing disc angle

        if (!Input.GetMouseButton(1))
            CharacterControl();
    }

    public bool StateThrown()
    {
        return _thrown;
    }

    private void CalcLift()
    {
        // Define a drag force
        //Vector3 dragForce = (-drag * rb.velocity) * Time.deltaTime;
        // Drag now calculated in rigidbody

        // Define normal force as the disc normal * our velocity
        Vector3 nForce = liftModifier * rb.velocity.sqrMagnitude * Vector3.Project(-Physics.gravity, DiscHandle.up.normalized).normalized;

        // Clamp lift mag to gravity's magnitude 
        if (nForce.magnitude > Physics.gravity.magnitude)
            nForce = Vector3.Project(-Physics.gravity, DiscHandle.up.normalized);

        Vector3 liftForce = Time.deltaTime * nForce;

        // Curve mechanics
        Vector3 sideDir = Vector3.Cross(DiscHandle.up, rb.velocity).normalized;
        Vector3 curve = (sideDir * curveAmount * curvePower) * Time.deltaTime;

        // Apply the lift force and curve mechanics
        rb.velocity += (liftForce + curve);

        //DiscHandle.position += (_velocity) * Time.deltaTime;
        // Position now calculated in rigidbody

        _debugText.text = "Lift: " + liftForce.magnitude + "Gravity: " + Physics.gravity.magnitude + "\nVelocity: " + rb.velocity.magnitude;
    }

    private void OnCollisionStay(Collision collision)
    {
        print(collision.collider.tag);
    }

    private void TiltAngle()
    {
        if (Input.GetMouseButton(1))
        {
            // Disc tilting (Anhyzer/Hyzer) (RMB) done before throwing
            //  - more curve is added the more you tilt the disc
            //  - angle is clamped
            //  - MUST IMPLEMENT HAMMER THROW

            float mouseX = Input.GetAxis("Mouse X");
            if (mouseX > 0)
            {
                if (curveAmount < maxCurve)
                {
                    curveAmount += mouseX * tiltSpeed * Time.deltaTime;
                    // Disc rotation may be handled by manipulating the transform or the rigidbody

                    //DiscHandle.eulerAngles = new Vector3(DiscHandle.eulerAngles.x, DiscHandle.eulerAngles.y, -curveAmount);
                    rb.rotation = Quaternion.Euler(DiscHandle.eulerAngles.x, DiscHandle.eulerAngles.y, -curveAmount);
                }
            }
            else if (mouseX < 0)
            {
                if (curveAmount > -maxCurve)
                {
                    curveAmount += mouseX * tiltSpeed * Time.deltaTime;
                    //DiscHandle.eulerAngles = new Vector3(DiscHandle.eulerAngles.x, DiscHandle.eulerAngles.y, -curveAmount);
                    rb.rotation = Quaternion.Euler(DiscHandle.eulerAngles.x, DiscHandle.eulerAngles.y, -curveAmount);
                }
            }

            curveAmount = Mathf.Clamp(curveAmount, -maxCurve, maxCurve);
        }
    }

    private void UIAngle()
    {
        //  Showing the angle of the disc on screen - along with a nifty slider!

        float angle = curveAmount;
        float maxAngle = maxCurve;

        float pos = (angle / maxAngle) * 130;

        if (angle < -1)
        {
            AngleText.text = angle.ToString("f0") + "º " + "Hyzer";
        }
        else if (angle > 1)
        {
            AngleText.text = angle.ToString("f0") + "º " + "Anhyzer";
        }
        else
        {
            AngleText.text = angle.ToString("f0") + "º " + "Flat";
        }

        tiltIndicator.transform.localPosition = new Vector3(pos, 0, 0);
    }

    private void ThrowDisc()
    {
        //  Resetting variables for a throw and adding an initial force

        DiscHandle.parent = null;

        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        _thrown = true;

        positionReset = false;

        rb.AddForce(DiscHandle.forward.normalized * speed * 100f);
    }

    private void Rethrow()
    {
        // Resetting the disc for a new throw

        rb.isKinematic = true;

        DiscHandle.SetParent(transform);

        _thrown = false;

        curveAmount = 0f;

        DiscHandle.localPosition = Vector3.zero;
        DiscHandle.localEulerAngles = Vector3.zero;

        CurrentDisc.transform.localEulerAngles = Vector3.zero;

        positionReset = true;
    }

    private void CharacterControl()
    {
        Vector3 inputMov = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);

        transform.Rotate(inputMov * rotateCharacterSpeed);

        Vector3 adjustedRot = transform.rotation.eulerAngles;
        adjustedRot.z = 0f;
        transform.rotation = Quaternion.Euler(adjustedRot);
    }
}
```
</details>

</details>

<details>
<summary><code>GolfCam.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCam : MonoBehaviour
{
    private Transform cameraTransform;

    private bool followDisc = false;

    private FlyingDiscRB golfer;

    private GameObject currentDisc;

    public Transform CameraTarget;

    private float scroll;

    public Vector3 startingPos = Vector3.zero;


    private void Start()
    {
        golfer = GetComponent<FlyingDiscRB>();
        cameraTransform = Camera.main.transform.parent;
        currentDisc = golfer.DiscHandle.gameObject;

        startingPos = Camera.main.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (followDisc) {
            // Camera follows the disc once thrown:
            //   Camera: LMouse
            //   Zoom: scroll wheel (distance clamped)

            cameraTransform.position = currentDisc.transform.position;

            cameraTransform.RotateAround(currentDisc.transform.position, Vector3.up, Input.GetAxis("Mouse X") * golfer.rotateCharacterSpeed);

            scroll = Mathf.Lerp(scroll, Input.mouseScrollDelta.y / 4f, Time.deltaTime * 2f);

            Camera.main.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(Camera.main.transform.localPosition.z + scroll, -5f, -0.13f));

            Camera.main.transform.LookAt(currentDisc.transform, Vector3.up);

            float rot = cameraTransform.rotation.eulerAngles.y;
            cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.eulerAngles.x + -Input.GetAxis("Mouse Y"), rot, 0);
        }
        else {
            // If disc is not thrown we reset our orientation
            cameraTransform.SetPositionAndRotation(CameraTarget.position, golfer.transform.rotation);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            followDisc = true;
        }

        // Flag disc to begin returning
        //  - reset camera's orientation

        if (Input.GetKeyDown(KeyCode.R))
        {
            followDisc = false;

            Camera.main.transform.localEulerAngles = new Vector3(5f, 0f, 0f);
            Camera.main.transform.localPosition = startingPos;
        }
    }
}

```
</details>

</details>

<details>
<summary><code>TriggerDiscHandler.cs</code></summary>

```
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

```
</details>


## Optimizations

What optimizations did you make in your code? E.g. refactors, performance improvements, accessibility


## Demo

Insert gif or link to demo

