

using System;
using TouchScript.Gestures;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;


/// <exclude />
public class workCellScript : MonoBehaviour
{

    GameObject pellet_, pellet_holder;

    private GameObject beeFree, beeRestricted;

    public bool contact_on = false;

    public Dictionary<string, bool> my_colliders = new Dictionary<string, bool>() {
        { "bee_free", false }, { "bee_restricted", false }};

    public Dictionary<string, bool> carrying_pellet = new Dictionary<string, bool>() {
        { "bee_free", false }, { "bee_restricted", false }};

    private void Start()
    {
        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestricted = GameObject.FindGameObjectWithTag("bee_restricted");
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
        my_colliders[collision.collider.tag] = true;

        contact_on = true;

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        my_colliders[other.tag] = true;

        if (other.transform.childCount != 0)
        {
            foreach (Transform tt in other.transform)
                if (tt.tag.Equals("pellet"))
                {
                    pellet_ = tt.gameObject;
                    pellet_holder = other.gameObject;
                    pellet_.GetComponent<PelletScript>().pellet_holder = pellet_holder;
                }
        }

        contact_on = true;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.childCount != 0)
        {
            pellet_ = null;
        }

        my_colliders[other.tag] = false;

        contact_on = false;
    }

    //this happens when the pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {
        if (pellet_ != null)
        {

            pellet_.GetComponent<PelletScript>().saveToBuffer("DR_WORK");

            pellet_.transform.SetParent(null);
            pellet_.transform.position = transform.position;
            pellet_.GetComponent<Collider2D>().enabled = true;
            pellet_.GetComponent<PelletScript>().pellet_in_workCell = true;

            beeRestricted.GetComponent<BeeTap>().grabbed_on = false;
            beeFree.GetComponent<BeeTap>().grabbed_on = false;

            if(pellet_holder.tag.Equals("bee_free"))
            {
                beeFree.GetComponent<SpriteRenderer>().color = Color.red;
            }else
            {
                beeRestricted.GetComponent<SpriteRenderer>().color = Color.blue;
            }

          //  pellet_holder = null;

          //  beeFree.GetComponent<SpriteRenderer>().color = Color.red;
          //  beeRestricted.GetComponent<SpriteRenderer>().color = Color.blue;

            //young bee can't move again until pellet has been fully worked
            if (my_colliders["bee_restricted"] && !pellet_.GetComponent<PelletScript>().worked)
                beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
        }

    }

    public void resetParameters()
    {
        contact_on = false;
        my_colliders["bee_free"] = false;
        my_colliders["bee_restricted"] = false;
        carrying_pellet["bee_free"] = false;
        carrying_pellet["bee_restricted"] = false;

    }
}
