using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DatabaseManager : MonoBehaviour
{
    public DataObject data_object;
    
    // Start is called before the first frame update
    void Start()
    {
        IDbConnection dbConnection = Create_And_Open_Database();

        dbConnection.Close();
    }

    // Update is called once per frame
    void Update()
    {
        Add_Data_To_Database();
    }



    private IDbConnection Create_And_Open_Database()
    {
        string db_Uri = "URI=file:Database.sqlite";
        IDbConnection dbConnection = new SqliteConnection(db_Uri);
        dbConnection.Open();

        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS Population_Table(Population_id INTEGER PRIMARY KEY, population_count INTEGER, healthy_count INTEGER, infected_count INTEGER, global_speed FLOAT)";
        dbCommandCreateTable.ExecuteReader();

        Debug.Log("Database Connected");

        return dbConnection;
    }

    public void Add_Data_To_Database()
    {
        IDbConnection dbConnection = Create_And_Open_Database();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "INSERT OR REPLACE INTO Population_Table (Population_id, population_count, healthy_count, infected_count, global_speed) VALUES (0," + data_object.population_count + "," + data_object.num_of_healthy + "," + data_object.num_of_infected + "," + data_object.global_speed + ")";
        dbCommandInsertValue.ExecuteNonQuery();

        dbConnection.Close();
    }


}
