using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAnimatorController : MonoBehaviour
{
    private Animator anim;
    private WizardController wizardController;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        wizardController = GetComponent<WizardController>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("state", wizardController.state);
        anim.SetBool("isJumpBackOver", wizardController.isJumpBackOver);
    }
}
