using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.U2D.IK;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    PlayerController playerController;
    PlayerMagic playerMagic;
    [SerializeField] GameObject skeletonRight;
    IKManager2D ikmRight;
    Animator animRight;
    [SerializeField] GameObject skeletonLeft;
    IKManager2D ikmLeft;
    Animator animLeft;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerMagic = GetComponent<PlayerMagic>();
        animRight = skeletonRight.GetComponent<Animator>();
        animRight.enabled = true;
        ikmRight = skeletonRight.GetComponent<IKManager2D>();
        animLeft = skeletonLeft.GetComponent<Animator>();
        ikmLeft = skeletonLeft.GetComponent<IKManager2D>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            animator.SetBool("castMagic", true);
        else
            animator.SetBool("castMagic", false);

        if (Input.GetKeyDown(KeyCode.C))

            animator.SetBool("castMelee", true);
        else
            animator.SetBool("castMelee", false);

    }
    private void FixedUpdate()
    {
        print("isgrounded = " + IsGrounded());

        float xVelocity = rb.velocity.x;
        float yVelocity = rb.velocity.y;

        animator.SetFloat("xVelocity", xVelocity);
        animator.SetFloat("yVelocity", yVelocity);

        if (yVelocity > 0.5f && !IsGrounded())
            animator.SetBool("isJumping", true);

        else if (IsGrounded())
            animator.SetBool("isJumping", false);

        if (yVelocity < -2)
            animator.SetBool("isFalling", true);
        else
            animator.SetBool("isFalling", false);

        if (IsGrounded())
            animator.SetBool("isGrounded", true);
        else
            animator.SetBool("isGrounded", false);

        if (rb.transform.localScale.x == 1)
        {
            skeletonRight.transform.localPosition = Vector3.zero;
            ikmLeft.enabled = false;
            ikmRight.enabled = true;
            animator = animRight;

            skeletonLeft.SetActive(false);
            skeletonRight.SetActive(true);
        }
        if (rb.transform.localScale.x == -1)
        {
            skeletonLeft.transform.localPosition = Vector3.zero;
            ikmRight.enabled = false;
            ikmLeft.enabled = true;
            animLeft.enabled = true;
            animator = animLeft;

            skeletonLeft.SetActive(true);
            skeletonRight.SetActive(false);
        }


    }

    [SerializeField] Transform groundRayObject;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundRayRadius;
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundRayObject.position, groundRayRadius, groundLayer);
    }
}
