using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Exit_app_script : MonoBehaviour
{
    float ScToWRatio; // some camera ratio?
    GameObject work_cells, drop_cells, spawn_cells;
    public GameObject foodPrefab, beeFree, beeRestrict, buttons, messageboxes;

    // input parameters
    public int ParticipantNumber; // pulls from the excel sheet with conditions
    public string RA_initials;

    // max number of pellets based on number of cells available
    private int MAX_PELLETS = 8;

    // initialize conditions variables
    public List<int> td_condition = new List<int>(7);
    public int ie_condition;
    public int com_condition;
    public int size_condition;
    public int experiment_num;

    // ending experiment condition
    private const int CONDITION_EXPERIMENT_OVER = 0;

    // conditions for task demand
    public const int PRACTICE = 0;
    public const int FREE_HARD = 1;
    public const int RESTRICT_HARD = 2;
    public const int BOTH_HARD = 3;

    // condition for individual effectivities
    public const int FREE_PASS = 1;
    public const int RESTRICT_PASS = 2;

    // condition for communication
    public const int TALK_SILENCE = 1;
    public const int TALK_BUTTONS = 2;
    public const int TALK_FREE = 3;

    // initial conditions
    public int round_number = 0;
    public bool practice_mode = true; // true to include the practice trial
    public bool explain_mode = true; // true to include the practice trial

    // allows for the rate of spawn to be changed in condition checkCondition()
    private List<float> spawn_rate = new List<float>(new float[20]);

    // counter for number of dropped pellets
    private List<GameObject> dropped_pellets = new List<GameObject>();
    public GameObject[] pels;

    // trial running, spawn rate, and frame variables
    public bool running_ = false;
    private float next_spawn = 0.0f;
    private int current_spawn = 0;

    // set up for inbetween trials
    public Text ui_text;
    public RawImage ui_image;

    // data structures for position and time
    public struct SaveData
    {
        public List<Vector3> allPositions;
        public List<float> allTimeStamps;
    }

    // data structures for pellet events and frames
    public struct PelletData
    {
        public List<string> pelletEvent;
        public List<int> frameNum;
    }

    public List<PelletData> pellData = new List<PelletData>();

    // variables for data structures
    public SaveData data_beeFree, data_beeRestrict;
    //private bool quitting = false;

    // old time variable to reset time after each trial
    //private float timeIni = 0;

    // Start is called before the first frame update
    void Start()
    {
        // create list/variable for saving pellet data
        for (int ii = 0; ii < 20; ii++)
        {
            //
            PelletData pD;
            pD.pelletEvent = new List<string>();
            pD.frameNum = new List<int>();

            pellData.Add(pD); //
        }

        // set spawn rate to every 2 seconds
        for (int ii = 0; ii < 20; ii++)
            spawn_rate[ii] = 2.0f;

        // set up parameters and conditions
        loadParams();
        //checkCondition();

        // set up practice screen
        ui_image.enabled = true;
        ui_text.text = "Red player: Condition " + td_condition[round_number].ToString() + ".\n Ready to practice?";

        //allocate memory for Data to save
        data_beeFree.allPositions = new List<Vector3>(100000); // old bee
        data_beeFree.allTimeStamps = new List<float>(100000); // old bee
        data_beeRestrict.allPositions = new List<Vector3>(100000); // young bee
        data_beeRestrict.allTimeStamps = new List<float>(100000); // young bee

        //find all the environment gameObjects and assign them to variables
        work_cells = GameObject.FindGameObjectWithTag("work_row_middle");
        drop_cells = GameObject.FindGameObjectWithTag("drop_cells");
        spawn_cells = GameObject.FindGameObjectWithTag("spawn_cells");
        beeFree = GameObject.FindGameObjectWithTag("bee_free");
        beeRestrict = GameObject.FindGameObjectWithTag("bee_restricted");

        // find buttons and message message boxes
        buttons = GameObject.FindGameObjectWithTag("buttons");
        buttons.SetActive(false);
        messageboxes = GameObject.FindGameObjectWithTag("messagebox");
        messageboxes.SetActive(false);

        // set camera ratio
        ScToWRatio = 1080 / (2 * Camera.main.orthographicSize);

        // setting up camera ratio of the field
        float ratio = 108 / ScToWRatio - Camera.main.orthographicSize + 108 / (2 * ScToWRatio);

        // check to see what the communication condition is and create communication buttons if condition 2
        if(com_condition == 2){
          buttons.SetActive(true);
          messageboxes.SetActive(true);
        }

    }

    // Update() is called once per frame
    void Update()
    {

        //float timestamp = Time.time - timeIni; // brings timestamp back to zero before each trial
        float timestamp = Time.time; // keeping the time relative to the start of the game, not reseting at each trial

        // allow for escape quitting
        if (Input.GetKey("escape")) Application.Quit();

        // if we are currently in a trial
        if (running_)
        {

            // If it's time to spawn the next pellet and there are still pellets to be spawned // spawn_rate.Count is MAX_PELLETS
            if (Time.time > next_spawn && current_spawn < MAX_PELLETS)
            {
                Debug.Log("RUNNING");
                Debug.Log("Time.time" + Time.time + " > next_spawn " + next_spawn + "&& current_spawn " + current_spawn + " < spawn_rate.Count" + spawn_rate.Count);
                List<int> avail_spawn = new List<int>();

                for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
                {
                    if (!spawn_cells.transform.GetChild(ii).GetComponent<SpawnCellScript>().contact_on) {
                      // make a list of availble pellets to spawn
                        avail_spawn.Add(ii);
                        Debug.Log("avail_spawn = " + avail_spawn[ii]);
                      }
                }

                // if there are more pellets, keep spawning
                if (avail_spawn.Count > 0)
                {
                    next_spawn = Time.time + spawn_rate[current_spawn];
                    current_spawn++;

                    int rand_ini = Random.Range(0, avail_spawn.Count);

                    // spawn a pellet prefab
                    // find all pellets on the field
                    pels = GameObject.FindGameObjectsWithTag("pellet");
                    Debug.Log("pels.Length = " + pels.Length);

                      int counter = 0;
                      bool spawned = false;

                      // if this isn't the first spawn
                      if(pels.Length > 0){

                        // might just need this stuff
                        while (spawned == false) {
                              //Debug.Log("hit while loop for spawned = false");
                              // randomly gives a number: 1-10
                              int rand_int = Random.Range(0, avail_spawn.Count);

                              // for each pellet currently on the field
                              for (int ii = 0; ii < pels.Length; ii++){

                                //Debug.Log(ii + "pellet position = " + pels[ii].transform.position + ". projected position = " + spawn_cells.transform.GetChild(avail_spawn[rand_int]).position);
                                // if the projected spawn location is not field with this pellet, add one to the count
                                if (!(spawn_cells.transform.GetChild(avail_spawn[rand_int]).position == pels[ii].transform.position)){

                                  counter++;
                                  Debug.Log("counter = " + counter + ".  " + ii + " pellet position = " + pels[ii].transform.position + ". projected position = " + spawn_cells.transform.GetChild(avail_spawn[rand_int]).position);

                                  // if we have
                                  if (counter == pels.Length){
                                    GameObject pel_ = Instantiate(foodPrefab, spawn_cells.transform.GetChild(avail_spawn[rand_int]).position, Quaternion.identity);
                                    pel_.GetComponent<PelletScript>().pelletID = current_spawn - 1;
                                    spawned = true;
                                    counter = 0;
                                    pels = GameObject.FindGameObjectsWithTag("pellet");
                                    Debug.Log("SPAWNED " + pels.Length + " in " + spawn_cells.transform.GetChild(avail_spawn[rand_int]).position);

                                  }
                                }
                              }
                              counter = 0;
                        } // while

                      } // pellet.Length > 0

                      else if(pels.Length == 0) {
                        int rand_int = Random.Range(0, avail_spawn.Count);

                        GameObject pel_ = Instantiate(foodPrefab, spawn_cells.transform.GetChild(avail_spawn[rand_int]).position, Quaternion.identity);
                        pel_.GetComponent<PelletScript>().pelletID = current_spawn - 1;
                        Debug.Log("FIRST SPAWN");
                      }

                      // reset to false for the next spawn
                      //location_free = false;

                } // avail spawn

            } // time/time


            // grab timstamp for each player and add to allTimeStamps (x, y coord)
            data_beeFree.allTimeStamps.Add(timestamp);
            data_beeRestrict.allTimeStamps.Add(timestamp);

            // grab position for each player and add to allTimeStamps (x, y coord)
            data_beeFree.allPositions.Add(beeFree.transform.position);
            data_beeRestrict.allPositions.Add(beeRestrict.transform.position);

        } // running

        // if this isn't the last round (if this isn't trial 7 (because we added a 0 at the end to prompt the ending screen in checkCondition()))
        else if (!(round_number == td_condition.Count - 1))
        {
            // if space bar is pressed between trials
            if (Input.GetKey("space"))
            {
                // remove the waiting screen and text
                ui_image.enabled = false;
                ui_text.text = "";

                // allow for touch behaviors
                beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;
                beeRestrict.GetComponent<TouchScript.Behaviors.Transformer>().enabled = true;

                // run the game
                running_ = true;

            }

        }

        else //allow for quitting
        {
            // allow for quitting with escape key
            if (Input.GetKey("escape")) Application.Quit();

        }

    } // update

    // fixed rate for saving position data (50 samples per second)
    private void FixedUpdate()
    {
        // save data only for real trials and only while running
        if (running_ && practice_mode == false)
        {
            // set up saving location
            string filetitle = "" + ParticipantNumber;
            string path = "Assets/Resources/Data/" + filetitle + "-movementdata.csv";

            // create writer
            StreamWriter writer = new StreamWriter(path, true);

            // create the string with the participant position data
            string next_line = System.DateTime.Now + ","
                    + Time.time + ","
                    + ParticipantNumber + ","
                    + round_number + ","
                    + ie_condition + ","
                    + td_condition[round_number] + ","
                    + com_condition + ","
                    + size_condition + ","
                    + experiment_num + ","
                    + beeFree.transform.position.x + ","
                    + beeFree.transform.position.y + ","
                    + beeRestrict.transform.position.x + ","
                    + beeRestrict.transform.position.y;

            // write the line
            writer.WriteLine(next_line);

            // close the writer
            writer.Close();
        }
    }

    // adding pellet data
    public void addPellet(GameObject pell_)
    {

        dropped_pellets.Add(pell_);

        // if there are no more pellets
        if (dropped_pellets.Count == MAX_PELLETS)
        {
            running_ = false;

            // clear pellet data
            dropped_pellets.Clear();

            // find and destroy all pellets
            GameObject[] allPellets = GameObject.FindGameObjectsWithTag("pellet");
            foreach (GameObject ggoo in allPellets) Destroy(ggoo);

            // don't let the players move anymore
            beeFree.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;
            beeRestrict.GetComponent<TouchScript.Behaviors.Transformer>().enabled = false;

            // reset positions of players
            beeFree.transform.position = new Vector3(-3.25f, 0, 0);
            beeRestrict.transform.position = new Vector3(3.25f, 0, 0);
            // reset variables
            current_spawn = 0;
            next_spawn = 0;


            for (int ii = 0; ii < drop_cells.transform.childCount; ii++)
            {
                drop_cells.transform.GetChild(ii).GetComponent<DropCellScript>().dropTapCounter = 0;
                drop_cells.transform.GetChild(ii).GetComponent<DropCellScript>().pelletCounter = 0;
                Debug.Log("RESETTING + pelletCounter = " + drop_cells.transform.GetChild(ii).GetComponent<DropCellScript>().pelletCounter);
            }

            for (int ii = 0; ii < work_cells.transform.childCount; ii++)
            {
                work_cells.transform.GetChild(ii).GetComponent<workCellScript>().resetParameters();
            }

            for (int ii = 0; ii < spawn_cells.transform.childCount; ii++)
            {
                spawn_cells.transform.GetChild(ii).GetComponent<SpawnCellScript>().contact_on = false;
            }

            // if this was the explanation round (7 pellets), set to explain mode to false and continue with full practice round
            if (explain_mode){

                explain_mode = false;
                MAX_PELLETS = 20; // set to full pellet count

                // set up practice screen
                ui_image.enabled = true;
                ui_text.text = "Red player: Condition " + td_condition[round_number].ToString() + ".\n Ready to practice?";

            }

            // if this was the practice round, set to false
            else if (practice_mode && !explain_mode){
              //MAX_PELLETS = 20; // Set to full pellet count
              practice_mode = false;
              round_number++;
              checkCondition(); // set waiting screen
            }

            // if it wasn't the practice round, play again
            else if(!practice_mode){
              round_number++;
              checkCondition(); // set waiting screen
            }



            // clear pellet data
            for (int ii = 0; ii < MAX_PELLETS; ii++)
            {
                pellData[ii].pelletEvent.Clear();
                pellData[ii].frameNum.Clear();
            }

        }

    }

    // in between trials, set message screen for that td_condition
    void checkCondition()
    {
        // if we aren't done with the trials, ask if they are ready for the next one
        switch (td_condition[round_number])
        {
            // if it's free > restrict, tell them their tapping order
            case FREE_HARD:

                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Red player: Condition " + FREE_HARD.ToString() + ".\n Ready for next round?";

                break;

            // if it's free < restrict, tell them their tapping order
            case RESTRICT_HARD:

                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Red player: Condition " + RESTRICT_HARD.ToString() + ".\n Ready for next round?";

                break;

            // if it's free = restrict, tell them their tapping order
            case BOTH_HARD:

                ui_image.enabled = true;
                ui_text.text = "Round Finished. \n Red player: Condition " + BOTH_HARD.ToString() + ".\n Ready for next round?";

                break;

            // end screen when done with trials
            case CONDITION_EXPERIMENT_OVER:

                ui_image.enabled = true;
                ui_text.text = "Game Over. \n Thanks for participating!";

                break;

            default:
                break;
        }

    }

    // grabbing randomization of condition types for trials
    void loadParams()
    {

        // read in condition orders
        TextAsset iniFile = Resources.Load<TextAsset>("loadParams/conditions");

        // create string to split out rows of conditions
        string[] row = iniFile.text.Split(new char[] { '\n' });

        // create string to split columns of conditions for the row
        string[] col = row[ParticipantNumber].Split(new char[] { ',' });

        // td_condition

        // create temp variable to hold the practice condition
        int temp_td;

        // parse apart conditions from file going down column 1
        int.TryParse(col[10], out temp_td);

        // add the condition to a string
        td_condition.Add(temp_td);

        // increase by 1 until we hit six (FIX: make this 7, one for trial round)
        for (int ii = 2; ii < 8; ii++)
        {
            // parse apart conditions from file going down column 1
            int.TryParse(col[ii], out temp_td);

            // add the condition to a string
            td_condition.Add(temp_td);
        }

        //add the stopping condition at the end
        td_condition.Add(CONDITION_EXPERIMENT_OVER);

        // ie_condition

        // create temp variable to hold the condition
        int temp_ie;

        // parse apart condition from file column 1
        int.TryParse(col[1], out temp_ie);
        ie_condition = temp_ie;

        // com_condition

        // create temp variable to hold the condition
        int temp_com;

        // parse apart condition from file column 8
        int.TryParse(col[8], out temp_com);
        com_condition = temp_com;

        // size_condition: number of participants

        // create temp variable to hold the condition
        int temp_size;

        // parse apart condition from file column 9
        int.TryParse(col[9], out temp_size);
        size_condition = temp_size;

        // experiment_num

        // create temp variable to hold the condition
        int temp_exp;

        // parse apart condition from file column 9
        int.TryParse(col[10], out temp_exp);
        experiment_num = temp_exp;

    }

}
