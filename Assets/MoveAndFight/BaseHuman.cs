using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    protected bool isMoving = false;

    private Vector3 tartgetPostion;

    public float speed = 1.2f;

    private Animator animator;

    public string desc = null;

    internal bool isAttacking = false;
    internal float attackTime = float.MinValue;

    public void Attack()
    {
        isAttacking = true;
        attackTime = Time.time;
        animator.SetBool("isAttacking", true);
    }

    public void AttackUpdate()
    {
        if (!isAttacking) return;
        if (Time.time - attackTime < 1.2f) return;
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    public void MoveTo(Vector3 pos)
    {
        tartgetPostion = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    public void MoveUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, tartgetPostion, speed * Time.deltaTime);
        transform.LookAt(tartgetPostion);
        if(Vector3.Distance(pos,tartgetPostion) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }

    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }
}
