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
        IDbConnection database_connection = Create_And_Open_Database();
        Save_Population();
        Save_Disease();
        Save_Simulation();



        Debug.Log(Get_Next_index("Simulation_Table"));
        database_connection.Close();
    }

    // Update is called once per frame
    void Update()
    {
        Save_Simulation();
    }

    private IDbConnection Create_And_Open_Database()
    {
        string db_Uri = "URI=file:Database.sqlite";
        IDbConnection dbConnection = new SqliteConnection(db_Uri);
        dbConnection.Open();

        // Population Table
        IDbCommand population_table_command = dbConnection.CreateCommand();
        population_table_command.CommandText = "CREATE TABLE IF NOT EXISTS Population_Table(" +
            "population_id INTEGER NOT NULL," +
            "population_count INTEGER," +
            "population_name varchar(255)," +
            "global_speed FLOAT," +
            "number_of_sensors INTEGER," +
            "PRIMARY KEY (population_id AUTOINCREMENT))";
        population_table_command.ExecuteReader();


        // Disease Table
        IDbCommand disease_Table_command = dbConnection.CreateCommand();
        disease_Table_command.CommandText = "CREATE TABLE IF NOT EXISTS Disease_Table(" +
            "disease_id INTEGER NOT NULL," +
            "disease_name varchar(255)," +
            "transmission_radius FLOAT," +
            "min_infection_time FLOAT," +
            "max_infection_time FLOAT," +
            "min_recovering_time FLOAT," +
            "max_recovering_time FLOAT," +
            "is_reccuring BOOl," +
            "PRIMARY KEY (disease_id AUTOINCREMENT))";

        disease_Table_command.ExecuteReader();

        // Simulation Table
        IDbCommand simulation_table_command = dbConnection.CreateCommand();
        simulation_table_command.CommandText = "CREATE TABLE IF NOT EXISTS Simulation_Table(" +
            "simulation_id INTEGER," +
            "frame_id INTEGER," +
            "population_id INTEGER," +
            "disease_id INTEGER," +
            "num_of_healthy INTEGER," +
            "num_of_infected INTEGER," +
            "num_of_recovered INTEGER," +
            "FOREIGN KEY (population_id) REFERENCES Population_Table(population_id)," +
            "FOREIGN KEY (disease_id) REFERENCES Disease_Table(disease_id)," +
            "PRIMARY KEY (simulation_id, frame_id))";

        simulation_table_command.ExecuteReader();


        Debug.Log("Database Connected");
        return dbConnection;
    }

    public void Save_Population()
    {
        IDbConnection database_connection = Create_And_Open_Database();
        IDbCommand save_population = database_connection.CreateCommand();
        save_population.CommandText =
            "INSERT INTO Population_Table(" +
            "population_count, population_name," +
            "global_speed, number_of_sensors)" +

            "VALUES(" + 
            
            data_object.population_count + "," +
            data_object.population_name + "," +
            data_object.global_speed + "," +
            data_object.number_of_sensors + ")";

        save_population.ExecuteNonQuery();
        database_connection.Close();
    }

    public void Save_Disease()
    {
        IDbConnection database_connection = Create_And_Open_Database();
        IDbCommand save_disease = database_connection.CreateCommand();
        save_disease.CommandText =
            "INSERT INTO Disease_Table(" +
            "disease_name,transmission_radius," +
            "min_infection_time, max_infection_time," +
            "min_recovering_time, max_recovering_time," +
            "is_reccuring)" +

            "VALUES(" +
            
            data_object.disease_name + "," +
            data_object.radius + "," +
            data_object.infection_time.x + "," +
            data_object.infection_time.y + "," +
            data_object.recovering_time.x + "," +
            data_object.recovering_time.y + "," +
            data_object.is_reccuring + ")";

        save_disease.ExecuteNonQuery();
        database_connection.Close();
            
    }

    public void Save_Simulation()
    {
        IDbConnection database_connection = Create_And_Open_Database();
        IDbCommand save_simulation = database_connection.CreateCommand();
        save_simulation.CommandText =
            "INSERT INTO Simulation_Table(" +
            "simulation_id, frame_id," +
            "population_id, disease_id," +
            "num_of_healthy, num_of_infected," +
            "num_of_recovered)" +

            "VALUES(" +
            data_object.simulation_id + "," +
            data_object.frame_id + "," +
            data_object.population_id + "," +
            data_object.disease_id + "," +
            data_object.num_of_healthy + "," +
            data_object.num_of_infected + "," +
            data_object.num_of_recovered + ")";

        save_simulation.ExecuteNonQuery();
        database_connection.Close();
    }

    public int Get_Next_index(string Table_name)
    {
        IDbConnection database_connection = Create_And_Open_Database();
        IDbCommand get_next_index = database_connection.CreateCommand();
        get_next_index.CommandText =
            "SELECT IDENT_CURRENT(" + Table_name + ")";

        int index = get_next_index.ExecuteNonQuery();
        database_connection.Close();
        return index;
    }
}

