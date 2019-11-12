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
    bool grabbed_on = false;
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

            if (!grabbed_on && !pellet_in_workCell)
            {
                grabbed_on = true;
                


                player_coll.transform.SetParent(transform);

              //  disable pellet collider
               player_coll.GetComponent<Collider2D>().enabled = false;


                 player_coll.transform.position = transform.position;

                GetComponent<SpriteRenderer>().color = Color.white;

                //cancel collider
                //set position to = player_coll.transform
            }else
            {
                grabbed_on = false;
                transform.SetParent(null);

                //restore collider
                //drop right to the left 

            }

        }

    }
}
