using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffRotate : MonoBehaviour
{
    public float rotateSpeed;
    public float throwForce;

    //Move
    private float g;
    private float startSpeed;
    private float startAngle;
    private float moveTime;
    private Vector3 startLocalPosition;
    private Quaternion targetRotation;
    public float rotationDelta;

    private bool isRotating;
    private Rigidbody2D staffRigidbody;
    private Collider2D myCollider;

    private void OnEnable()
    {
        myCollider = GetComponentInChildren<Collider2D>();
        myCollider.enabled = true;

        isRotating = true;
        staffRigidbody = GetComponent<Rigidbody2D>();
        staffRigidbody.bodyType = RigidbodyType2D.Dynamic;
        staffRigidbody.gravityScale = 0.0f;
        //staffRigidbody.AddForce(transform.right * throwForce, ForceMode2D.Impulse);
        startAngle = Mathf.Deg2Rad * transform.eulerAngles.z;
        startLocalPosition = transform.localPosition;
        moveTime = 0;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void OnDisable()
    {
        myCollider.enabled = false;
        staffRigidbody.bodyType = RigidbodyType2D.Kinematic;
        staffRigidbody.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRotating)
        {
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0, 0, rotateSpeed * Time.deltaTime));
            float dx = startSpeed * Mathf.Cos(startAngle) * moveTime;
            float dy = startSpeed * Mathf.Sin(startAngle) * moveTime + 0.5f * g * moveTime * moveTime;
            transform.localPosition = startLocalPosition + new Vector3((transform.right.x < 0 ? -1 : 1) * dx, dy, 0);
            //float vXAxis = startSpeed * Mathf.Cos(startAngle);
            //float vYAxis = startSpeed * Mathf.Sin(startAngle) + g * moveTime;
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Atan2(vYAxis, vXAxis) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationDelta * Time.fixedDeltaTime);
            moveTime += Time.fixedDeltaTime * 2;
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

    public void SetMoveAttr(float gravity, float speed, float angle)
    {
        g = gravity;
        startSpeed = speed;
        startAngle = angle;
    }
}
