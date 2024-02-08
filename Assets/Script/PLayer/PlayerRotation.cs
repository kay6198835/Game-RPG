using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    //[SerializeField] float moveSpeed = 5f;

    //[SerializeField] Rigidbody2D rb;
    //[SerializeField] Camera cam;

    //Vector2 movement;
    //Vector2 mousePos;

    //void Update()
    //{
    //    movement.x = Input.GetAxisRaw("Horizontal");
    //    movement.y = Input.GetAxisRaw("Vertical");

    //    mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    //}

    private void FixedUpdate()
    {
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
