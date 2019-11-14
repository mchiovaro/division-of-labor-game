

using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;


/// <exclude />
public class DropCellScript : MonoBehaviour
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
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        contact_on = false;
    }

    //this happens when the pellet is tapped
    private void tappedHandler2(object sender, EventArgs eventArgs)
    {

        if (contact_on)
        {
            if (player_coll.transform.childCount != 0)
            {

                Debug.Log(player_coll.transform.gameObject.tag);

                Debug.Log("dropped pellet");

                GameObject pellet_ = player_coll.transform.GetChild(0).gameObject;
                pellet_.transform.SetParent(null);
                pellet_.transform.position = transform.position;
                pellet_.GetComponent<Collider2D>().enabled = true;
                pellet_.GetComponent<Rigidbody2D>().isKinematic = true;
              //  pellet_.GetComponent<Collider2D>().isTrigger = true;
                player_coll.GetComponent<BeeTap>().grabbed_on = false;

                if (player_coll.tag.Equals("bee_free"))
                {
                    player_coll.GetComponent<SpriteRenderer>().color = Color.magenta;
                }
                else
                {
                    player_coll.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
            }
        }

    }
}
