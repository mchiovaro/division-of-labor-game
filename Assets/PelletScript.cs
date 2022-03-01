/*
Script attached to pellets only
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;


/// <exclude />
public class PelletScript : MonoBehaviour
{
    AudioSource audioData;
    public  AudioClip [] aClips = new AudioClip [4];

    //public GameObject pellet_holder;

    public int pelletID;
    public bool free_contact_on = false;
    public bool restrict_contact_on = false;
    public bool advanced_pellet = false;
    private int left = 1, right = -1, from = 0;
    public bool pellet_in_spawnCell = false;
    public bool worked = false;
    public Collider2D player_coll;
    // set grabber indicators for the different kinds of pellets
    private int grabber_advanced;
    private int grabber_base;

    private GameObject beeFree, beeRestricted;

    public Dictionary<string, bool> my_colliders = new Dictionary<string, bool>() {{"bee_free",false}, {"bee_restricted",false}};

    Collider2D[] work_cell;
    Collider2D[] spawn_cell;

    // CONDITION: Change this number for the number of taps for bee free
    private float process_rate = 1.0f/6;

    // starting size of the yellow disk
    float scale_size = 0;
    //
    private Color pellet_color = new Color(1, 1, 1);

    // allows yellow disk to be scaled up
    private Transform yellow_disk;

    //correct tapping sequence on inner shapes (can add 3 more, 1,2,3 again)
    private int[] CORRECT_SEQUENCE; //= {1, 2, 3};
    private int[] seq_1 = {1, 2, 3}; // same difficulty
    private int[] seq_2 = {3, 2, 1, 3, 2, 1}; // free harder
    private int[] seq_3 = {1, 1, 1}; // restrict harder

    // variable for recording the players current input sequence
    private int current_seq = 0;

    // for workcell pellets
    private List<GameObject> seqButtonsGO = new List<GameObject>();
    public List<int> SeqButtons = new List<int>();

    // add reference to Exit_app_script to grab td and ie conditions
    Exit_app_script exitappscript;

    // initialize conditions variables
    public List<int> td_cond = new List<int>(7);
    public int ParticipantNumber, round_num, ie_cond, com_cond, size_cond, experiment_num;

    private void Awake()
    {

        // locate objects
        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestricted = GameObject.FindGameObjectWithTag("bee_restricted");
        audioData = GetComponent<AudioSource>();

        //
        Transform buttonGroup = new GameObject().transform;
        foreach(Transform tt in transform)
            if (tt.tag.Equals("button_group")) buttonGroup = tt;

        //
        foreach(Transform tt in buttonGroup)
        {
            seqButtonsGO.Add(tt.gameObject);
            tt.gameObject.SetActive(false);
            //SeqButtons.Add(0);
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

      // get td_condition from Exit_app_script
      exitappscript = Camera.main.GetComponent<Exit_app_script>();
      td_cond = exitappscript.td_condition;

      // get the round number and other conditions from Exit_app_script
      ParticipantNumber = exitappscript.ParticipantNumber;
      round_num = Camera.main.GetComponent<Exit_app_script>().round_number;
      com_cond = Camera.main.GetComponent<Exit_app_script>().com_condition;
      ie_cond = Camera.main.GetComponent<Exit_app_script>().ie_condition;
      size_cond = Camera.main.GetComponent<Exit_app_script>().size_condition;

      // start a data file if this is the first round
      if(pelletID == 0 && round_num == 1) {

        string path = "Assets/Resources/Data/"+ ParticipantNumber + "-PelletData.csv";

        StreamWriter writer = new StreamWriter(path, true);

        string header_string = "System.DateTime.Now,Time.time,ParticipantNumber,round_number,ie_condition,td_condition,com_condition,size_condition,experiment_num,x_position,y_position,pelletID,event";

        writer.WriteLine(header_string);
        writer.Close();

      }

      // log the spawn
      saveToBuffer("SPAWN");

      // set the sequence based on td_cond for this round
      if(td_cond[round_num] == 1 || td_cond[round_num] == 4)CORRECT_SEQUENCE = seq_1;
      if(td_cond[round_num] == 2 || td_cond[round_num] == 5)CORRECT_SEQUENCE = seq_2;
      if(td_cond[round_num] == 3 || td_cond[round_num] == 6)CORRECT_SEQUENCE = seq_3;
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
    }

    private void OnCollisionExit2D(Collision2D collider_)
    {
    }

    // Senses the contact and logs it
    private void OnTriggerEnter2D(Collider2D collider_)
    {

      // if the collided object is bee free, set contact true and give a debug message
      if (collider_.tag.Equals("bee_free")){

        my_colliders["bee_free"] = true;
        free_contact_on = true;

        if (collider_.transform.position.x < 0)
        {
            from = left;
            //Debug.Log("coming from left");
            Debug.Log("FROM = " + from);
        }
        else
        {
            from = right;
            Debug.Log("FROM = " + from);
            //Debug.Log("coming from right");
        }
      }

      // if the collided object is bee free, set contact true and give a debug message
      if (collider_.tag.Equals("bee_restricted")){

        my_colliders["bee_restricted"] = true;
        restrict_contact_on = true;

        if (collider_.transform.position.x < 0)
        {
            from = left;
            //Debug.Log("coming from left");
        }
        else
        {
            from = right;
            //Debug.Log("coming from right");
        }
      }
    }

    // when bee moves away
    private void OnTriggerExit2D(Collider2D collider_)
    {
        // if the collider object that moved away was bee free, set contact false and give debug message
        if (collider_.tag.Equals("bee_free"))
        {
            my_colliders["bee_free"] = false;
            free_contact_on = false;
            //Debug.Log(collider_.gameObject.tag + " contact off");
        }

        // if the collider object that moved away was bee restricted, set contact false and give debug message
        if (collider_.tag.Equals("bee_restricted"))
        {
          my_colliders["bee_restricted"] = false;
          restrict_contact_on = false;
          //Debug.Log(collider_.gameObject.tag + " contact off");

        }
    }

    // when pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {

      // if bee free is in contact and not holding a pellet OR restrict in contact and not holding a pellet
      if ((free_contact_on == true && beeFree.GetComponent<BeeTap>().grabbed_on == false && advanced_pellet == false) || (restrict_contact_on == true && beeRestricted.GetComponent<BeeTap>().grabbed_on == false && advanced_pellet == false))
      {

          // identify who is trying to grab the pellet
          if(free_contact_on == true && restrict_contact_on == false) grabber_base = 1;
          if(restrict_contact_on == true && free_contact_on == false) grabber_base = 2;

          // If the yellow disk is not the size of the whole pellet
          if (scale_size < 1)
          {
              // log the tap
              if(grabber_base == 1)
              {
                saveToBuffer("1_TAPPED");
                beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
              }
              if(grabber_base == 2)
              {
                saveToBuffer("2_TAPPED");
                beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
              }

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

                  // making the little shapes active?
                  foreach (GameObject ggoo in seqButtonsGO) ggoo.SetActive(true);

                  // let free bee move again
                  if (grabber_base == 1){
                    // enable moving again
                    beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
                    // turn contact off for next pellet
                    free_contact_on = false;
                  }

                  // let restrict bee move again
                  if (grabber_base == 2){
                    // enable moving again
                    beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
                    // turn contact off for next pellet
                    restrict_contact_on = false;
                  }

              }
          }

          // if the yellow fills the pellet
          if (scale_size > 1)
          {

                //  disable pellet collider
                GetComponent<Collider2D>().enabled = false;
                GetComponent<Rigidbody2D>().isKinematic = true;

                if(grabber_base == 1){

                  // note that free bee grabbed on to the advanced pellets
                  beeFree.GetComponent<BeeTap>().grabbed_on = true;
                  //Debug.Log("FREE GRABBED ON");
                  // log the pickup
                  saveToBuffer("1_PICKUP");
                  free_contact_on = false;
                  //Debug.Log("FREE CONTACT OFF");

                  // set the parent to be bee free
                  transform.SetParent(beeFree.transform);
                  // move pellet onto the bee?
                  transform.position = beeFree.transform.position;

                  // put the pellet on it's back?
                  beeFree.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                }

                if(grabber_base == 2){

                  // note that restricted bee grabbed on to the advanced pellets
                  beeRestricted.GetComponent<BeeTap>().grabbed_on = true;
                  //Debug.Log("RESTRICT GRABBED ON");
                  // log the pickup
                  saveToBuffer("2_PICKUP");
                  restrict_contact_on = false;
                  //Debug.Log("RESTRICT CONTACT OFF");

                  // set the parent to be bee restrict
                  transform.SetParent(beeRestricted.transform);
                  // move pellet onto the bee?
                  transform.position = beeRestricted.transform.position;

                  // put the pellet on it's back?
                  beeRestricted.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                }

          }
      }

    }

    // this is called from the PelletSequenceButton.cs script;
    // creates the sequence of tapped shapes, tapped_shape is the button tag
    public void addToSequence(int tapped_shape, SpriteRenderer sp)
    {

        // if bee_free is touching the pellet and not grabbing anything
        if(((free_contact_on == true && restrict_contact_on == false && beeFree.GetComponent<BeeTap>().grabbed_on == false) || (restrict_contact_on == true && free_contact_on == false && beeRestricted.GetComponent<BeeTap>().grabbed_on == false)) && from == left)
        {

          //
          // identify who is trying to grab the pellet
          if(free_contact_on == true && restrict_contact_on == false)
          {
            grabber_advanced = 1;
            saveToBuffer("1_TAPPED_" + tapped_shape);
            beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;

          }

          if(restrict_contact_on == true && free_contact_on == false)
          {
            grabber_advanced = 2;
            saveToBuffer("2_TAPPED_" + tapped_shape);
            beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
          }

          bool seqCorresponds = true;

          // if they haven't done the full sequence yet and they are approaching from the left
          if(current_seq < CORRECT_SEQUENCE.Length && from == left)
          {
              // if their taps were in the right order but they aren't done yet
              if (tapped_shape == CORRECT_SEQUENCE[current_seq])
              {
                  //Debug.Log(tapped_shape);
                  current_seq++;

                  // play a tune to let them know they're on the right pattern
                  audioData.clip = aClips[0];
                  audioData.Play();
              }

              // if they messed up the sequence
              else
              {
                  // play a different tune to let them know
                  audioData.clip = aClips[3];
                  audioData.Play();
                  //Debug.Log()
                  current_seq = 0;
                  seqCorresponds = false;
                  // put all buttons back
                  foreach (GameObject ggoo in seqButtonsGO) ggoo.GetComponent<SpriteRenderer>().enabled = true;
              }
          }

          // if the player does the right sequence
          if (seqCorresponds && current_seq == CORRECT_SEQUENCE.Length)
          {

              // destroy the inner buttons
              GameObject.Destroy(seqButtonsGO[0].transform.parent.gameObject);

              // if it's bee free doing the work
              if(grabber_advanced == 1){

                // log the pick up
                saveToBuffer("1_PICKUP_MID");

                // note that free bee grabbed on to the advanced pellet
                beeFree.GetComponent<BeeTap>().grabbed_on = true;

                // set bee free as parent and put pellet on it's back
                transform.SetParent(beeFree.transform);
                transform.position = beeFree.transform.position;
                beeFree.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
              }

              // if it's bee restrict doing the work
              if(grabber_advanced == 2){

                // log the pick up
                saveToBuffer("2_PICKUP_MID");

                // note that free bee grabbed on to the advanced pellet
                beeRestricted.GetComponent<BeeTap>().grabbed_on = true;

                // set bee free as parent and put pellet on it's back
                transform.SetParent(beeRestricted.transform);
                transform.position = beeRestricted.transform.position;
                beeRestricted.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
              }

              //  disable pellet collider
              GetComponent<Collider2D>().enabled = false;
              GetComponent<Rigidbody2D>().isKinematic = true;
          }

        }

    }

    public void saveToBuffer(string stringus)
    {

      if(!(round_num == 0)) {

        string path = "Assets/Resources/Data/"+ ParticipantNumber + "-PelletData.csv";

        StreamWriter writer = new StreamWriter(path, true);

        string data = System.DateTime.Now + ","
                        + Time.time + ","
                        + ParticipantNumber + ","
                        + round_num + ","
                        + ie_cond + ","
                        + td_cond[round_num] + ","
                        + com_cond + ","
                        + size_cond + ","
                        + experiment_num + ","
                        + transform.position.x + ","
                        + transform.position.y + ","
                        + pelletID + ","
                        + stringus;

        writer.WriteLine(data);
        writer.Close();

      }

    }
}
