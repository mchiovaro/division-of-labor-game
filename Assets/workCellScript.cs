

using System;
using TouchScript.Gestures;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;


/// <exclude />
public class workCellScript : MonoBehaviour
{

    GameObject pellet_;
    private GameObject beeFree, beeRestricted;
    private bool free_contact_cell = false;
    private bool restrict_contact_cell = false;
    public bool advanced_pellet;
    public bool contact_on = false;

    public Dictionary<string, bool> my_colliders = new Dictionary<string, bool>() {
        { "bee_free", false }, { "bee_restricted", false }};

    // do we need this since we have grabbed_on?
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

    }


    private void OnTriggerEnter2D(Collider2D collider_)
    {

        // if the collided object is bee free, set contact true and give a debug message
        if (collider_.tag.Equals("bee_free")){
          free_contact_cell = true;
          //Debug.Log(collider_.gameObject.tag + " contacted work cell");
        }

        // if the collided object is bee free, set contact true and give a debug message
        if (collider_.tag.Equals("bee_restricted")){
          restrict_contact_cell = true;
          //Debug.Log(collider_.gameObject.tag + " contacted work cell");
        }

        // if the player has a child object (pellet)
        if (collider_.transform.childCount != 0)
        {
            // for each child in their possession
            foreach (Transform tt in collider_.transform)
                    pellet_ = tt.gameObject;

        }

    }

    private void OnTriggerExit2D(Collider2D collider_)
    {
        // if there was a pellet and we moved away, reset pellet to null
        // so it can be identified on next trigger
        if (collider_.transform.childCount != 0)
        {
            pellet_ = null;
        }

        // if the collider object that moved away was bee free, set contact false and give debug message
        if (collider_.tag.Equals("bee_free"))
        {
            free_contact_cell = false;
            Debug.Log(collider_.gameObject.tag + " moved away from work cell");
        }

        // if the collider object that moved away was bee restricted, set contact false and give debug message
        if (collider_.tag.Equals("bee_restricted"))
        {
          restrict_contact_cell = false;
          Debug.Log(collider_.gameObject.tag + " moved away from work cell");
        }

    }

    //this happens when the cell is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {
        Debug.Log("TAPPED");
        //if restricted player has a pellet and is in contact with the cell
        if (pellet_ != null && restrict_contact_cell == true)
        {

            pellet_.GetComponent<PelletScript>().saveToBuffer("2_DROP_MID");

            // remove the player as the parent
            pellet_.transform.SetParent(null);

            // put pellet in the cell
            pellet_.transform.position = transform.position;
            pellet_.GetComponent<Collider2D>().enabled = true;

            // reset the bee to not be grabbed on in tapping script
            beeRestricted.GetComponent<BeeTap>().grabbed_on = false;
            restrict_contact_cell = false;
            // identify the pellet as semi-processed so that we can't pick it up from the right
            pellet_.GetComponent<PelletScript>().advanced_pellet = true;

            // reset the bee color once they've dropped the pellet
            beeRestricted.GetComponent<SpriteRenderer>().color = Color.blue;

            //young bee can't move again until pellet has been fully worked
            if (!pellet_.GetComponent<PelletScript>().worked)
                beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
        }

        //if free player has a pellet and is in contact with the cell
        if (pellet_ != null && free_contact_cell == true)
        {

            pellet_.GetComponent<PelletScript>().saveToBuffer("1_DROP_MID");

            // remove the player as the parent
            pellet_.transform.SetParent(null);

            // put pellet in the cell
            pellet_.transform.position = transform.position;
            pellet_.GetComponent<Collider2D>().enabled = true;

            // reset the bee to not be grabbed on in tapping script
            beeFree.GetComponent<BeeTap>().grabbed_on = false;
            free_contact_cell = false;
            // identify the pellet as semi-processed so that we can't pick it up from the right
            pellet_.GetComponent<PelletScript>().advanced_pellet = true;

            // reset the bee color once they've dropped the pellet
            beeFree.GetComponent<SpriteRenderer>().color = Color.red;

            // free bee can't move again until pellet has been fully worked
            if (!pellet_.GetComponent<PelletScript>().worked)
                beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
        }

    }

    // where does this get called in?
    public void resetParameters()
    {
        //contact_on = false;
        free_contact_cell = false;
        restrict_contact_cell = false;
        Debug.Log("workCellScript contact set to FALSE");
        my_colliders["bee_free"] = false;
        my_colliders["bee_restricted"] = false;
        carrying_pellet["bee_free"] = false;
        carrying_pellet["bee_restricted"] = false;

    }
}
