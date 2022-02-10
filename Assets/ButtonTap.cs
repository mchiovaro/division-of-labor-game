using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// <exclude />
public class ButtonTap : MonoBehaviour
{

    // add reference to Exit_app_script to grab td and ie conditions
    Exit_app_script exitappscript;

    // read in td_condition from Exit_app_script.cs
    private List<int> td_cond = new List<int>();

    // create a variables
    private int ParticipantNumber, round_num, com_cond, ie_cond, size_cond;

    // create text holder from buttonlog
    public String text;

    // locate message boxes
    public GameObject message_free, message_restrict;

    // create strings for holding the sets of button names
    List<string> freebuttons = new List<string> { "button_free_1", "button_free_2", "button_free_3", "button_free_4"};
    List<string> restrictbuttons = new List<string> { "button_restrict_1", "button_restrict_2", "button_restrict_3", "button_restrict_4"};

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

    //  Debug.Log("BUTTONTAP SCRIPT ParticipantNumber = " + ParticipantNumber
    //            + " round_num = " + round_num
    //            + " com_cond = " + com_cond
    //            + " ie_cond = " + ie_cond
    //            + " size_cond = " + size_cond);


      // find the text boxes
      message_free = GameObject.FindGameObjectWithTag("messagebox_free");
      message_restrict = GameObject.FindGameObjectWithTag("messagebox_restrict");

      // set text to blank
      message_free.GetComponent<TMP_Text>().text = "";
      message_restrict.GetComponent<TMP_Text>().text = "";

    }

    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += tappedHandler;
    }

    private void OnDisable()
    {
        GetComponent<TapGesture>().Tapped -= tappedHandler;
    }

    //this happens when the pellet is tapped
    private void tappedHandler(object sender, EventArgs e)
    {

      // save to disk
      saveToDisk();

      // send message to other player
      sendMessage();

    }

    // save the button data to file
    void saveToDisk()
    {

      string buttonname = transform.name;

      // set up saving location
      string path = "Assets/Resources/Data/" + ParticipantNumber + "-buttonlog.csv";

      // create writer
      StreamWriter writer = new StreamWriter(path, true);

      // create the string when button is pressed logging time and participant position data
      string next_line = System.DateTime.Now + ","
                        + Time.time + ","
                        + ParticipantNumber + ","
                        + round_num + ","
                        + td_cond[round_num] + ","
                        + buttonname;

      // write the line
      writer.WriteLine(next_line);

      // close the writer
      writer.Close();

    }

    void sendMessage()
    {
      // grab text from the button
      text =  transform.GetChild(0).GetComponent<TMP_Text>().text;

      // if it's a free button
      if(freebuttons.Contains(transform.name)){
        // update the text
        message_free.GetComponent<TMP_Text>().text = text;
        // set wait time
        StartCoroutine(waiter());
      }

      // if it's a restrict button
      if(restrictbuttons.Contains(transform.name)){
        // update the text
        message_restrict.GetComponent<TMP_Text>().text = text;
        // set wait time
        StartCoroutine(waiter());
      }


    }

    IEnumerator waiter()
    {
      // if it's a free button
      if(freebuttons.Contains(transform.name)){
        // wait for three seconds then turn the message off
        yield return new WaitForSeconds(3);
        message_free.GetComponent<TMP_Text>().text = "";
      }

      // if it's a restrict button
      if(restrictbuttons.Contains(transform.name)){
        // wait for three seconds then turn the message off
        yield return new WaitForSeconds(3);
        message_restrict.GetComponent<TMP_Text>().text = "";
      }

    }

}
