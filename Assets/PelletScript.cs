/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;


/// <exclude />
public class PelletScript : MonoBehaviour
{
    bool contact_on = false;
    bool grabbed_on = false;
    bool pellet_in_workCell = false;
    Collider2D player_coll;
    Collider2D[] work_cell;

    public float process_rate;

    private float scale_size = 0;
    private Color pellet_color = new Color(1, 1, 1);

    private Transform yellow_disk;

    private void Start()
    {
        work_cell = GameObject.FindGameObjectWithTag("work_row_middle").GetComponentsInChildren<Collider2D>();
        
        yellow_disk = transform.GetChild(0);
        
        yellow_disk.localScale = new Vector3(scale_size, scale_size, 0);

        Debug.Log(" colliders = " + work_cell.Length);
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
        player_coll = collision.collider;

        contact_on = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        contact_on = false;

    }

       private void OnTriggerEnter2D(Collider2D collider_)
    {
        player_coll = collider_;

        contact_on = true;
    }

    private void OnTriggerExit2D(Collider2D collider_)
    {
        contact_on = false;

    }

    //this happens when the pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {
        //check if the pellet is in any of the work cells
        foreach (Collider2D collInd in work_cell)
        {
            if (collInd.bounds.Contains(transform.position))
            {
                pellet_in_workCell = true;
            }

        }
        if (pellet_in_workCell)
        {
            //   pellet_in_workCell = false;

            if (scale_size != 1)
            {
                if (scale_size + process_rate < 1)
                {
                    scale_size += process_rate;

                    yellow_disk.localScale = new Vector3(scale_size, scale_size, 0);
                }
                else
                {
                    scale_size = 1;

                    Destroy(yellow_disk.gameObject);
                    GetComponent<SpriteRenderer>().color = new Color(1,204.0f/255,0);


               //     yellow_disk.localScale = new Vector3(scale_size, scale_size, 0);
                }
            }

        }
    }
}
