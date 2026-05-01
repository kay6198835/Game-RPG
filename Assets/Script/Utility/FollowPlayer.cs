using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] Transform player;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Get),0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            Run();
        }
    }
    void Get()
    {
        player = GameObject.Find("PlayerTest(Clone)").transform;
    }
    void Run()
    {
        transform.position = player.position+new Vector3(0,0,-10);
    }
}
