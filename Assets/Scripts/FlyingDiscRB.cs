using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyingDiscRB : MonoBehaviour
{
    public float speed = 5f;
    //public float drag = 1f;
    public float lift = 1f;

    public Transform DiscHandle;

    //private Vector3 _velocity = Vector3.zero;

    //private Vector3 _moveForce = Vector3.zero;

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

    private bool grounded = false;

    public float distToGround = 1f;

    public float curvePower = 0.2f;


    private void Start()
    {
        _currSpeed = speed;
        //_moveForce = (_currSpeed * DiscHandle.forward.normalized) * Time.deltaTime;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool StateThrown()
    {
        return _thrown;
    }

    private void CalcLift()
    {
        // Define drag
        //Vector3 dragForce = (-drag * rb.velocity) * Time.deltaTime;

        // Define lift
        Vector3 nForce = lift * rb.velocity.sqrMagnitude * Vector3.Project(-Physics.gravity, DiscHandle.up.normalized).normalized;

        // Clamp lift to gravity's magnitude 
        if (nForce.magnitude > Physics.gravity.magnitude)
            nForce = Vector3.Project(-Physics.gravity, DiscHandle.up.normalized);

        Vector3 liftForce = Time.deltaTime * nForce;

        // Define gravity
        //Vector3 gravity = Physics.gravity * Time.deltaTime;

        //Update throw force and direction
        //_moveForce = (_currSpeed * DiscHandle.forward.normalized) * Time.deltaTime;

        // Curve mechanics
        Vector3 sideDir = Vector3.Cross(DiscHandle.up, rb.velocity).normalized;
        Vector3 curve = (sideDir * curveAmount * curvePower) * Time.deltaTime;

        // Apply all forces to our velocity
        //_velocity += gravity + _moveForce + dragForce + liftForce + curve;
        rb.velocity += (liftForce + curve);

        // Update the position
        //DiscHandle.position += (_velocity) * Time.deltaTime;

        _debugText.text = "Lift: " + liftForce.magnitude + "Gravity: " + Physics.gravity.magnitude + "\nVelocity: " + rb.velocity.magnitude;
    }

    private void MoveDisc()
    {
        _currSpeed = Mathf.Lerp(_currSpeed, 0f, Time.deltaTime * 6f);

        float rotSpeed = (-curveAmount * _OrientSpeed) / rb.velocity.magnitude;
        //DiscHandle.RotateAround(DiscHandle.position, _velocity.normalized, rotSpeed * Time.deltaTime);

        //CurrentDisc.transform.Rotate(new Vector3(0, rotateDiscSpeed * Time.deltaTime, 0));

    }

    private void OnCollisionStay(Collision collision)
    {
        print(collision.collider.tag);
    }

    private void TiltAngle()
    {
        if (Input.GetMouseButton(1))
        {
            // Disc tilting (Anhyzer/Hyzer)

            float mouseX = Input.GetAxis("Mouse X");
            if (mouseX > 0)
            {
                if (curveAmount < maxCurve)
                {
                    curveAmount += mouseX * tiltSpeed * Time.deltaTime;
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
        //_velocity = Vector3.zero;

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
        _currSpeed = 0f;

        rb.isKinematic = true;

        DiscHandle.SetParent(transform);

        _thrown = false;

        curveAmount = 0f;
        //_moveForce = Vector3.zero;

        DiscHandle.localPosition = Vector3.zero;
        DiscHandle.localEulerAngles = Vector3.zero;

        //DiscParent.localEulerAngles = Vector3.zero;
        CurrentDisc.transform.localEulerAngles = Vector3.zero;

        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;

        //discReturned = true;

        positionReset = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
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

        if (Input.GetMouseButtonDown(0))
        {
            ThrowDisc();
        }

        if (_thrown)
        {
            MoveDisc();
            CalcLift();

            
        }
        else
        {
            TiltAngle();
        }

        UIAngle();

        if (!Input.GetMouseButton(1))
            CharacterControl();
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