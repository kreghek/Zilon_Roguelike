using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) /*&& Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Human Player Walk"*/)
        {
            Animator.Play("Walk");
        }
    }
}
