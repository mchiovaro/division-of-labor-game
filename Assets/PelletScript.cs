/*
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
    AudioSource audioData;
    public  AudioClip [] aClips = new AudioClip [4];

    public GameObject pellet_holder;

    public int pelletID;
    public bool contact_on = false;
    bool grabbed_on = false;
    public bool pellet_in_workCell = false;
    public bool worked = false;
    public Collider2D player_coll;

    private GameObject beeFree, beeRestricted;

    public Dictionary<string, bool> my_colliders = new Dictionary<string, bool>() { {"bee_free",false}, {"bee_restricted",false}};

    Collider2D[] work_cell;

    private float process_rate = 1.0f/15;

    float scale_size = 0;
    private Color pellet_color = new Color(1, 1, 1);

    private Transform yellow_disk;

    private List<GameObject> seqButtonsGO = new List<GameObject>();

    public List<int> SeqButtons = new List<int>();

    private int[] CORRECT_SEQUENCE = { 1, 2, 3};

    private int current_seq = 0;

    private void Awake()
    {

        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestricted = GameObject.FindGameObjectWithTag("bee_restricted");

        audioData = GetComponent<AudioSource>();

        Transform buttonGroup = new GameObject().transform;
        foreach(Transform tt in transform)
            if (tt.tag.Equals("button_group")) buttonGroup = tt;

        foreach(Transform tt in buttonGroup)
        {
            seqButtonsGO.Add(tt.gameObject);
            tt.gameObject.SetActive(false);
            SeqButtons.Add(0);
        }

        work_cell = GameObject.FindGameObjectWithTag("work_row_middle").GetComponentsInChildren<Collider2D>();
        
        yellow_disk = transform.GetChild(0);
        
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
     //   my_colliders[collision.collider.tag] = true;

      //  player_coll = collision.collider;

     //   contact_on = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
       // my_colliders[collision.collider.tag] = false;

      //  contact_on = false;
      //  Debug.Log(" from collision " + collision.collider.tag + " = " + my_colliders[collision.collider.tag]);
    }

       private void OnTriggerEnter2D(Collider2D collider_)
    {

        Debug.Log(collider_.gameObject.tag);
        if (collider_.tag.Equals("bee_free")) my_colliders["bee_free"] = true;
        if (collider_.tag.Equals("bee_restricted")) my_colliders["bee_restricted"] = true;



          
      //  Debug.Log("pell " + pelletID + " enter free = " + my_colliders["bee_free"] + "  restrict = " + my_colliders["bee_restricted"]);

        //  my_colliders[collider_.tag] = true;
        // Debug.Log(" enter " + collider_.tag + " = " + my_colliders[collider_.tag]);
        if (!collider_.gameObject.tag.Equals("work_cell"))
            player_coll = collider_;

        contact_on = true;

    //    Debug.Log(" coll " + collider_.tag + " = " + my_colliders[collider_.tag]);
    }

    private void OnTriggerExit2D(Collider2D collider_)
    {
        if (collider_.tag.Equals("bee_free")) my_colliders["bee_free"] = false;
        if (collider_.tag.Equals("bee_restricted")) my_colliders["bee_restricted"] = false;

        Debug.Log("pell " + pelletID + " enter free = " + my_colliders["bee_free"] + "  restrict = " + my_colliders["bee_restricted"]);

        //   my_colliders[collider_.tag] = false;

        //    Debug.Log(" exit " + collider_.tag + " = " + my_colliders[collider_.tag]);

        if (!my_colliders["bee_free"] && !my_colliders["bee_restricted"])
            contact_on = false;

        if (!my_colliders["bee_free"] && my_colliders["bee_restricted"]) player_coll = beeRestricted.GetComponent<Collider2D>();
        if (my_colliders["bee_free"] && !my_colliders["bee_restricted"]) player_coll = beeFree.GetComponent<Collider2D>();
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
        if (pellet_in_workCell && contact_on)
        {

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

                    worked = true;

                    Destroy(yellow_disk.gameObject);
                    GetComponent<SpriteRenderer>().color = new Color(1,1,0);

                    beeRestricted.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;

                    foreach (GameObject ggoo in seqButtonsGO) ggoo.SetActive(true);
                    //add 4 colored points for sequence

                }
            }
        }
    }

    public void addToSequence(int addendum, SpriteRenderer sp)
    {
        audioData.clip = aClips[addendum - 1];
        audioData.Play();

        SeqButtons.RemoveAt(0);
        SeqButtons.Add(addendum);

        bool seqCorresponds = true;

        if(current_seq < CORRECT_SEQUENCE.Length)
        {
            if (addendum == CORRECT_SEQUENCE[current_seq])
            {
                current_seq++;
                sp.enabled = false;
            }else
            {
                current_seq = 0;
                seqCorresponds = false;
                foreach (GameObject ggoo in seqButtonsGO) ggoo.GetComponent<SpriteRenderer>().enabled = true;
            }

        }

        if (seqCorresponds && current_seq == CORRECT_SEQUENCE.Length)
        {

            saveToBuffer("GR_WORK");

            GameObject.Destroy(seqButtonsGO[0].transform.parent.gameObject);

            GameObject next_holder = new GameObject();

            Debug.Log("decision free " + my_colliders["bee_free"] + " restrict " + my_colliders["bee_restricted"]);

            if (my_colliders["bee_free"])
            {
                next_holder = beeFree;
            }else if(my_colliders["bee_restricted"])
            {
                next_holder = beeRestricted;
            }

            beeFree.GetComponent<BeeTap>().grabbed_on = true;

            transform.SetParent(beeFree.transform);

            //  disable pellet collider
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;

            transform.position = beeFree.transform.position;

            beeFree.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

        }

    }

    public void saveToBuffer(string stringus)
    {
        Exit_app_script e_a_s = Camera.main.GetComponent<Exit_app_script>();

        e_a_s.pellData[pelletID].pelletEvent.Add("X_" + transform.position.x + "_Y_" + transform.position.y + "_" + stringus);
        e_a_s.pellData[pelletID].frameNum.Add(e_a_s.current_frame - 1);

    }
}
