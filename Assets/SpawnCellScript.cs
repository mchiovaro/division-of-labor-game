using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCellScript : MonoBehaviour
{

    public bool contact_on = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        contact_on = true;
     //   Debug.Log(" pellet spawned inside ");

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        contact_on = false;
      //  Debug.Log(" pellet gone");
    }
}
