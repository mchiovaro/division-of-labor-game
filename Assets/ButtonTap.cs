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
    private List<int> ie_cond = new List<int>();

    // create a variables
    private int ParticipantNumber, round_num, com_cond, size_cond, experiment_num;

    // create text holder from buttonlog
    public String text;

    // locate message boxes
    public GameObject message_free, message_free2, message_restrict, message_restrict2;

    // create strings for holding the sets of button names
    List<string> freebuttons = new List<string> { "button_free_1", "button_free_2", "button_free_3", "button_free_4", "button_free_5"};
    List<string> restrictbuttons = new List<string> { "button_restrict_1", "button_restrict_2", "button_restrict_3", "button_restrict_4", "button_restrict_5"};

    private void Start()
    {

      // get td_condition from Exit_app_script
      exitappscript = Camera.main.GetComponent<Exit_app_script>();
      td_cond = exitappscript.td_condition;
      ie_cond = exitappscript.ie_condition;

      // get the round number and other conditions from Exit_app_script
      ParticipantNumber = exitappscript.ParticipantNumber;
      //com_cond = Camera.main.GetComponent<Exit_app_script>().com_condition;
      //size_cond = Camera.main.GetComponent<Exit_app_script>().size_condition;
      //experiment_num = Camera.main.GetComponent<Exit_app_script>().experiment_num;
      round_num = Camera.main.GetComponent<Exit_app_script>().round_number;

      // find the text boxes
      message_free = GameObject.FindGameObjectWithTag("messagebox_free");
      message_restrict = GameObject.FindGameObjectWithTag("messagebox_restrict");
      message_free2 = GameObject.FindGameObjectWithTag("messagebox_free2");
      message_restrict2 = GameObject.FindGameObjectWithTag("messagebox_restrict2");

      // set text to blank
      message_free.GetComponent<TMP_Text>().text = "";
      message_restrict.GetComponent<TMP_Text>().text = "";
      message_free2.GetComponent<TMP_Text>().text = "";
      message_restrict2.GetComponent<TMP_Text>().text = "";

      if(round_num == 1){

        // set up saving location
        string path = "Assets/Resources/Data/" + ParticipantNumber + "-buttonlog.csv";

        // write header
        StreamWriter writer = new StreamWriter(path, true);
        string header_string = "System.DateTime.Now,Time.time,ParticipantNumber,round_number,ie_condition,td_condition,com_condition,size_condition,experiment_num,buttonname";
        writer.WriteLine(header_string); // write the line
        writer.Close(); // close the writer

      }

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
      // update variables for saving (if placed in start - last 3 conditions end up with 0s for buttons 1 and 5 for both players)
      round_num = Camera.main.GetComponent<Exit_app_script>().round_number;
      com_cond = Camera.main.GetComponent<Exit_app_script>().com_condition;
      size_cond = Camera.main.GetComponent<Exit_app_script>().size_condition;
      experiment_num = Camera.main.GetComponent<Exit_app_script>().experiment_num;

      //Debug.Log("round number = " + round_num);

      // save to disk if it isn't the practice round
      if(!(round_num == 0)) saveToDisk();

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

      //string header_string = "System.DateTime.Now,Time.time,ParticipantNumber,round_number,ie_condition,td_condition,com_condition,size_condition,experiment_num";

      // create the string when button is pressed logging time and participant position data
      string next_line = System.DateTime.Now + ","
                        + Time.time + ","
                        + ParticipantNumber + ","
                        + round_num + ","
                        + ie_cond[round_num] + ","
                        + td_cond[round_num] + ","
                        + com_cond + ","
                        + size_cond + ","
                        + experiment_num + ","
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
        message_free2.GetComponent<TMP_Text>().text = text;
        // set wait time
        StartCoroutine(waiter());
      }

      // if it's a restrict button
      if(restrictbuttons.Contains(transform.name)){
        // update the text
        message_restrict.GetComponent<TMP_Text>().text = text;
        message_restrict2.GetComponent<TMP_Text>().text = text;
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
        message_free2.GetComponent<TMP_Text>().text = "";
      }

      // if it's a restrict button
      if(restrictbuttons.Contains(transform.name)){
        // wait for three seconds then turn the message off
        yield return new WaitForSeconds(3);
        message_restrict.GetComponent<TMP_Text>().text = "";
        message_restrict2.GetComponent<TMP_Text>().text = "";
      }

    }

}
