using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DatabaseManager : MonoBehaviour
{
    public DataObject data_object;

    public GuiManager gui_manager;
    public const string db_uri = "URI=file:Database.sqlite";


    // Start is called before the first frame update
    void Start()
    {
        Set_Up_Tables();
        Save_Simulation_Preset();
        data_object.population_name = "Default";
        data_object.disease_name = "Default";
        Load_Population_Preset();
        Load_Disease_Preset();
        Get_Max_Sim_ID();
        Debug.Log(Get_Results_Data(122));
    }

    // Checks for correct tables and linking of foreign keys
    private void Set_Up_Tables()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            // Population Table

            using (IDbCommand population_table_command = database_connection.CreateCommand())
            {
                population_table_command.CommandText = "CREATE TABLE IF NOT EXISTS Population_Table(" +
                    "population_id INTEGER NOT NULL," +
                    "population_count INTEGER," +
                    "population_name varchar(255) NOT NULL UNIQUE," +
                    "global_speed FLOAT," +
                    "number_of_sensors INTEGER," +
                    "PRIMARY KEY (population_id AUTOINCREMENT))";

                population_table_command.ExecuteNonQuery();
            }

            using (IDbCommand population_table_default = database_connection.CreateCommand())
            {
                population_table_default.CommandText =
                    "INSERT OR REPLACE INTO Population_Table(population_id," +
                    "population_count, population_name," +
                    "global_speed, number_of_sensors)" +

                    "VALUES(0,100000,'Default',14,20)";

                population_table_default.ExecuteNonQuery();
            }      

            // Disease Table
            using (IDbCommand disease_Table_command = database_connection.CreateCommand())
            {
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
            }

            using(IDbCommand disease_table_default = database_connection.CreateCommand())
            {
                disease_table_default.CommandText =
                    "INSERT OR REPLACE INTO Disease_Table(disease_id," +
                    "disease_name, transmission_radius," +
                    "min_infection_time, max_infection_time," +
                    "min_recovering_time, max_recovering_time," +
                    "is_reccuring)" +

                    "VALUES(0," +
                    "'Default', 4, " +
                    "2, 4," +
                    "6, 8," +
                    "1)";

                disease_table_default.ExecuteNonQuery();
            }


            // Simulation Results Table
            using (IDbCommand simulation_results_table_command = database_connection.CreateCommand())
            {
                simulation_results_table_command.CommandText =
                    "CREATE TABLE IF NOT EXISTS Simulation_Results_Table(" +
                    "simulation_id INTEGER," +
                    "frame_id INTEGER," +              
                    "num_of_healthy INTEGER," +
                    "num_of_infected INTEGER," +
                    "num_of_recovered INTEGER," +
                    "num_of_immune INTEGER," +
                    "PRIMARY KEY (simulation_id, frame_id))";

                simulation_results_table_command.ExecuteReader();
            }

            using (IDbCommand simulation_preset_table_command = database_connection.CreateCommand())
            {
                // Simulation Preset Table
                simulation_preset_table_command.CommandText =
                    "CREATE TABLE IF NOT EXISTS Simulation_Preset_Table(" +
                    "simulation_id INTEGER," +
                    "disease_id INTEGER," +
                    "population_id INTEGER," +
                    "FOREIGN KEY (population_id) REFERENCES Population_Table(population_id)," +
                    "FOREIGN KEY (disease_id) REFERENCES Disease_Table(disease_id)," +
                    "FOREIGN KEY (simulation_id) REFERENCES Simulation_Results_Table(simulation_id)" +
                    "PRIMARY KEY (simulation_id))";

                simulation_preset_table_command.ExecuteReader();
            }

            using (IDbCommand simulation_results_table_default = database_connection.CreateCommand())
            {
                simulation_results_table_default.CommandText =
                    "INSERT OR REPLACE INTO Simulation_Preset_Table(" +
                    "simulation_id, population_id," +
                    "disease_id)" +

                    "VALUES(" +
                    "0, 0," +
                    "0)";
                simulation_results_table_default.ExecuteNonQuery();
            }
        }
    }

    // passes through relavent data to the population table with 
    public void Save_Population()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();

            using(IDbCommand save_population = database_connection.CreateCommand())
            {
                save_population.CommandText  =
                    "INSERT OR REPLACE INTO Population_Table(" +
                    "population_count, population_name," +
                    "global_speed, number_of_sensors)" +

                    "VALUES( @Population_Count, @Population_Name, @Global_Speed, @Number_Of_Sensors)";

                // Setting up of the parameters to the data in the data object for saving 

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

                
                save_population.ExecuteNonQuery();
            }
        }
    }

    public void Save_Disease()
    {
        using(IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();

            using (IDbCommand save_disease = database_connection.CreateCommand())
            {
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
                param_max_infection_time.Value = data_object.infection_time.y;
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
                param_is_reccuring.Value = data_object.is_reccuring;
                save_disease.Parameters.Add(param_is_reccuring);


                save_disease.ExecuteNonQuery();
            }
        }
    }

    public void Save_Simulation_Preset()
    {
        using(IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();

            using (IDbCommand save_simulation_preset = database_connection.CreateCommand())
            {
                save_simulation_preset.CommandText =
                    "INSERT OR REPLACE INTO Simulation_Preset_Table(" +
                    "simulation_id, population_id," +
                    "disease_id)" +

                    "VALUES(" +
                    "@Simulation_id, @Population_id," +
                    "@Disease_id)";

                IDbDataParameter param_simulation_id = save_simulation_preset.CreateParameter();
                param_simulation_id.ParameterName = "@Simulation_id";
                param_simulation_id.Value = data_object.simulation_id;
                save_simulation_preset.Parameters.Add( param_simulation_id );

                IDbDataParameter param_population_id = save_simulation_preset.CreateParameter();
                param_population_id.ParameterName = "@Population_id";
                param_population_id.Value = data_object.population_id;
                save_simulation_preset.Parameters.Add(param_population_id);

                IDbDataParameter param_disease_id = save_simulation_preset.CreateParameter();
                param_disease_id.ParameterName = "@Disease_id";
                param_disease_id.Value = data_object.disease_id;
                save_simulation_preset.Parameters.Add(param_disease_id);

                save_simulation_preset.ExecuteNonQuery();
            }
        }
    }

    public void Save_Simulation_Results()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using (IDbCommand save_simulation_results = database_connection.CreateCommand())
            {
                save_simulation_results.CommandText =
                    "INSERT OR REPLACE INTO Simulation_Results_Table(" +
                    "simulation_id, frame_id," +
                    "num_of_healthy, num_of_infected," +
                    "num_of_recovered, num_of_immune)" +

                    "VALUES(" +
                    "@Simulation_id, @Frame_id," +
                    "@Num_Of_Healthy, @Num_Of_Infected," +
                    "@Num_Of_Recovered, @Num_of_Immune)";

                IDbDataParameter param_simulation_id = save_simulation_results.CreateParameter();
                param_simulation_id.ParameterName = "@Simulation_id";
                param_simulation_id.Value = data_object.simulation_id;
                save_simulation_results.Parameters.Add(param_simulation_id);

                IDbDataParameter param_frame_id = save_simulation_results.CreateParameter();
                param_frame_id.ParameterName = "@Frame_id";
                param_frame_id.Value = data_object.frame_id;
                save_simulation_results.Parameters.Add(param_frame_id);

                IDbDataParameter param_num_of_healthy = save_simulation_results.CreateParameter();
                param_num_of_healthy.ParameterName = "@Num_Of_Healthy";
                param_num_of_healthy.Value = data_object.num_of_healthy;
                save_simulation_results.Parameters.Add(param_num_of_healthy);

                IDbDataParameter param_num_of_infected = save_simulation_results.CreateParameter();
                param_num_of_infected.ParameterName = "@Num_Of_Infected";
                param_num_of_infected.Value = data_object.num_of_infected;
                save_simulation_results.Parameters.Add(param_num_of_infected);

                IDbDataParameter param_num_of_recovered = save_simulation_results.CreateParameter();
                param_num_of_recovered.ParameterName = "@Num_Of_Recovered";
                param_num_of_recovered.Value = data_object.num_of_recovered;
                save_simulation_results.Parameters.Add(param_num_of_recovered);

                IDbDataParameter param_num_of_immune = save_simulation_results.CreateParameter();
                param_num_of_immune.ParameterName = "@Num_of_Immune";
                param_num_of_immune.Value = data_object.num_of_immune;
                save_simulation_results.Parameters.Add(param_num_of_immune);

                save_simulation_results.ExecuteNonQuery();
            }
        }
    }

    public void Get_Max_Sim_ID()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using(IDbCommand get_max_sim_id = database_connection.CreateCommand())
            {
                get_max_sim_id.CommandText =
                    "SELECT MAX(simulation_id) FROM Simulation_Preset_Table";
                data_object.simulation_id = (int)(long)get_max_sim_id.ExecuteScalar() + 1;
                //Debug.Log((long)get_max_sim_id.ExecuteScalar());
            }
        }
    }

    public void Get_Max_Pop_ID()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using (IDbCommand get_max_pop_id = database_connection.CreateCommand())
            {
                get_max_pop_id.CommandText =
                    "SELECT MAX(population_id) FROM Population_Table";

                data_object.population_id = (int)(long)get_max_pop_id.ExecuteScalar() + 1;
            }
        }
    }

    public void Get_Max_Dis_ID()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using (IDbCommand get_max_dis_id = database_connection.CreateCommand())
            {
                get_max_dis_id.CommandText =
                    "SELECT MAX(disease_id) FROM Disease_Table";
                
                data_object.disease_id = (int)(long)get_max_dis_id.ExecuteScalar() + 1;
            }
        }
    }

    public void Load_Population_Preset()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using(IDbCommand load_population = database_connection.CreateCommand())
            {
                load_population.CommandText =
                    "SELECT * FROM Population_Table WHERE population_name == @Given_Population_name";

                IDbDataParameter param_population_name = load_population.CreateParameter();
                param_population_name.ParameterName = "@Given_Population_name";
                param_population_name.Value = data_object.population_name;
                load_population.Parameters.Add(param_population_name);


                IDataReader reader = load_population.ExecuteReader();

                bool null_value = false;
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        null_value = true;
                    };
                };

                if (null_value)
                {
                    gui_manager.Invalid_Population_Query();
                    return;
                }
                while (reader.Read())
                {
                    data_object.population_id = reader.GetInt32(0);
                    data_object.population_count = reader.GetInt32(1);
                    data_object.population_name = reader.GetString(2);
                    data_object.global_speed = reader.GetFloat(3);
                    data_object.number_of_sensors = reader.GetInt32(4);
                }

                gui_manager.Reset_Population_Sliders();
            }
        }
    }

    public void Load_Disease_Preset()
    {
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using(IDbCommand load_disease = database_connection.CreateCommand())
            {
                load_disease.CommandText =
                    "SELECT * FROM Disease_Table WHERE disease_name == @Given_Disease_name";

                IDbDataParameter param_disease_name = load_disease.CreateParameter();
                param_disease_name.ParameterName = "@Given_Disease_name";
                param_disease_name.Value = data_object.disease_name;
                load_disease.Parameters.Add(param_disease_name);

                IDataReader reader = load_disease.ExecuteReader();

                bool null_value = false;
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        //Debug.Log("Null values");
                        null_value = true;
                    };
                };

                if (null_value)
                {
                    gui_manager.Invalid_Disease_Query();
                    return;
                }

                while (reader.Read())
                {
                    

                    data_object.disease_id = reader.GetInt32(0);
                    data_object.disease_name = reader.GetString(1);
                    data_object.radius = reader.GetFloat(2);
                    data_object.infection_time.x = reader.GetFloat(3);
                    data_object.infection_time.y = reader.GetFloat(4);
                    data_object.recovering_time.x = reader.GetFloat(5);
                    data_object.recovering_time.y = reader.GetFloat(6);
                    data_object.is_reccuring = reader.GetBoolean(7);
                }

                gui_manager.Reset_Disease_Sliders();
            }
        }
    }

    public List<string> Get_Population_Names()
    {
        List<string> names = new List<string>();
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using(IDbCommand get_population_names = database_connection.CreateCommand())
            {
                get_population_names.CommandText =
                    "Select population_name FROM Population_Table";
                IDataReader reader = get_population_names.ExecuteReader();

                while (reader.Read())
                {
                    for(int i =  0; i < reader.FieldCount; i++)
                    {
                        names.Add(reader.GetString(i));
                    }
                }
                return names;
            }
        }
    }

    public List<string> Get_Disease_Names()
    {
        List<string> names = new List<string>();
        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();
            using(IDbCommand get_disease_names = database_connection.CreateCommand())
            {
                get_disease_names.CommandText =
                    "SELECT disease_name FROM Disease_Table";
                IDataReader reader = get_disease_names.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0;i < reader.FieldCount; i++)
                    {
                        names.Add(reader.GetString(i));
                    }
                }
                return names;
            }
        }
    }


    public List<List<int>> Get_Results_Data(int simulation_id)
    {
        List<List<int>> results_data = new List<List<int>>();
        int frame_count = new int(); 

        using (IDbConnection database_connection = new SqliteConnection(db_uri))
        {
            database_connection.Open();

            // Command to find the number of records (frames) are saved into the database
            using (IDbCommand get_frame_count = database_connection.CreateCommand())
            {
                get_frame_count.CommandText =
                    "SELECT Count(frame_id) FROM Simulation_Results_Table WHERE simulation_id = @Given_Simulation_ID";

                IDbDataParameter given_simulation_id = get_frame_count.CreateParameter();
                given_simulation_id.ParameterName = "@Given_Simulation_ID";
                given_simulation_id.Value = simulation_id;
                get_frame_count.Parameters.Add(given_simulation_id);

                IDataReader reader = get_frame_count.ExecuteReader();
                while (reader.Read())
                {
                    frame_count = reader.GetInt32(0);
                }
                reader.Close();
            }

            // Command that iterates through the database and frames to read the data saved
            using (IDbCommand get_results_data = database_connection.CreateCommand())
            {
                get_results_data.CommandText =
                    "SELECT * FROM Simulation_Results_Table WHERE (simulation_id == @Given_Simulation_ID AND frame_id == @Given_Frame_ID)";

                IDbDataParameter given_simulation_id = get_results_data.CreateParameter();
                given_simulation_id.ParameterName = "@Given_Simulation_ID";
                given_simulation_id.Value = simulation_id;
                get_results_data.Parameters.Add(given_simulation_id);

                // iterate for the length of that simulation
                for (int i = 0; i< frame_count; i++)
                {
                    IDbDataParameter given_frame_id = get_results_data.CreateParameter();
                    given_frame_id.ParameterName = "@Given_Frame_ID";
                    given_frame_id.Value = frame_count;
                    get_results_data.Parameters.Add(given_frame_id);

                    IDataReader frame_reader = get_results_data.ExecuteReader();

                    while (frame_reader.Read())
                    {
                        // Iterate through the field of the database
                        List<int> frame_data = new List<int>();

                        for(int j = 0;  j < frame_reader.FieldCount; j++)
                        {
                            // save the data to the temp array
                            frame_data.Add(frame_reader.GetInt32(j));
                        }
                        // write the temp array to the results
                        results_data.Add(frame_data);
                    }
                    frame_reader.Close();
                }
            }
        }
        return results_data;
    }

}

