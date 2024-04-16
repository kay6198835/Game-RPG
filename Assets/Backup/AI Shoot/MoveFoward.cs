using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFoward : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float durationMove;

    private void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime);
        durationMove += speed*Time.deltaTime;
    }
}
