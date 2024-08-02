using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyingDiscRB : MonoBehaviour
{
    public float speed = 5f;
    //public float drag = 1f;
    public float liftModifier = 1f;

    public Transform DiscHandle;

    private float _currSpeed = 0f;

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
        _currSpeed = speed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            //  Moving the disc back to the player to make a new throw
            //  It also creates a cool effect, pulling the disc back into your hand rather than it magically re-appearing
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

        if (_thrown) {
            MoveDisc();
            CalcLift();
        }
        else {
            TiltAngle();
        }

        UIAngle();

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

        // Update the position
        //DiscHandle.position += (_velocity) * Time.deltaTime;

        _debugText.text = "Lift: " + liftForce.magnitude + "Gravity: " + Physics.gravity.magnitude + "\nVelocity: " + rb.velocity.magnitude;
    }

    private void MoveDisc()
    {
        _currSpeed = Mathf.Lerp(_currSpeed, 0f, Time.deltaTime * 6f);
    }

    private void OnCollisionStay(Collision collision)
    {
        print(collision.collider.tag);
    }

    private void TiltAngle()
    {
        if (Input.GetMouseButton(1))
        {
            // Disc tilting (Anhyzer/Hyzer) (RMB)

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

        _currSpeed = speed;

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
        _currSpeed = 0f;

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