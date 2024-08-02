using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCam : MonoBehaviour
{
    private Transform cameraTransform;

    public Vector3 cameraFollowOffset;

    private bool followDisc = false;

    //private Golfer myGolfer;
    private FlyingDiscRB golfer;

    private GameObject currentDisc;

    public Vector3 followCamOffset = Vector3.zero;
    public float followCamHeight = 2f;

    public Transform CameraTarget;

    private float scroll;

    public float followSpeed = 4f;

    public Vector3 startingPos = Vector3.zero;

    void Start()
    {
        golfer = GetComponent<FlyingDiscRB>();
        cameraTransform = Camera.main.transform.parent;
        currentDisc = golfer.DiscHandle.gameObject;

        startingPos = Camera.main.transform.localPosition;
    }

    private void LateUpdate()
    {
        if (followDisc) {
            cameraTransform.position = currentDisc.transform.position;

            cameraTransform.RotateAround(currentDisc.transform.position, Vector3.up, Input.GetAxis("Mouse X") * golfer.rotateCharacterSpeed);
            //cameraTransform.RotateAround(currentDisc.transform.position, cameraTransform.right, -Input.GetAxis("Mouse Y") * golfer.rotateCharacterSpeed);

            scroll = Mathf.Lerp(scroll, Input.mouseScrollDelta.y / 4f, Time.deltaTime * 2f);
            Camera.main.transform.localPosition += new Vector3(0, 0, scroll);

            if (Camera.main.transform.localPosition.z > -0.13f)
                Camera.main.transform.localPosition = new Vector3(0, 0, -0.13f);

            Camera.main.transform.LookAt(currentDisc.transform, Vector3.up);


            float rot = cameraTransform.rotation.eulerAngles.y;
            cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.eulerAngles.x + -Input.GetAxis("Mouse Y"), rot, 0);
        }
        else {
            cameraTransform.position = CameraTarget.position;
            cameraTransform.rotation = golfer.transform.rotation;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            followDisc = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            followDisc = false;

            Camera.main.transform.localEulerAngles = new Vector3(5f, 0f, 0f);
            Camera.main.transform.localPosition = startingPos;
        }
    }
}
