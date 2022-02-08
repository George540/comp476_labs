using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public CharacterController cc;
    private float yVelocity;

    private void Update()
    {
        Camera cam = Camera.main;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = cam.transform.right * horizontal + cam.transform.forward * vertical;
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);

        if (movement.magnitude != 0)
        {
            transform.rotation = Quaternion.LookRotation(movement);
            animator.SetBool("walking", true);

            if (Input.GetKey(KeyCode.LeftShift))
                animator.SetBool("running", true);
            else
                animator.SetBool("running", false);
        }
        else
        {
            animator.SetBool("walking", false);
            animator.SetBool("running", false);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetFloat("speed", 1.2f);
        }
        else
        {
            animator.SetFloat("speed", 1);
        }

        if (cc.isGrounded)
        {
            yVelocity = 0;
        }

        yVelocity = -4 * Time.deltaTime;
    }

    // called after update
    private void OnAnimatorMove()
    {
        Vector3 velocity = animator.deltaPosition;
        velocity.y = yVelocity;
        cc.Move(velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
        }
    }
}
