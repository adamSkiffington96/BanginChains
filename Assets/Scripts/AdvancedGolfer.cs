using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedGolfer : MonoBehaviour
{
    private Transform HandleDisc;

    public GameObject CurrentDisc;

    private Animator discAnims;

    public Transform DiscParent;

    //public float rotateDiscSpeed = 0f;

    public float throwDiscPower = 3f;

    public float rotateCharacterSpeed = 1f;

    private bool throwingDisc = false;

    public Text velocityText;

    private Vector3 _force;

    private Vector3 _velocity;

    public float curveAmount = 0.1f;
    public float maxCurve = 6f;

    public float discAddPowerFalloff = 1f;

    public float bankFactor = 1;
    public float bankSmoothing = 10f;
    //public float angleLiftPower = 1f;

    public float minimumLift = 1f;
    public float tiltSpeed = 1f;

    private bool discReturned = true;

    public Rigidbody rb;

    // -- UI --

    public GameObject tiltIndicator;

    public TextMeshProUGUI AngleText;


    void Start()
    {
        HandleDisc = CurrentDisc.transform.parent.parent;

        rb = HandleDisc.GetComponent<Rigidbody>();

        discAnims = CurrentDisc.GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool HoldingDisc()
    {
        return discReturned;
    }

    private void ThrowDisc()
    {
        discReturned = false;

        HandleDisc.parent = null;

        throwingDisc = true;

        _force = HandleDisc.transform.forward.normalized * throwDiscPower * Time.deltaTime;
    }

    public void Grounded()
    {
        throwingDisc = false;
    }

    private void DiscReturn()
    {
        throwingDisc = false;

        curveAmount = 0f;

        _force = Vector3.zero;

        HandleDisc.SetParent(transform);
        HandleDisc.localPosition = Vector3.zero;
        HandleDisc.localEulerAngles = Vector3.zero;

        _velocity = Vector3.zero;

        DiscParent.localEulerAngles = Vector3.zero;
        CurrentDisc.transform.localEulerAngles = Vector3.zero;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        discReturned = true;
    }

    private void Update()
    {
        if (throwingDisc)
        {
            // Decrease throw force based on set constant (drag power)
            _force = Vector3.Lerp(_force, HandleDisc.transform.forward.normalized * 0.001f, discAddPowerFalloff * Time.deltaTime);

            // Define gravity
            Vector3 gravity = Physics.gravity * Time.deltaTime;

            // Current speed of disc
            float speed = _velocity.magnitude * Time.deltaTime;

            // Create disc normal vector
            Vector3 normal = Vector3.Project(-Physics.gravity, HandleDisc.up.normalized);

            // Lift force using disc speed and normal vector
            Vector3 liftForce = minimumLift * normal * speed;
            if(liftForce.magnitude > normal.magnitude)
                liftForce = normal;

            // Curve mechanics
            Vector3 sideDir = Vector3.Cross(HandleDisc.up, _velocity).normalized;
            Vector3 curve = (sideDir * curveAmount) * Time.deltaTime;

            // Apply gravity, lift, curve, and forward throwing force, to current velocity
            _velocity += ((gravity + (liftForce * Time.deltaTime)) + _force) + curve;   // allow gravity to work on our velocity vector

            // add newly created velocity to disc position
            HandleDisc.position += ((_velocity) * Time.deltaTime);    // move us this frame according to our speed

            // rotate disc at constant speed (simple implementation)
            //CurrentDisc.transform.Rotate(new Vector3(0, rotateDiscSpeed * Time.deltaTime, 0));
        }
        else
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
                        HandleDisc.eulerAngles = new Vector3(HandleDisc.eulerAngles.x, HandleDisc.eulerAngles.y, -curveAmount * bankFactor);
                    }
                }
                else if (mouseX < 0)
                {
                    if (curveAmount > -maxCurve)
                    {
                        curveAmount += mouseX * tiltSpeed * Time.deltaTime;
                        HandleDisc.eulerAngles = new Vector3(HandleDisc.eulerAngles.x, HandleDisc.eulerAngles.y, -curveAmount * bankFactor);
                    }
                }

                curveAmount = Mathf.Clamp(curveAmount, -maxCurve, maxCurve);
            }

            float angle = curveAmount * bankFactor;
            float maxAngle = maxCurve * bankFactor;

            float pos = (angle / maxAngle) * 135f;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            DiscReturn();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            InspectDisc();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (discReturned)
            {
                if (!throwingDisc)
                    ThrowDisc();
            }
        }

        if (Input.GetMouseButton(1) == false)
            CharacterControl();

        velocityText.text = "Velocity: " + _velocity.magnitude;
    }

    private void LateUpdate()
    {
        //SetRotation();
    }

    private void SetRotation()
    {
        
    }

    private void InspectDisc()
    {
        discAnims.SetTrigger("inspect");
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
