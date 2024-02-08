using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FX : MonoBehaviour
{
    private void Awake()
    {
        Invoke("DestroyItSeft", 0.5f);
    }
    void DestroyItSeft()
    {
        Destroy(gameObject);
    }
}
