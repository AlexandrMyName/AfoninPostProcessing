using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_System : MonoBehaviour
{
    [Space]
    [Header("    ____Player settings with camera____    ")]
    [Space] [Space] [Space]
    [Header("Player settings")]
    [SerializeField][Range(250,500)] private float force_Jump;
    [SerializeField] private float speed_Walk;
    [SerializeField] private float speed_Run;
    [SerializeField] private float speed_CameraRotation;
    [SerializeField] private float speed_PlayerRotation;
    [Space] [Space]
    private Quaternion rot_Player;
    private Quaternion rot_Camera;
    
    GameObject camera_Main;
    GameObject camera_System;
    [Space] [Space]
    [Header("Camera settings")]
    public LayerMask mask_everything;
    public LayerMask mask_NoPlayer;
    public LayerMask mask_clearCamera;
    [SerializeField] private Vector3 camera_OffSet;
    [Space] [Space]
    public float RangeMin_camera;
    public float RangeMax_camera;
    public float minDistanceToPlayer;

    private float MouseX;
    private float MouseY;
    private float horizontal;
    private float vertical;

    private Animator animatorPlayer;
    private Rigidbody rbPlayer;
    private CapsuleCollider capsule;
    public LayerMask mask_Jump;

    void Start()
    {
        camera_Main = GameObject.Find("Camera");
        camera_System = GameObject.Find("Camera_Position");
        camera_Main.transform.position = camera_System.transform.TransformPoint(camera_OffSet);
        rot_Camera = Quaternion.identity;
        rot_Player = Quaternion.identity;
        animatorPlayer = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    
    void Update()
    {
        camera_System.transform.position = this.transform.position + Vector3.up * 1.7f;

        MouseX = Input.GetAxis("Mouse X") /** speed_CameraRotation*/;
        MouseY = Input.GetAxis("Mouse Y") /** speed_CameraRotation*/;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");


        if(isGrounded() && !Input.GetKey(KeyCode.LeftShift))
        {
            animatorPlayer.SetFloat("Horizontal", horizontal);
            animatorPlayer.SetFloat("Vertical", vertical);
        }
         if (Input.GetKey(KeyCode.LeftShift) && isGrounded())
        {
            animatorPlayer.SetBool("isRun", true);
        }
        else
        {
            animatorPlayer.SetBool("isRun", false);
        }
    }
    private void LateUpdate()
    {
        Camera_Behavior();
    }

    private  void Camera_Behavior()
    {
        rot_Camera.x += -MouseY * speed_CameraRotation;
        rot_Camera.y += MouseX * speed_CameraRotation;
        rot_Camera.z = 0f;

        rot_Camera.x = Mathf.Clamp(rot_Camera.x, RangeMin_camera, RangeMax_camera);

        camera_System.transform.rotation = Quaternion.Euler(rot_Camera.x, rot_Camera.y, rot_Camera.z);
        camera_Main.transform.LookAt(camera_System.transform.position);

        float maxDistance = Vector3.Distance(camera_System.transform.position, camera_System.transform.TransformPoint(camera_OffSet));
        float distance = Vector3.Distance(camera_System.transform.position,camera_Main.transform.position);

        RaycastHit hit;
        Ray ray = new Ray(camera_System.transform.position, camera_Main.transform.position - camera_System.transform.position);

        if(Physics.Raycast(ray,out hit, maxDistance, mask_clearCamera))
        {
            camera_Main.transform.position = hit.point;
            camera_Main.transform.LookAt(camera_System.transform.position);
        }
        if (distance <= minDistanceToPlayer)
        {
            camera_Main.GetComponent<Camera>().cullingMask = mask_NoPlayer;
        }
        else
        {
            camera_Main.GetComponent<Camera>().cullingMask = mask_everything;
        }
    }


    private void Player_Behavior()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? speed_Run : speed_Walk;
        Vector3 lookAtRot = new Vector3();
        lookAtRot.Set(horizontal, 0f, vertical);

        if (vertical == 1 )
        {
            rot_Player = camera_System.transform.rotation;
            rot_Player.x = 0;
            rot_Player.z = 0;

            this.transform.rotation = Quaternion.Slerp(transform.rotation, rot_Player, Time.fixedDeltaTime * speed_PlayerRotation);
            rbPlayer.MovePosition(this.transform.position + transform.forward  * speed * Time.fixedDeltaTime);
        }
        else if(vertical == -1 )
        {
            Quaternion rot = Quaternion.LookRotation(-camera_Main.transform.forward, Vector3.up);
            rot.x = 0;
            rot.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * speed_PlayerRotation);
            rbPlayer.MovePosition(this.transform.position + -transform.forward * vertical   * speed * Time.fixedDeltaTime);
        }
        else if(horizontal == 1)
        {
            Quaternion rot = Quaternion.LookRotation(camera_Main.transform.right, Vector3.up);
            rot.x = 0;
            rot.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * speed_PlayerRotation);
            rbPlayer.MovePosition(this.transform.position + transform.forward  * speed * Time.fixedDeltaTime);
        }
        else if (horizontal == -1)
        {
            Quaternion rot = Quaternion.LookRotation(-camera_Main.transform.right, Vector3.up);
            rot.x = 0;
            rot.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * speed_PlayerRotation);
            rbPlayer.MovePosition(this.transform.position + transform.forward * speed /** animatorPlayer.deltaPosition.magnitude*/ * Time.fixedDeltaTime);
        }
        else
        {
            rot_Player = this.transform.rotation;
        }


        if (isGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            animatorPlayer.SetTrigger("isJump");
            rbPlayer.AddForce(Vector3.up * force_Jump);
        }

        
    }
    private void OnAnimatorMove()
    {
        Player_Behavior();
    }


    private bool isGrounded()
    {
       Vector3 bounds = new Vector3(capsule.bounds.center.x,capsule.bounds.min.y, capsule.bounds.center.z);
       bool isGround =  Physics.CheckCapsule(capsule.bounds.center,bounds,0.1f,mask_Jump,QueryTriggerInteraction.Ignore) ? true : false; 
       return isGround;
    }
    
}
