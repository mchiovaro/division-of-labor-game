/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;

/// <exclude />
public class BeeTap : MonoBehaviour
{
    //bool contact_on = false;
    public bool grabbed_on;

    Collider2D player_coll; // collider for the players
    //Collider2D[] work_cell; // collider for the cells

    private Color pellet_color = new Color(1, 1, 1);

    // add reference to Exit_app_script to grab ie_condition
    Exit_app_script exitappscript;

    // define our objects
    GameObject work_row, drop_cells, spawn_cells, beeFree, beeRestrict;

    // read in ie_condition from Exit_app_script.cs
    private int ie_cond;

    void Awake ()
    {

      grabbed_on = false;
      Debug.Log("GRABBED SET TO FALSE");

      // find the cells
      work_row = GameObject.FindGameObjectWithTag("work_row_middle");
      drop_cells = GameObject.FindGameObjectWithTag("drop_cells");
      spawn_cells = GameObject.FindGameObjectWithTag("spawn_cells");

    }

    private void Start()
    {

      // find conditions
      exitappscript = FindObjectOfType<Exit_app_script>();
      ie_cond = exitappscript.ie_condition;

    }

    // updates once per frame - keeping players within the game field (specifically the upper and lower bounds)
    void Update()
    {

      beeFree = GameObject.FindGameObjectWithTag("bee_free");
      beeRestrict = GameObject.FindGameObjectWithTag("bee_restricted");

      //Debug.Log("position = " + GameObject.Find("player_blue").transform.position.x);

      checkBoundaries();
    }

    private void OnEnable()
    {
      //  GetComponent<TapGesture>().Tapped += tappedHandler2;
    }

    private void OnDisable()
    {
      //  GetComponent<TapGesture>().Tapped -= tappedHandler2;
    }

    // if the player collides with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if the collided object is a *pellet*
        if(collision.collider.tag.Equals("pellet"))
		    {
          // make the player a collider so that it doesn't pass through
          player_coll = collision.collider; // I don't think this is doing anything for us because it never defines player_coll
          // player_coll is the collided pellet

          // note that they are in contact (so that they can pick it up)
        	//contact_on = true;
          Debug.Log("CONTACT ON");
		    }
    }

    //
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag.Equals("pellet"))
		    {
          player_coll = collision.collider;
        	//contact_on = false;
          Debug.Log("CONTACT OFF");
		    }
    }

    //this happens when the player is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {
    }

    private void checkBoundaries()
    {
        // create barriers for left and right of work cells
        // let the player enter the cell a little (0.8f) so they can be in touch with the pellet to grab it
        float barrier_right = work_row.transform.GetChild(0).position.x + 0.8f;
        float barrier_left = work_row.transform.GetChild(0).position.x - 0.8f;

        // field barriers (y coord comes from middle so +/1 0.3 means a 0.2 overlap, the same as left/right)
        float field_top = drop_cells.transform.GetChild(0).position.y + 0.3f;
        float field_bottom = drop_cells.transform.GetChild(9).position.y - 0.3f;

        //if (gameObject.tranform.position.y < field_bottom) gameObject.transform.position.y = field_bottom;
        if (GameObject.Find("player_blue").transform.position.y < field_bottom) {
               //GameObject.Find("player_blue").transform.position = new Vector3(GameObject.Find("player_blue").transform.position.x, field_bottom, GameObject.Find("player_blue").transform.position.z);
         }


        // if ie_condition = 1, block restricted player and allow free player to pass.
        if (ie_cond == 1) {
           // if the restricted player hits the boundary, don't allow them to pass.
           if (GameObject.Find("player_blue").transform.position.x < barrier_right) {
                  GameObject.Find("player_blue").transform.position = new Vector3(barrier_right, GameObject.Find("player_blue").transform.position.y, GameObject.Find("player_blue").transform.position.z);
            }
        }

        // if ie_condition = 2, block free player and allow restricted player to pass.
        if (ie_cond == 2) {
           // if the restricted player hits the boundary, don't allow them to pass.
           if (GameObject.Find("player_red").transform.position.x > barrier_left) {
                  GameObject.Find("player_red").transform.position = new Vector3(barrier_left, GameObject.Find("player_red").transform.position.y, GameObject.Find("player_red").transform.position.z);
            }
        }

    }

}
