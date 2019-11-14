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
    public bool grabbed_on = false;
    bool pellet_in_workCell = false;
    Collider2D player_coll;
    Collider2D[] work_cell;

    public float process_rate;

    private float scale_size = 0;
    private Color pellet_color = new Color(1, 1, 1);

    private void Start()
    {
    }

    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += tappedHandler2;
    }

    private void OnDisable()
    {
        GetComponent<TapGesture>().Tapped -= tappedHandler2;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag.Equals("pellet"))
		{
			player_coll = collision.collider;

        	contact_on = true;

       //     Debug.Log(" touched pellet " + contact_on );
		}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag.Equals("pellet"))
		{
			player_coll = collision.collider;

        	contact_on = false;
		}
    }

    //this happens when the pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {

        if (contact_on)
        {

            Debug.Log(" grabbed = " + grabbed_on + "  pell in WC = " + pellet_in_workCell);

            if (!grabbed_on && !pellet_in_workCell)
            {

                grabbed_on = true;

                player_coll.transform.SetParent(transform);

              //  disable pellet collider
               player_coll.GetComponent<Collider2D>().enabled = false;
                player_coll.GetComponent<Rigidbody2D>().isKinematic = true;

                 player_coll.transform.position = transform.position;

                GetComponent<SpriteRenderer>().color = player_coll.GetComponent<SpriteRenderer>().color;

            }

        }

    }
}
