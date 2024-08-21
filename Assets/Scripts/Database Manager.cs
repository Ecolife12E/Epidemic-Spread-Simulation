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



        //Debug.Log(Get_Next_index("Simulation_Table"));
        database_connection.Close();
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
            "population_name varchar(255) NOT NULL UNIQUE," +
            "global_speed FLOAT," +
            "number_of_sensors INTEGER," +
            //"UNIQUE(population_count,global_speed,number_of_sensors)" +
            "PRIMARY KEY (population_id AUTOINCREMENT))";
        population_table_command.ExecuteReader();


        // Disease Table
        IDbCommand disease_Table_command = dbConnection.CreateCommand();
        disease_Table_command.CommandText = "CREATE TABLE IF NOT EXISTS Disease_Table(" +
            "disease_id INTEGER NOT NULL," +
            "disease_name varchar(255) NOT NULL UNIQUE," +
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
            "INSERT OR REPLACE INTO Population_Table(" +
            "population_count, population_name," +
            "global_speed, number_of_sensors)" +

            "VALUES( @Population_Count, @Population_Name, @Global_Speed, @Number_Of_Sensors)";

        IDbDataParameter param_population_count = save_population.CreateParameter();
        param_population_count.ParameterName = "@Population_Count";
        param_population_count.Value = data_object.population_count;
        save_population.Parameters.Add(param_population_count);

        IDbDataParameter param_population_name = save_population.CreateParameter();
        param_population_name.ParameterName = "@Population_Name";
        param_population_name.Value = data_object.population_name;
        save_population.Parameters.Add(param_population_name);

        IDbDataParameter param_global_speed = save_population.CreateParameter();
        param_global_speed.ParameterName = "@Global_Speed";
        param_global_speed.Value = data_object.global_speed;
        save_population.Parameters.Add(param_global_speed);

        IDbDataParameter param_number_of_sensors = save_population.CreateParameter();
        param_number_of_sensors.ParameterName = "@Number_Of_Sensors";
        param_number_of_sensors.Value = data_object.number_of_sensors;
        save_population.Parameters.Add(param_number_of_sensors);

        try
        {
            save_population.ExecuteNonQuery();
        }
        finally
        {
            database_connection.Close();
        }
    }

    public void Save_Disease()
    {
        IDbConnection database_connection = Create_And_Open_Database();
        IDbCommand save_disease = database_connection.CreateCommand();
        save_disease.CommandText =
            "INSERT OR REPLACE INTO Disease_Table(" +
            "disease_name, transmission_radius," +
            "min_infection_time, max_infection_time," +
            "min_recovering_time, max_recovering_time," +
            "is_reccuring)" +

            "VALUES(" +
            "@Disease_Name, @Transmission_Radius, " +
            "@Min_Infection_Time, @Max_Infection_Time," +
            "@Min_Recovering_Time, @Max_Recovering_Time," +
            "@Is_Reccuring)";
            
        IDbDataParameter param_disease_name = save_disease.CreateParameter();
        param_disease_name.ParameterName = "@Disease_Name";
        param_disease_name.Value = data_object.disease_name;
        save_disease.Parameters.Add(param_disease_name);

        IDbDataParameter param_transmission_radius = save_disease.CreateParameter();
        param_transmission_radius.ParameterName = "@Transmission_Radius";
        param_transmission_radius.Value = data_object.radius;
        save_disease.Parameters.Add(param_transmission_radius);

        IDbDataParameter param_min_infection_time = save_disease.CreateParameter();
        param_min_infection_time.ParameterName = "@Min_Infection_Time";
        param_min_infection_time.Value = data_object.infection_time.x;
        save_disease.Parameters.Add(param_min_infection_time);

        IDbDataParameter param_max_infection_time = save_disease.CreateParameter();
        param_max_infection_time.ParameterName = "@Max_Infection_Time";
        param_max_infection_time .Value = data_object.infection_time.y;
        save_disease.Parameters.Add(param_max_infection_time);

        IDbDataParameter param_min_recovering_time = save_disease.CreateParameter();
        param_min_recovering_time.ParameterName = "@Min_Recovering_Time";
        param_min_recovering_time.Value = data_object.recovering_time.x;
        save_disease.Parameters.Add(param_min_recovering_time);

        IDbDataParameter param_max_recovering_time = save_disease.CreateParameter();
        param_max_recovering_time.ParameterName = "@Max_Recovering_Time";
        param_max_recovering_time.Value = data_object.recovering_time.y;
        save_disease.Parameters.Add(param_max_recovering_time);

        IDbDataParameter param_is_reccuring = save_disease.CreateParameter();
        param_is_reccuring.ParameterName = "@Is_Reccuring";
        param_is_reccuring .Value = data_object.is_reccuring;
        save_disease.Parameters.Add(param_is_reccuring);

        try
        {
            save_disease.ExecuteNonQuery();
        }
        finally
        {
            database_connection.Close();
        }
    }

    public void Save_Simulation()
    {
        IDbConnection database_connection = Create_And_Open_Database();
        IDbCommand save_simulation = database_connection.CreateCommand();
        save_simulation.CommandText =
            "INSERT OR REPLACE INTO Simulation_Table(" +
            "simulation_id, frame_id," +
            "population_id, disease_id," +
            "num_of_healthy, num_of_infected," +
            "num_of_recovered)" +

            "VALUES(" +
            "@Simulation_id, @Frame_id," +
            "@Population_id, @Disease_id," +
            "@Num_Of_Healthy, @Num_Of_Infected," +
            "@Num_of_Recovered)";

        IDbDataParameter param_simulation_id = save_simulation.CreateParameter();
        param_simulation_id.ParameterName = "@Simulation_id";
        param_simulation_id.Value = data_object.simulation_id;
        save_simulation.Parameters.Add(param_simulation_id);

        IDbDataParameter param_frame_id = save_simulation.CreateParameter();
        param_frame_id.ParameterName = "@Frame_id";
        param_frame_id.Value = data_object.frame_id;
        save_simulation.Parameters.Add(param_frame_id);

        IDbDataParameter param_population_id = save_simulation.CreateParameter();
        param_population_id.ParameterName = "@Population_id";
        param_population_id.Value = data_object.population_id;
        save_simulation.Parameters.Add(param_population_id);

        IDbDataParameter param_disease_id = save_simulation.CreateParameter();
        param_disease_id.ParameterName = "@Disease_id";
        param_disease_id.Value = data_object.disease_id;
        save_simulation.Parameters.Add(param_disease_id);

        IDbDataParameter param_num_of_healthy = save_simulation.CreateParameter();
        param_num_of_healthy.ParameterName = "@Num_Of_Healthy";
        param_num_of_healthy.Value = data_object.num_of_healthy;
        save_simulation.Parameters.Add(param_num_of_healthy);

        IDbDataParameter param_num_of_infected = save_simulation.CreateParameter();
        param_num_of_infected.ParameterName = "@Num_Of_Infected";
        param_num_of_infected.Value = data_object.num_of_infected;
        save_simulation.Parameters.Add(param_num_of_infected);

        IDbDataParameter param_num_of_recovered = save_simulation.CreateParameter();
        param_num_of_recovered.ParameterName = "@Num_Of_Recovered";
        param_num_of_recovered.Value = data_object.num_of_recovered;
        save_simulation.Parameters.Add(param_num_of_recovered);

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

