using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PriestAnimation : MonoBehaviour
{
    [SerializeField] GameObject priestMagicing;
    [SerializeField] GameObject priestPatrolling;

    Animator animMagic;
    Animator animPatrol;

    PriestMovement priestMovement;
    PriestMagic priestMagic;

    Rigidbody2D rb;

    void Awake()
    {
        animMagic = priestMagicing.GetComponent<Animator>();
        animPatrol = priestPatrolling.GetComponent<Animator>();
        priestMovement = GetComponent<PriestMovement>();
        priestMagic = GetComponent<PriestMagic>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        priestMagicing.SetActive(false);
    }

    void FixedUpdate()
    {
        if (priestMagic.priestIsMagicking)
        {
            priestPatrolling.SetActive(false);
            priestMagicing.SetActive(true);
        }

        if (!priestMagic.priestIsMagicking)
        {
            priestMagicing.SetActive(false);
            priestPatrolling.SetActive(true);
        }

        if (priestMagic.priestShouldAttack && priestMagic.priestIsMagicking)
        {
            animMagic.SetBool("isAttacking", true);
        }
        else
        {
            animMagic.SetBool("isAttacking", false);
        }
    }
}
