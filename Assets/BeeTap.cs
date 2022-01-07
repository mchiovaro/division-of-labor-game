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
    bool contact_on = false;
    public bool grabbed_on;

    Collider2D player_coll; // collider for the players
    Collider2D[] work_cell; // collider for the cells

    private int dropTapCounter = 0;
    //public float process_rate;

    private float scale_size = 0;
    private Color pellet_color = new Color(1, 1, 1);

    void Awake ()
    {

      grabbed_on = false;
      Debug.Log("GRABBED SET TO FALSE");

    }

    private void Start()
    {

      //beeFree = GameObject.FindGameObjectWithTag("bee_free");
      //beeRestricted = GameObject.FindGameObjectWithTag("bee_restricted");

      //beeFree.GetComponent<BeeTap>().grabbed_on = false;
      //beeRestricted.GetComponent<BeeTap>().grabbed_on = false;
      //Debug.Log("free and restrictured set to GRABBED FALSE");

    }

    // updates once per frame - keeping players within the game field (specifically the upper and lower bounds)
    void Update()
    {
        // if the player is going past the top edge, move them back down
        if(transform.position.y >= 4.9f)
        {
          transform.position = new Vector3(transform.position.x, 4.9f, 0);
        }

        // if player is going past bottom edge, move them back up
        if(transform.position.y <= -4.9f)
        {
          transform.position = new Vector3(transform.position.x, -4.9f, 0);
        }

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
        	contact_on = true;
          Debug.Log("CONTACT ON");
		    }
    }

    //
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag.Equals("pellet"))
		    {
          player_coll = collision.collider;
        	contact_on = false;
          Debug.Log("CONTACT OFF");
		    }
    }

    // is this not doing anything for us?
  //  private void OnTriggerEnter2D(Collider2D collider)
  //  {
  //      if (collider.tag.Equals("pellet"))
  //      {
  //          player_coll = collider;
  //          contact_on = true;
            //Debug.Log(" touched pellet ");
  //      }
  //  }

    // is this not doing anything for us?
    //private void OnTriggerExit2D(Collider2D collider)
    //{
    //    if (collider.tag.Equals("pellet"))
    //    {
    //        player_coll = collider;
    //        contact_on = false;
    //    }
    //}

    //this happens when the pellet is tapped?
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {
        //Debug.Log("PELLET WAS TAPPED");

        // if the player is in contact with the pellet
        //if (contact_on)
      //  {
            // if they are not holding anything and there is a pellet in the cell
          //  if (!grabbed_on && player_coll.GetComponent<PelletScript>().pellet_in_workCell) // && !pellet_in_workCell)
          //  {

              //  dropTapCounter++;
                // if they've tapped three times
                //if (dropTapCounter == 3)
                //{
                    // should this be a picked up tag?
                  //  player_coll.GetComponent<PelletScript>().saveToBuffer("GR_SPAWN");
                    //Debug.Log(player_coll.gameObject.GetComponent<PelletScript>().player_coll.gameObject.tag);
                  //  grabbed_on = true;
                  //  Debug.Log("Grabbed on " + grabbed_on);

                    // ?
                    //player_coll.transform.SetParent(transform);

                    //  disable pellet collider
                  //  player_coll.GetComponent<Collider2D>().enabled = false;
                  //  player_coll.GetComponent<Rigidbody2D>().isKinematic = true;

                    // ?
                //    player_coll.transform.position = transform.position;

                    // ? Put the pellet on their back?
              //      GetComponent<SpriteRenderer>().color = player_coll.GetComponent<SpriteRenderer>().color;

                    // reset the counter for the next pellet
            //        dropTapCounter = 0;

          //      }

          //  }

          //  else if (player_coll.GetComponent<PelletScript>().pellet_in_workCell && transform.tag.Equals("bee_free"))
          //  {
                //maybe do something
          //  }

        //}

    }
}
