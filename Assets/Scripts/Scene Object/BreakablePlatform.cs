using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour {

    [Tooltip("This is public just so you can test it out in the inspector, idealy some game system will change this once.")]
    public bool trigger = false;

    public GameObject platform;
    public GameObject cracks;
    public GameObject pieces;
    public GameObject particles;

    [Header("How long until it colapses")]
    public float cracksTimer;
    WaitForSeconds waitCracksTimer;

    Collider2D myCollider;

    private void Start()
    {
        waitCracksTimer = new WaitForSeconds(cracksTimer);
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (trigger == true)
        {
            StartCoroutine(TriggerTrunk());
            trigger = false;
        }
    }

    IEnumerator TriggerTrunk()
    {
        cracks.SetActive(true);
        particles.SetActive(true);

        yield return waitCracksTimer;

        cracks.SetActive(false);
        if (myCollider != null)
            myCollider.enabled = false;

        platform.SetActive(false);

        pieces.SetActive(true);
    }

    //Trigger the object's action when the player enters the collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            trigger = true;
        }
    }




}
