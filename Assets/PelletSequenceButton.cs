/*
This script is for the "harder" tapping sequence, where the player must tap in
a pattern of small shapes nested in the pellet.
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <exclude />
public class PelletSequenceButton : MonoBehaviour
{

    private const int button_triangle = 1;
    private const int button_square = 2;
    private const int button_circle = 3;
    private int myButton = 0;

    public bool free_contact_on;
    public bool restrict_contact_on;

    // add reference to Exit_app_script to grab ie_condition
    //Exit_app_script exitappscript;

    // read in td_condition from Exit_app_script.cs
    //private List<int> td_cond = new List<int>();

    private void Start()
    {

      //exitappscript = Camera.main.GetComponent<Exit_app_script>();
      // get td_condition from Exit_app_script
      //td_cond = exitappscript.td_condition;
      //Debug.Log("td_cond = " + td_cond[0] + " " + td_cond[1] + " " + td_cond[2] + " " + td_cond[3] + " " + td_cond[4] + " " + td_cond[5]);
    }

    private void Awake()
    {
        if (transform.tag.Equals("button_triangle"))
            myButton = button_triangle;
        if (transform.tag.Equals("button_square"))
            myButton = button_square;
        if (transform.tag.Equals("button_circle"))
            myButton = button_circle;
    }

    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += tappedHandler2;
    }

    private void OnDisable()
    {
        GetComponent<TapGesture>().Tapped -= tappedHandler2;
    }

    //this happens when the pellet is tapped?
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {

      Debug.Log("tapped " + myButton);
      // put conditions in here for three separate sequences
      transform.parent.parent.GetComponent<PelletScript>().addToSequence(myButton, GetComponent<SpriteRenderer>());

      //}

    }
}
