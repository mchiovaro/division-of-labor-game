// This script records when a pellet spawns and is moved out of spawn cells.
// The actual spawning code is in Exit_app_script.cs under Update()

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCellScript : MonoBehaviour
{
    // start contact as false
    public bool contact_on = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // "Sent when another object enters a trigger collider attached to this object (2D physics only)."
    // when a pellet spawns inside a spawn cell, it triggers it (does not physically collide)
    private void OnTriggerEnter2D(Collider2D other)
    {
        //contact_on = true;
        // this logs the contacts for both the pellets IN the cells and the player into the side of the cell
        //Debug.Log("player made contact");
    }

    // "Sent when another object leaves a trigger collider attached to this object (2D physics only)."
    private void OnTriggerExit2D(Collider2D other)
    {
        //contact_on = false;
        //Debug.Log("player moved away");
    }

}
