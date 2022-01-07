/*
Script attached to pellets only
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;


/// <exclude />
public class PelletScript : MonoBehaviour
{
    //AudioSource audioData;
    //public  AudioClip [] aClips = new AudioClip [4];

    //public GameObject pellet_holder;

    public int pelletID;
    public bool free_contact_on = false;
    public bool restrict_contact_on = false;
    public bool advanced_pellet = false;
    //public int FROM_LEFT, FROM_RIGHT, from_where;
    private int left = 1, right = -1, from = 0;
    //public bool pellet_in_workCell = false;
    public bool pellet_in_spawnCell = false;
    public bool worked = false;
    public Collider2D player_coll;
  //  public bool free_grabbed_on = false;
  //  public bool restrict_grabbed_on = false;

    private GameObject beeFree, beeRestricted;

    public Dictionary<string, bool> my_colliders = new Dictionary<string, bool>() {{"bee_free",false}, {"bee_restricted",false}};

    Collider2D[] work_cell;
    Collider2D[] spawn_cell;

// TASK DEMAND VARIABLES

    // CONDITION: Change this number for the number of taps for bee free
    private float process_rate = 1.0f/2;

    // starting size of the yellow disk
    float scale_size = 0;
    //
    private Color pellet_color = new Color(1, 1, 1);

    // allows yellow disk to be scaled up
    private Transform yellow_disk;

    // CONDITION: Change for bee beeFree
    //correct tapping sequence on inner shapes (can add 3 more, 1,2,3 again)
    private int[] CORRECT_SEQUENCE = {1, 2, 3};

    // variable for recording the players current input sequence
    private int current_seq = 0;

    // for workcell pellets
    private List<GameObject> seqButtonsGO = new List<GameObject>();
    public List<int> SeqButtons = new List<int>();

    private void Awake()
    {

        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestricted = GameObject.FindGameObjectWithTag("bee_restricted");

        //audioData = GetComponent<AudioSource>();

        //
        Transform buttonGroup = new GameObject().transform;
        foreach(Transform tt in transform)
            if (tt.tag.Equals("button_group")) buttonGroup = tt;

        //
        foreach(Transform tt in buttonGroup)
        {
            seqButtonsGO.Add(tt.gameObject);
            tt.gameObject.SetActive(false);
            SeqButtons.Add(0);
        }

        work_cell = GameObject.FindGameObjectWithTag("work_row_middle").GetComponentsInChildren<Collider2D>();
        spawn_cell = GameObject.FindGameObjectWithTag("spawn_cell").GetComponentsInChildren<Collider2D>();

        //
        yellow_disk = transform.GetChild(0);

        // giving the yellow disk a scaling vector for tapping
        yellow_disk.localScale = new Vector3(scale_size, scale_size, 0);

    }

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


    private void OnCollisionEnter2D(Collision2D collider_)
    {
/*      // if the collided object is bee free, set contact true and give a debug message
      if (collider_.tag.Equals("bee_free")){
        my_colliders["bee_free"] = true;
        free_contact_on = true;
        Debug.Log(collider_.gameObject.tag + " contact on");
      }

      // if the collided object is bee free, set contact true and give a debug message
      if (collider_.tag.Equals("bee_restricted")){
        my_colliders["bee_restricted"] = true;
        restrict_contact_on = true;
        Debug.Log(collider_.gameObject.tag + " contact on");
      }
*/    }

    private void OnCollisionExit2D(Collision2D collider_)
    {
/*      // if the collider object that moved away was bee free, set contact false and give debug message
      if (collider_.tag.Equals("bee_free"))
      {
          my_colliders["bee_free"] = false;
          free_contact_on = false;
          Debug.Log(collider_.gameObject.tag + " contact off");
      }

      // if the collider object that moved away was bee restricted, set contact false and give debug message
      if (collider_.tag.Equals("bee_restricted"))
      {
        my_colliders["bee_restricted"] = false;
        restrict_contact_on = false;
        Debug.Log(collider_.gameObject.tag + " contact off");
      }
      //make make their collider be the pellet they are contacting
      if (!my_colliders["bee_free"] && my_colliders["bee_restricted"]) player_coll = beeRestricted.GetComponent<Collider2D>();
      if (my_colliders["bee_free"] && !my_colliders["bee_restricted"]) player_coll = beeFree.GetComponent<Collider2D>();
*/    }

    // Senses the contact and logs it
    private void OnTriggerEnter2D(Collider2D collider_)
    {

      // if the collided object is bee free, set contact true and give a debug message
      if (collider_.tag.Equals("bee_free")){
        my_colliders["bee_free"] = true;
        free_contact_on = true;
        Debug.Log(collider_.gameObject.tag + " contact on");
        if (collider_.transform.position.x < 0)
        {
            from = left;
            Debug.Log("coming from left");
        }
        else
        {
            from = right;
            Debug.Log("coming from right");
        }
      }

      // if the collided object is bee free, set contact true and give a debug message
      if (collider_.tag.Equals("bee_restricted")){
        my_colliders["bee_restricted"] = true;
        restrict_contact_on = true;
        Debug.Log(collider_.gameObject.tag + " contact on");
        if (collider_.transform.position.x < 0)
        {
            from = left;
            Debug.Log("coming from left");
        }
        else
        {
            from = right;
            Debug.Log("coming from right");
        }
      }

      //make make their collider be the pellet they are contacting
      //if (!my_colliders["bee_free"] && my_colliders["bee_restricted"]) player_coll = beeRestricted.GetComponent<Collider2D>();
      //if (my_colliders["bee_free"] && !my_colliders["bee_restricted"]) player_coll = beeFree.GetComponent<Collider2D>();
      //Debug.Log("player_coll = " + player_coll);

    }

    // when bee moves away
    private void OnTriggerExit2D(Collider2D collider_)
    {
        // if the collider object that moved away was bee free, set contact false and give debug message
        if (collider_.tag.Equals("bee_free"))
        {
            my_colliders["bee_free"] = false;
            free_contact_on = false;
            Debug.Log(collider_.gameObject.tag + " contact off");
        }

        // if the collider object that moved away was bee restricted, set contact false and give debug message
        if (collider_.tag.Equals("bee_restricted"))
        {
          my_colliders["bee_restricted"] = false;
          restrict_contact_on = false;
          Debug.Log(collider_.gameObject.tag + " contact off");

        }
    }

    // when pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {
      // if bee free is in contact and not holding a pellet
      if (free_contact_on == true && beeFree.GetComponent<BeeTap>().grabbed_on == false && advanced_pellet == false)
      {
          // If the yellow disk is not the size of the whole pellet
          if (scale_size < 1)
          {
              // If there are at least two  more taps the whole pellet is yellow
              if (scale_size + process_rate < 1)
              {
                  // adding size
                  scale_size += process_rate;

                  // updating the yellow disk size
                  yellow_disk.localScale = new Vector3(scale_size, scale_size, 0);
              }

              // if the pellet has been worked enough
              else
              {

                  scale_size = 2;
                  worked = true;

                  // remove the yellow disk
                  Destroy(yellow_disk.gameObject);

                  // change the disk back to all white? Of the player?
                  GetComponent<SpriteRenderer>().color = new Color(1,1,0);

                  // enable moving again?
                  beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;

                  // ? making the little shapes active?
                  foreach (GameObject ggoo in seqButtonsGO) ggoo.SetActive(true);

              }
          }

          // if the yellow fills the pellet
          if (scale_size > 1)
          {
                // note that free bee grabbed on to the advanced pellets
                beeFree.GetComponent<BeeTap>().grabbed_on = true;
                Debug.Log("FREE GRABBED ON");
                free_contact_on = false;
                Debug.Log("FREE CONTACT OFF");

                // set the parent to be bee free
                transform.SetParent(beeFree.transform);

                //  disable pellet collider
                GetComponent<Collider2D>().enabled = false;
                GetComponent<Rigidbody2D>().isKinematic = true;

                // move pellet onto the bee?
                transform.position = beeFree.transform.position;

                // put the pellet on it's back?
                beeFree.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

          }
      }

      // if bee restricted is in contact and they aren't holding anything
      if (restrict_contact_on == true && beeRestricted.GetComponent<BeeTap>().grabbed_on == false && advanced_pellet == false)
      {

          //Debug.Log("Restrict NOT grabbed on");
          // If the yellow disk is not the size of the whole pellet
          if (scale_size < 1)
          {
              // If there are at least two  more taps the whole pellet is yellow
              if (scale_size + process_rate < 1)
              {
                  // adding size
                  scale_size += process_rate;

                  // updating the yellow disk size
                  yellow_disk.localScale = new Vector3(scale_size, scale_size, 0);
              }

              // if the pellet has been worked enough
              else
              {
                  scale_size = 2;
                  worked = true;

                  // remove the yellow disk // is this not working??
                  Destroy(yellow_disk.gameObject);

                  // change the disk back to all white? Of the player?
                  GetComponent<SpriteRenderer>().color = new Color(1,1,0);

                  // enable moving again?
                  beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;

                  // making the little shapes active?
                  foreach (GameObject ggoo in seqButtonsGO) ggoo.SetActive(true);

                  // turn contact off for next pellet
                  restrict_contact_on = false;
              }
          }

          // if the yellow fills the pellet
          if (scale_size > 1)
          {

            // note that free bee grabbed on to the advanced pellets
            beeRestricted.GetComponent<BeeTap>().grabbed_on = true;
            Debug.Log("RESTRICT GRABBED ON");

            // set the parent to be bee free
            transform.SetParent(beeRestricted.transform);

            //  disable pellet collider
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;

            // move pellet onto the bee?
            transform.position = beeRestricted.transform.position;

            // put the pellet on it's back?
            beeRestricted.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

          }
      }
    }

    // this is called from the PelletSequenceButton.cs script;
    // creates the sequence of tapped shapes, tapped_shape is the button tag
    public void addToSequence(int tapped_shape, SpriteRenderer sp)
    {
        //audioData.clip = aClips[tapped_shape - 1];
        //audioData.Play();
        // if bee_free is touching the pellet and not grabbing anything //&& beeFree.GetComponent<BeeTap>().grabbed_on == false
        if(free_contact_on == true && restrict_contact_on == false){

          Debug.Log("addToSequence FREE CONTACT ON");
          //Debug.Log("my_colliders[bee_free] = " + my_colliders["bee_free"]);
          //Debug.Log("my_colliders[bee_restricted] = " + my_colliders["bee_restricted"]);
          //Debug.Log("tapped " + tapped_shape);
          SeqButtons.RemoveAt(0);
          SeqButtons.Add(tapped_shape);

          bool seqCorresponds = true;

          // if they haven't done the full sequence yet and they are approaching from the left
          if(current_seq < CORRECT_SEQUENCE.Length && from == left)
          {
              // if their taps were in the right order but they aren't done yet
              if (tapped_shape == CORRECT_SEQUENCE[current_seq])
              {
                  current_seq++;
                  // remove that button
                  sp.enabled = false;
              }

              // if they messed up the sequence
              else
              {
                  current_seq = 0;
                  seqCorresponds = false;
                  // put all buttons back
                  foreach (GameObject ggoo in seqButtonsGO) ggoo.GetComponent<SpriteRenderer>().enabled = true;
              }
          }

          // if the player does the right sequence
          if (seqCorresponds && current_seq == CORRECT_SEQUENCE.Length)
          {
              // record that bee free worked a pellet at that moment
              saveToBuffer("GR_WORK");

              // destroy the inner buttons
              GameObject.Destroy(seqButtonsGO[0].transform.parent.gameObject);

              // note that free bee grabbed on to the advanced pellet
              beeFree.GetComponent<BeeTap>().grabbed_on = true;

              // set bee free as parent and put pellet on it's back
              transform.SetParent(beeFree.transform);
              transform.position = beeFree.transform.position;
              beeFree.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

              //  disable pellet collider
              GetComponent<Collider2D>().enabled = false;
              GetComponent<Rigidbody2D>().isKinematic = true;
          }

        }

        // if bee_restricted is touching the pellet
        if(restrict_contact_on == true && free_contact_on == false){

          Debug.Log("addToSequence RESTRICT CONTACT ON");
          //Debug.Log("from_where = " + from_where);
          //Debug.Log("bee restricted is touching");
          //Debug.Log("tapped " + tapped_shape);
          SeqButtons.RemoveAt(0);
          SeqButtons.Add(tapped_shape);

          bool seqCorresponds = true;

          // if they haven't done the full sequence yet and they are approaching from the left
          if(current_seq < CORRECT_SEQUENCE.Length && from == left)
          {
              // if their taps were in the right order but they aren't done yet
              if (tapped_shape == CORRECT_SEQUENCE[current_seq])
              {
                  current_seq++;
                  // remove that button
                  sp.enabled = false;
              }

              // if they messed up the sequence
              else
              {
                  current_seq = 0;
                  seqCorresponds = false;
                  // put all buttons back
                  foreach (GameObject ggoo in seqButtonsGO) ggoo.GetComponent<SpriteRenderer>().enabled = true;
              }
          }

          // if the player does the right sequence
          if (seqCorresponds && current_seq == CORRECT_SEQUENCE.Length)
          {
              // record that bee restricted worked a pellet at that moment
              //saveToBuffer("GR_WORK");

              // destroy the inner buttons
              GameObject.Destroy(seqButtonsGO[0].transform.parent.gameObject);

              // note that restricted bee grabbed on to the advanced pellet
              beeRestricted.GetComponent<BeeTap>().grabbed_on = true;

              // set bee restricted as parent and put pellet on it's back
              transform.SetParent(beeRestricted.transform);
              transform.position = beeRestricted.transform.position;
              beeRestricted.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

              //  disable pellet collider
              GetComponent<Collider2D>().enabled = false;
              GetComponent<Rigidbody2D>().isKinematic = true;
          }

        }



    }

    public void saveToBuffer(string stringus)
    {
        Exit_app_script e_a_s = Camera.main.GetComponent<Exit_app_script>();

        // save the pellets position and frame
        e_a_s.pellData[pelletID].pelletEvent.Add("X_" + transform.position.x + "_Y_" + transform.position.y + "_" + stringus);
        e_a_s.pellData[pelletID].frameNum.Add(e_a_s.current_frame - 1);

    }
}
