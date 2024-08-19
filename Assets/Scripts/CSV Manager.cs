using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    public DataObject dataobject;

    public void update()
    {
        Add_Record_CSV("1", dataobject.num_of_healthy, dataobject.num_of_infected, dataobject.num_of_recovered, "C:\\Users\\willb\\Unity Projects\\Computer Science Coursework - Epidemic Spread Simulation\\Assets\\Results\\Results.txt");
    }

    public static void Add_Record_CSV(string frame_id,int number_of_healthy, int number_of_infected, int number_of_recovered,string file_path)
    {
        try
        {
            using (System.IO.StreamWriter csv_file = new System.IO.StreamWriter(@file_path, true))
            {
                csv_file.WriteLine(frame_id + "," + number_of_healthy.ToString() + "," + number_of_infected.ToString() + "," + number_of_recovered.ToString());
                Debug.Log(frame_id + "," + number_of_healthy.ToString() + "," + number_of_infected.ToString() + "," + number_of_recovered.ToString());
            }
        }
        catch
        {
            Debug.Log("Application did a little wrongen");
        }
    }
}
