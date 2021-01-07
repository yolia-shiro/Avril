using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffRotate : MonoBehaviour
{
    public float rotateSpeed;
    public float throwForce;
    public float gravityScale;

    private bool isRotating;
    private Rigidbody2D staffRigidbody;
    private Collider2D myCollider;
    // Start is called before the first frame update

    private void OnEnable()
    {
        myCollider = GetComponentInChildren<Collider2D>();
        myCollider.enabled = true;

        isRotating = true;
        staffRigidbody = GetComponent<Rigidbody2D>();
        staffRigidbody.bodyType = RigidbodyType2D.Dynamic;
        staffRigidbody.gravityScale = gravityScale;
        staffRigidbody.AddForce(transform.right * throwForce, ForceMode2D.Impulse);
    }

    private void OnDisable()
    {
        myCollider.enabled = false;
        staffRigidbody.bodyType = RigidbodyType2D.Kinematic;
        staffRigidbody.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0, 0, rotateSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.enabled)
        {
            return;
        }
        if (collision.CompareTag("Ground"))
        {
            isRotating = false;
            staffRigidbody.velocity = Vector2.zero;
            staffRigidbody.gravityScale = 0.0f;
        }
    }


}
