using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_IK : MonoBehaviour
{
    private float rightFootWeight;
    private float leftFootWeight;

    [SerializeField] [Range(0,0.2f)] private float distanceToGround;
    [SerializeField][Range(0, 3f)] private float distanceOFFset;

    private Animator animator;
    [SerializeField] private LayerMask layerMask;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

   
    void Update()
    {
        
    }


    private void OnAnimatorIK(int layerIndex)
    {
        #region Foot IK
        #region Left Foot IK

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

        RaycastHit hit;
        Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot),Vector3.down);

        if(Physics.Raycast(ray, out hit,distanceToGround + distanceOFFset,layerMask))
        {
            if(hit.transform.tag == "Walkable")
            {
                Vector3 footPosition = hit.point;

                footPosition.y += distanceToGround;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot,footPosition);

                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward, hit.normal));
            }
        }
        #endregion

        #region Right Foot IK

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

         ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot), Vector3.down);

        if (Physics.Raycast(ray, out hit, distanceToGround + distanceOFFset, layerMask))
        {
            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPosition = hit.point;

                footPosition.y += distanceToGround;

                animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);

                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward, hit.normal));
            }
        }
        #endregion
        #endregion
    }
}
