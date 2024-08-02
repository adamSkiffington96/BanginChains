using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Golfer : MonoBehaviour
{
    private Transform HandleDisc;

    public GameObject CurrentDisc;

    private Animator discAnims;

    public Transform DiscParent;

    public float rotateDiscSpeed = 0f;

    public float throwDiscSpeed = 3f;

    public float rotateCharacterSpeed = 1f;

    private bool throwingDisc = false;

    public Vector3 discTargetVelocity = Vector3.zero;

    private Vector3 currentDiscVelocity = Vector3.zero;

    //public Vector3 gravity = Vector3.down.normalized * 9.8f;

    private Vector3 prevDiscPosition = Vector3.zero;

    private Vector3 grav;

    private Vector3 discDirection = Vector3.zero;

    public Text velocityText;

    public float liftModifier = 1f;


    void Start()
    {
        HandleDisc = CurrentDisc.transform.parent.parent;

        discAnims = CurrentDisc.GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ThrowDisc()
    {
        HandleDisc.parent = null;

        discTargetVelocity = (transform.forward.normalized * throwDiscSpeed);

        currentDiscVelocity = discTargetVelocity;

        throwingDisc = true;
    }

    void Update()
    {
        DiscVelocity();

        GetLift();

        if (throwingDisc) {
            grav += Physics.gravity * Time.deltaTime;   // allow gravity to work on our velocity vector
            Vector3 gravity = grav * Time.deltaTime;




            float angleOfAttack = Vector3.Angle(HandleDisc.forward, currentDiscVelocity);

            Vector3 normalForce = Mathf.Clamp(angleOfAttack, 0.75f, 100f) * -grav * Time.deltaTime;

            velocityText.text = angleOfAttack.ToString();


            //HandleDisc.position += gravity + currentDiscVelocity;
            HandleDisc.position += (grav * Time.deltaTime) + currentDiscVelocity + normalForce;


            //currentDiscVelocity = Vector3.Lerp(currentDiscVelocity, HandleDisc.forward.normalized * 0f, Time.deltaTime * angleOfAttack);









        }


        if (Input.GetKeyDown(KeyCode.T)) {
            InspectDisc();
        }

        if(Input.GetMouseButtonDown(0)) {
            if(!throwingDisc)
                ThrowDisc();
        }

        CharacterControl();
    }

    private void GetLift()
    {
        Vector3 tiltAngle = discDirection.normalized - Vector3.down.normalized;

        //Debug.Log(tiltAngle);
        //velocityText.text = tiltAngle.magnitude.ToString();


    }

    private void DiscVelocity()
    {
        discDirection = HandleDisc.position - prevDiscPosition;

        float vel = discDirection.magnitude * 10f;

        ///Debug.Log(vel);

        prevDiscPosition = HandleDisc.position;
    }

    private void LateUpdate()
    {
        if(throwingDisc)
            SetRotation();
    }

    private void SetRotation()
    {
        DiscParent.transform.Rotate(new Vector3(0, rotateDiscSpeed * Time.deltaTime, 0));
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
