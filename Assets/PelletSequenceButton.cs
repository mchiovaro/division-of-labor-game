/*
This script is for the "harder" tapping sequence, where the player must tap in
a pattern of small shapes nested in the pellet.
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;
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

    private void Start()
    {
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
      transform.parent.parent.GetComponent<PelletScript>().addToSequence(myButton, GetComponent<SpriteRenderer>());

      //}

    }
}
