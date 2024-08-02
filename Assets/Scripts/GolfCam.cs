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
            // The camera follows the disc once thrown:
            //   Rotate around it using the mouse
            //   Zoom using scroll wheel (distance clamped)

            cameraTransform.position = currentDisc.transform.position;

            cameraTransform.RotateAround(currentDisc.transform.position, Vector3.up, Input.GetAxis("Mouse X") * golfer.rotateCharacterSpeed);

            scroll = Mathf.Lerp(scroll, Input.mouseScrollDelta.y / 4f, Time.deltaTime * 2f);

            Camera.main.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(Camera.main.transform.localPosition.z + scroll, -5f, -0.13f));

            Camera.main.transform.LookAt(currentDisc.transform, Vector3.up);

            float rot = cameraTransform.rotation.eulerAngles.y;
            cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.eulerAngles.x + -Input.GetAxis("Mouse Y"), rot, 0);
        }
        else {
            cameraTransform.SetPositionAndRotation(CameraTarget.position, golfer.transform.rotation);
        }
    }

    private void Update()
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
