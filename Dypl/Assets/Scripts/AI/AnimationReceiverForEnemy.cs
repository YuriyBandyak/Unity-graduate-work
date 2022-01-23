using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReceiverForEnemy : MonoBehaviour
{
    //references
    private Animator myAnimator;
    private SimpleEnemy parent;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        parent = transform.parent.GetComponent<SimpleEnemy>();
    }

    public void EndOfGetDamage()
    {
        myAnimator.SetBool("getDamage", false);
    }

    public void Attack()
    {
        parent.AnimationPointOfAttack();
    }

    public void EndOfAttack()
    {
        parent.IsAnimationOfAttackEnd();
    }

    public void Jump()
    {
        parent.JumpForAttack();
    }
}
