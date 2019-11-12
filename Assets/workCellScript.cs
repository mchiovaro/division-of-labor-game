

using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;


/// <exclude />
public class workCellScript : MonoBehaviour
{
    bool contact_on = false;
    Collider2D player_coll;


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
        player_coll = collision.collider;
        Debug.Log(" bee in contact ");


        contact_on = true;

    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        contact_on = true;
        player_coll = other;
        if (player_coll.transform.childCount == 0)
        {
            Debug.Log(" not carrying food");
        }
        else
        {
            Debug.Log(" yes carrying food");
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        contact_on = false;
		Debug.Log(" contact off");
    }

    //this happens when the pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {

        Debug.Log(" work cell tapped ");

		if(contact_on)
		{
			if(player_coll.transform.childCount != 0)
			{

				Debug.Log(player_coll.transform.gameObject.tag);

				GameObject pellet_ = player_coll.transform.GetChild(0).gameObject;
				pellet_.transform.SetParent(null);
				pellet_.transform.position = transform.position;
				pellet_.GetComponent<Collider2D>().enabled = true;

				if(player_coll.tag.Equals("bee_free"))
				{
				player_coll.GetComponent<SpriteRenderer>().color = Color.magenta;
				}else
				{
				player_coll.GetComponent<SpriteRenderer>().color = Color.cyan;
				}


			}
		}

    }
}
