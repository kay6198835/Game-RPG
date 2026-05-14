using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = Vector3.one*GameConstants.SettingStats.GAME_SCALE;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Input.GetAxisRaw(GameConstants.Input.HORIZONTAL), Input.GetAxisRaw(GameConstants.Input.VERTICAL),0) *Time.deltaTime*10;
    }
}
