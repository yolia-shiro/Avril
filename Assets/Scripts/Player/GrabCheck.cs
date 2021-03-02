using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCheck : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.PlayerState.isDownPlatform)
        {
            return;
        }
        Collider2D collider = collision.collider;
        if (collider.CompareTag("Ground") && !playerController.PlayerState.isGround)
        {
            ContactPoint2D contact = collision.contacts[0];
            Vector3 pos = contact.point;
            float topPos = collider.bounds.max.y;
            float bottomPos = collider.bounds.min.y;
            if (pos.y <= topPos + 0.1f && pos.y >= (topPos - playerController.grabDiff) - 0.1f || collider is EdgeCollider2D)
            {
                if (collider is EdgeCollider2D)
                {
                    //计算EdgeCollider2D上的碰撞点的详细位置
                    Vector2 center = collider.bounds.center;
                    Vector2 point1 = center + (collider as EdgeCollider2D).points[0];
                    Vector2 point2 = center + (collider as EdgeCollider2D).points[1];
                    float k = (point1.y - point2.y) / (point1.x - point2.x);
                    float other = Mathf.Tan(collider.transform.eulerAngles.z * Mathf.Deg2Rad);
                    float relK = (k + other) / (1 - k * other);
                    relK = collider.transform.eulerAngles.y == 0 ? relK : -relK;
                    float b = center.y - relK * center.x;
                    float grabPosX = pos.x + playerController.grabOffset.x * (playerController.transform.right.x < 0 ? -1 : 1);

                    StartCoroutine(playerController.Grab(
                        new Vector3(grabPosX,
                        relK * grabPosX + b + playerController.grabOffset.y,
                        transform.position.z)));
                }
                else
                {
                    StartCoroutine(playerController.Grab(
                        new Vector3(transform.position.x + playerController.grabOffset.x * (playerController.transform.right.x < 0 ? -1 : 1), 
                        topPos + playerController.grabOffset.y, 
                        transform.position.z)));
                }
            }
        }
    }
}
