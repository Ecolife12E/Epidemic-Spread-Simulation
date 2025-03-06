using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComputeManager : MonoBehaviour
{
    // Compute Shaders and Textures
    public ComputeShader simulation_compute_shader;
    private RenderTexture simulation_render_texture;
    private RenderTexture graph_render_texture;


    // DataObject that holds all the needed data
    public DataObject data_object;

    // Structs that are sent to the Compute Shaders
    public struct Person
    {
        public Vector2 position;
        public Vector2 target_position;
        public float speed_percentage;
        public int health_state;
        public float remaining_time;
    }

    public struct Data_Struct
    {
        public int number_of_healthy;
        public int number_of_infected;
        public int number_of_recovered;
        public int number_of_immune;
    }

    public struct Graph_Data
    {
        public int frame_id;
        public int num_of_healthy;
        public int num_of_infected;
        public int num_of_recovered;
        public int num_of_immune;
    }


    // Creating the buffers and their sizes
    private int buffer_size; // size of each person in bits
    private Person[] buffer_data; // array to save population data to
    private int data_buffer_size; // size of Data_Struct in bits    
    private Data_Struct[] data_buffer_data;
    private int graph_buffer_size;
    private Graph_Data[] graph_buffer_data;
    private Vector2[] debug_buffer_data;

    private Graph_Data[] graph_1_buffer_data;
    private Graph_Data[] graph_2_buffer_data;


    // Connection to database Manager to run functions
    public DatabaseManager database_manager;
    public GuiManager gui_manager;

    // Used to track how many times right mouse button has been clicked.
    private int RMB_clicks;

    // Start is called before the first frame update
    void Start()
    {
        data_object.frame_id = 0;
        data_object.zoomed = false;



        // a new texture is creates and we enable RandomWrite so the texture can be modified
        simulation_render_texture = new RenderTexture(data_object.texture_width, data_object.texture_height, 24);
        simulation_render_texture.enableRandomWrite = true;
        simulation_render_texture.Create();

        graph_render_texture = new RenderTexture(data_object.texture_width, data_object.texture_height, 24);
        graph_render_texture.enableRandomWrite = true;
        graph_render_texture.Create();


        // Passes render_texture to the compute shader under the variable name Result
        simulation_compute_shader.SetTexture(0, "Result", simulation_render_texture);



        // Compute Buffer to hold data being sent to GPU
        buffer_size = sizeof(float) * 6 + sizeof(int);
        buffer_data = new Person[data_object.population_count];

        debug_buffer_data = new Vector2[data_object.population_count];

        data_buffer_size = sizeof(int) * 4;
        data_buffer_data = new Data_Struct[1]; // only one instance as there is no need for more counters.

        graph_buffer_size = sizeof(int) * 5;


        database_manager.Get_Max_Sim_ID();


        // create the population with some random values
        for (int i = 0; i < data_object.population_count; i++)
        {
            // Gives each new point a random staring position within the texture bounds
            buffer_data[i].position = new Vector2(Random.Range(0, data_object.texture_width), Random.Range(0, data_object.texture_height));
            // Gives each new point a random target position within the texture bounds
            buffer_data[i].target_position = new Vector2(Random.Range(0, data_object.texture_width), Random.Range(0, data_object.texture_height));
            // Gives each point a speed percentage that creates variance
            buffer_data[i].speed_percentage = Random.Range(25, 125);
            // fills the remaining time values with the max time. 
            buffer_data[i].remaining_time = data_object.infection_time.y;

            // creates initial infected person
            if (i == 0)
            {
                buffer_data[i].health_state = 1;
                data_buffer_data[0].number_of_infected += 1;
            }
            else
            {
                // sets each point to being of healthy state
                buffer_data[i].health_state = 0;
                data_buffer_data[0].number_of_healthy += 1;
            }

            RMB_clicks = 0;

        }
    }

    public void Start_Simulation()
    {
        if (!data_object.simulation_active)
        {
            data_object.simulation_active = true;
            // creates a Computebuffer setting each person to be buffer_size bits long
            ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
            // fills the buffer with the data from the buffer_data array
            buffer.SetData(buffer_data);

            ComputeBuffer data_buffer = new ComputeBuffer(data_buffer_data.Length, data_buffer_size);
            data_buffer.SetData(data_buffer_data);



            // another buffer for debugging
            ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float) * 2);
            debug_buffer.SetData(debug_buffer_data);

            // passing through of all the global variables in to the compute shader
            simulation_compute_shader.SetBuffer(0, "buffer", buffer);
            simulation_compute_shader.SetBuffer(0, "data_buffer", data_buffer);

            simulation_compute_shader.SetFloat("global_speed", data_object.global_speed);
            simulation_compute_shader.SetFloat("min_distance", data_object.min_distance);
            simulation_compute_shader.SetFloat("PI", Mathf.PI);
            simulation_compute_shader.SetInt("texture_width", data_object.texture_width);
            simulation_compute_shader.SetInt("texture_height", data_object.texture_height);
            simulation_compute_shader.SetFloat("radius", data_object.radius);
            simulation_compute_shader.SetInt("number_of_sensors", data_object.number_of_sensors);
            simulation_compute_shader.SetBool("show_sensors", data_object.show_sensors);
            simulation_compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);
            simulation_compute_shader.SetFloat("deltatime", Time.deltaTime);
            simulation_compute_shader.SetFloat("min_infectious_time", data_object.infection_time.x);
            simulation_compute_shader.SetFloat("max_infectious_time", data_object.infection_time.y);
            simulation_compute_shader.SetFloat("min_recovering_time", data_object.recovering_time.x);
            simulation_compute_shader.SetFloat("max_recovering_time", data_object.recovering_time.y);
            simulation_compute_shader.SetBool("is_recursive", data_object.is_reccuring);
            simulation_compute_shader.SetBool("immunity", data_object.immunity);
            simulation_compute_shader.SetFloat("immunity_chance", data_object.immunity_chance);

            // Allows the Compute Shader to run, with the needed threads
            simulation_compute_shader.Dispatch(0, 1024, 1, 1);

            // retrieves the processed data from the buffer then deleting the buffer
            buffer.GetData(buffer_data);
            buffer.Dispose();

            data_buffer.GetData(data_buffer_data);
            data_buffer.Dispose();
        }
        else
        {
            data_object.simulation_active = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (data_object.simulation_active)
        {
            // clears the texture so there are no trails
            simulation_render_texture.Release();
            simulation_compute_shader.SetTexture(0, "Result", simulation_render_texture);

            // creates a new buffer and fills it with the data from the buffer_data array
            ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
            buffer.SetData(buffer_data);
            simulation_compute_shader.SetBuffer(0, "buffer", buffer);

            // another buffer for debugging
            ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float) * 2);
            debug_buffer.SetData(debug_buffer_data);
            simulation_compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);

            // Compute Buffer for the Data retrived
            ComputeBuffer data_buffer = new ComputeBuffer(data_buffer_data.Length, data_buffer_size);
            data_buffer.SetData(data_buffer_data);
            simulation_compute_shader.SetBuffer(0, "data_buffer", data_buffer);

            // passes through the global variables for the Compute shader to use.
            simulation_compute_shader.SetFloat("global_speed", data_object.global_speed);
            simulation_compute_shader.SetFloat("min_distance", data_object.min_distance);
            simulation_compute_shader.SetFloat("PI", Mathf.PI);
            simulation_compute_shader.SetInt("texture_width", data_object.texture_width);
            simulation_compute_shader.SetInt("texture_height", data_object.texture_height);
            simulation_compute_shader.SetFloat("radius", data_object.radius);
            simulation_compute_shader.SetInt("number_of_sensors", data_object.number_of_sensors);
            simulation_compute_shader.SetBool("show_sensors", data_object.show_sensors);
            simulation_compute_shader.SetFloat("deltatime", Time.deltaTime);
            simulation_compute_shader.SetFloat("min_infectious_time", data_object.infection_time.x);
            simulation_compute_shader.SetFloat("max_infectious_time", data_object.infection_time.y);
            simulation_compute_shader.SetFloat("min_recovering_time", data_object.recovering_time.x);
            simulation_compute_shader.SetFloat("max_recovering_time", data_object.recovering_time.y);
            simulation_compute_shader.SetBool("is_recursive", data_object.is_reccuring);
            simulation_compute_shader.SetBool("immunity", data_object.immunity);
            simulation_compute_shader.SetFloat("immunity_chance", data_object.immunity_chance);

            simulation_compute_shader.Dispatch(0, 1024, 1, 1);

            // retrieves data from the compute buffers and then deletes the buffers.
            buffer.GetData(buffer_data);
            debug_buffer.GetData(debug_buffer_data);

            buffer.Dispose();
            debug_buffer.Dispose();

            data_buffer.GetData(data_buffer_data);
            data_buffer.Dispose();
            UpdateData(data_buffer_data);

            database_manager.Save_Simulation_Results();

            // Add_Record_CSV(data_object.frame_id, data_object.num_of_healthy, data_object.num_of_infected, data_object.num_of_recovered, "C:\\Users\\willb\\Unity Projects\\Computer Science Coursework - Epidemic Spread Simulation\\Assets\\Results\\Results.csv");
            data_object.frame_id++;

        }
        /*
        else if (data_object.graph_active)
        {
            Graph_Compute_Shader(data_object.chosen_simulation_id); // temporary passing in of value
        }
        */
        else if (data_object.graph_active)
        {
            if (data_object.chosen_simulation_id_2 != 0)
            {
                Compare_Graph_Compute_Shader(data_object.chosen_simulation_id_1, data_object.chosen_simulation_id_2);
            }
            else
            {
                Graph_Compute_Shader(data_object.chosen_simulation_id);
            }

            // If Right mouse button pressed
            if (Input.GetMouseButtonDown(1))
            {
                // Calculate Width Density
                int width_desnity = (int)Mathf.Floor(data_object.max_frame / data_object.texture_width);

                // Check how many times the RMB has been pressed to determine which variable to save to
                if (RMB_clicks % 2 == 0)
                {
                    // Store in first frame option
                    data_object.chosen_frame_1 = (int)Mathf.Floor(Input.mousePosition.x * width_desnity);
                    RMB_clicks++;
                }
                else
                {
                    // Store in second frame option
                    data_object.chosen_frame_2 = (int)Mathf.Floor(Input.mousePosition.x * width_desnity);
                    RMB_clicks++;

                    // get graph to update on screen
                    data_object.zoomed = true;
                    data_object.chosen_graph_drawn = false;
                }
            }
            // If middle Mouse Button Clicked
            if (Input.GetMouseButtonDown(2))
            {
                data_object.zoomed = false;
                data_object.chosen_graph_drawn = false;
                data_object.chosen_frame_1 = 0;
                data_object.chosen_frame_2 = 0;
            }
        }
    }

    public void Reset_Compute_Shader()
    {
        database_manager.Save_Simulation_Preset();
        database_manager.Get_Max_Sim_ID();
        Start();
    }


    public void Graph_Compute_Shader(int given_simulation_id)
    {
        Debug.Log("Single Graph run");

        int max_frame;
        int min_frame;
        int frame_difference;

        if (!data_object.chosen_graph_drawn)
        {
            graph_render_texture.Release();
            simulation_compute_shader.SetTexture(1, "Graph_Result", graph_render_texture);

            List<List<int>> results_data = database_manager.Get_Results_Data(given_simulation_id);
            //Debug.Log(results_data.Count);

            // check if the user has selected frames to view inbetween
            if (data_object.zoomed)
            {
                // find which one is max and min
                max_frame = Mathf.Max(data_object.chosen_frame_1, data_object.chosen_frame_2);
                min_frame = Mathf.Min(data_object.chosen_frame_1, data_object.chosen_frame_2);

                // new length of buffer
                frame_difference = max_frame - min_frame;

                graph_buffer_data = new Graph_Data[frame_difference];
            }
            else
            {
                max_frame = database_manager.Get_Max_Frame_ID(given_simulation_id);
                min_frame = 0;
                frame_difference = max_frame;

                graph_buffer_data = new Graph_Data[max_frame];
            }

            data_object.max_frame = max_frame;

            for (int i = min_frame; i < max_frame; i++)
            {
                //Debug.Log(i);
                graph_buffer_data[i - min_frame].frame_id = results_data[i][1];
                graph_buffer_data[i - min_frame].num_of_healthy = results_data[i][2];
                graph_buffer_data[i - min_frame].num_of_infected = results_data[i][3];
                graph_buffer_data[i - min_frame].num_of_recovered = results_data[i][4];
                graph_buffer_data[i - min_frame].num_of_immune = results_data[i][5];
                // Debug.Log(results_data[i][0]);
            }


            ComputeBuffer graph_buffer = new ComputeBuffer(graph_buffer_data.Length, graph_buffer_size);
            graph_buffer.SetData(graph_buffer_data);
            simulation_compute_shader.SetBuffer(1, "graph_buffer", graph_buffer);

            ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float) * 2);
            debug_buffer.SetData(debug_buffer_data);
            simulation_compute_shader.SetBuffer(1, "debug_buffer", debug_buffer);

            simulation_compute_shader.SetInt("texture_width", data_object.texture_width);
            simulation_compute_shader.SetInt("texture_height", data_object.texture_height);
            simulation_compute_shader.SetInt("population_count", database_manager.Get_Population_Count(given_simulation_id));
            simulation_compute_shader.SetInt("max_frame_id", frame_difference);
            simulation_compute_shader.SetInt("min_frame_id", min_frame);

            simulation_compute_shader.Dispatch(1, 128, 1, 1);

            debug_buffer.GetData(debug_buffer_data);
            debug_buffer.Dispose();

            graph_buffer.GetData(graph_buffer_data);
            graph_buffer.Dispose();

            data_object.chosen_graph_drawn = true;
        }
        else
        {
            if (graph_buffer_data == null)
            {
                graph_buffer_data = new Graph_Data[database_manager.Get_Max_Frame_ID(given_simulation_id)];
                List<List<int>> results_data = database_manager.Get_Results_Data(given_simulation_id);

                for (int i = 0; i < database_manager.Get_Max_Frame_ID(given_simulation_id); i++)
                {
                    graph_buffer_data[i].frame_id = results_data[i][1];
                    graph_buffer_data[i].num_of_healthy = results_data[i][2];
                    graph_buffer_data[i].num_of_infected = results_data[i][3];
                    graph_buffer_data[i].num_of_recovered = results_data[i][4];
                    graph_buffer_data[i].num_of_immune = results_data[i][5];
                }

            }

            // If User has Zoomed the graph
            if (data_object.zoomed)
            {
                // Calculate max and min of the two clicks
                min_frame = Mathf.Min(data_object.chosen_frame_1, data_object.chosen_frame_2);
                max_frame = Mathf.Max(data_object.chosen_frame_1, data_object.chosen_frame_2);
                // Calculate Difference
                frame_difference = max_frame - min_frame;
            }
            else
            {
                // Set Min and Max frames
                min_frame = 0;
                max_frame = data_object.max_frame;
                frame_difference = max_frame;
            }

            // Get X position of the mouse pointer
            int mouse_x_position = (int)Mathf.Floor(Input.mousePosition.x);
            // Calculate Width Density based on how many frames there are
            int width_desnity = (int)Mathf.Floor(frame_difference / data_object.texture_width);
            int selected_frame = (mouse_x_position * width_desnity);

            // If the mouse pointer is on the screen.
            if (selected_frame > 0 && selected_frame < frame_difference)
            {
                // Update and send values to UI
                int healthy = graph_buffer_data[selected_frame].num_of_healthy;
                int infected = graph_buffer_data[selected_frame].num_of_infected;
                int recovering = graph_buffer_data[selected_frame].num_of_recovered;
                int immune = graph_buffer_data[selected_frame].num_of_immune;

                gui_manager.Update_Graph_Data(healthy, infected, recovering, immune);

                // Calculating the R-Values
                double graph_1_R_value;
                try
                {
                    // Calculate the R value estimation
                    graph_1_R_value = (float)graph_buffer_data[selected_frame + 1].num_of_infected / infected;
                }
                catch
                {
                    // Takes this path if trying to divide by 0
                    graph_1_R_value = 0;
                }

                // Update UI
                gui_manager.Update_R_Values(graph_1_R_value, (double)100);
            }
        }
    }

    public void Compare_Graph_Compute_Shader(int given_simulation_id_1, int given_simulation_id_2)
    {
        // only executing when needed to maintain performance

        int max_frame;
        int min_frame;
        int frame_difference;

        if (!data_object.chosen_graph_drawn)
        {
            Debug.Log("Comparing Graph Ran");
            // Clear any previous writeing to screen 
            graph_render_texture.Release();
            simulation_compute_shader.SetTexture(2, "Graph_Result", graph_render_texture);

            // Get the results for the data in lists and their respective frame counts;
            List<List<int>> graph_1_results = database_manager.Get_Results_Data(given_simulation_id_1);
            //Debug.Log(graph_1_results[0].ToString());
            int graph_1_max_frame = data_object.max_frame;
            List<List<int>> graph_2_results = database_manager.Get_Results_Data(given_simulation_id_2);
            int graph_2_max_frame = data_object.max_frame;


            if (data_object.zoomed)
            {
                max_frame = Mathf.Max(data_object.chosen_frame_1, data_object.chosen_frame_2);
                min_frame = Mathf.Min(data_object.chosen_frame_1, data_object.chosen_frame_2);

                frame_difference = max_frame - min_frame;

                graph_1_buffer_data = new Graph_Data[frame_difference];
                graph_2_buffer_data = new Graph_Data[frame_difference];
            }
            else
            {
                max_frame = Mathf.Max(graph_1_max_frame, graph_2_max_frame);
                min_frame = 0;
                frame_difference = max_frame;

                graph_1_buffer_data = new Graph_Data[frame_difference];
                graph_2_buffer_data = new Graph_Data[frame_difference];
            }

            // Get the population count for the two simulations
            int graph_1_population = database_manager.Get_Population_Count(given_simulation_id_1);
            int graph_2_population = database_manager.Get_Population_Count(given_simulation_id_2);

            int max_population = Mathf.Max(graph_1_population, graph_2_population);
            data_object.max_frame = max_frame;

            for (int i = 0; i < frame_difference; i++)
            {
                try
                {
                    if (graph_1_results[i] != null)
                    {
                        graph_1_buffer_data[i].frame_id = graph_1_results[i + min_frame][1];
                        graph_1_buffer_data[i].num_of_healthy = graph_1_results[i + min_frame][2];
                        graph_1_buffer_data[i].num_of_infected = graph_1_results[i + min_frame][3];
                        graph_1_buffer_data[i].num_of_recovered = graph_1_results[i + min_frame][4];
                        graph_1_buffer_data[i].num_of_immune = graph_1_results[i + min_frame][5];
                    }
                }
                catch
                {
                    graph_1_buffer_data[i].frame_id = 0;
                    graph_1_buffer_data[i].num_of_healthy = 0;
                    graph_1_buffer_data[i].num_of_infected = 0;
                    graph_1_buffer_data[i].num_of_recovered = 0;
                    graph_1_buffer_data[i].num_of_immune = 0;
                    //Debug.Log("Graph 1 was null at frame" + i.ToString());
                }

                try
                {
                    if (graph_2_results[i] != null)
                    {
                        graph_2_buffer_data[i].frame_id = graph_2_results[i + min_frame][1];
                        graph_2_buffer_data[i].num_of_healthy = graph_2_results[i + min_frame][2];
                        graph_2_buffer_data[i].num_of_infected = graph_2_results[i + min_frame][3];
                        graph_2_buffer_data[i].num_of_recovered = graph_2_results[i + min_frame][4];
                        graph_2_buffer_data[i].num_of_immune = graph_2_results[i + min_frame][5];
                    }
                }
                catch
                {
                    graph_2_buffer_data[i].frame_id = 0;
                    graph_2_buffer_data[i].num_of_healthy = 0;
                    graph_2_buffer_data[i].num_of_infected = 0;
                    graph_2_buffer_data[i].num_of_recovered = 0;
                    graph_2_buffer_data[i].num_of_immune = 0;
                    //Debug.Log("Graph 2 was null at frame" + i.ToString());
                }
            }

            // Creating the Buffers
            ComputeBuffer graph_1_buffer = new ComputeBuffer(graph_1_buffer_data.Length, graph_buffer_size);
            graph_1_buffer.SetData(graph_1_buffer_data);
            simulation_compute_shader.SetBuffer(2, "graph_1_buffer", graph_1_buffer);

            ComputeBuffer graph_2_buffer = new ComputeBuffer(graph_2_buffer_data.Length, graph_buffer_size);
            graph_2_buffer.SetData(graph_2_buffer_data);
            simulation_compute_shader.SetBuffer(2, "graph_2_buffer", graph_2_buffer);

            ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float) * 2);
            debug_buffer.SetData(debug_buffer_data);
            simulation_compute_shader.SetBuffer(2, "debug_buffer", debug_buffer);

            // Passing through the needed variables
            simulation_compute_shader.SetInt("texture_width", data_object.texture_width);
            simulation_compute_shader.SetInt("texture_height", data_object.texture_height);
            simulation_compute_shader.SetInt("graph_population_count", max_population);
            simulation_compute_shader.SetInt("graph_max_frame", frame_difference);
            simulation_compute_shader.SetInt("min_frame_id", min_frame);

            // Dispatching the Compute Shader
            simulation_compute_shader.Dispatch(2, 128, 8, 1);

            graph_1_buffer.GetData(graph_1_buffer_data);
            graph_1_buffer.Dispose();

            graph_2_buffer.GetData(graph_2_buffer_data);
            graph_2_buffer.Dispose();

            debug_buffer.GetData(debug_buffer_data);
            debug_buffer.Dispose();

            Debug.Log(debug_buffer_data[0].ToString());


            data_object.chosen_graph_drawn = true;
        }
        else // once the graph has been drawn
        {

            if (data_object.zoomed)
            {
                max_frame = Mathf.Max(data_object.chosen_frame_1, data_object.chosen_frame_2);
                min_frame = Mathf.Min(data_object.chosen_frame_1, data_object.chosen_frame_2);
                frame_difference = max_frame - min_frame;
            }
            else
            {
                int graph_1_max_frame = database_manager.Get_Max_Frame_ID(given_simulation_id_1);
                int graph_2_max_frame = database_manager.Get_Max_Frame_ID(given_simulation_id_2);

                max_frame = Mathf.Max(graph_1_max_frame, graph_2_max_frame);
                min_frame = 0;
                frame_difference = max_frame;
            }
            data_object.max_frame = max_frame;


            // Resetting Buffers if they become null
            if (graph_1_buffer_data == null)
            {
                graph_1_buffer_data = new Graph_Data[data_object.max_frame+1];
                graph_2_buffer_data = new Graph_Data[data_object.max_frame+1];
                List<List<int>> graph_1_results = database_manager.Get_Results_Data(given_simulation_id_1);
                List<List<int>> graph_2_results = database_manager.Get_Results_Data(given_simulation_id_2);

                for (int i = 0; i < data_object.max_frame; i++)
                {
                    Debug.Log(i);
                    try
                    {
                        if (graph_1_results[i] != null)
                        {
                            graph_1_buffer_data[i].frame_id = graph_1_results[i + min_frame][1];
                            graph_1_buffer_data[i].num_of_healthy = graph_1_results[i + min_frame][2];
                            graph_1_buffer_data[i].num_of_infected = graph_1_results[i + min_frame][3];
                            graph_1_buffer_data[i].num_of_recovered = graph_1_results[i + min_frame][4];
                            graph_1_buffer_data[i].num_of_immune = graph_1_results[i + min_frame][5];
                        }
                    }
                    catch
                    {
                        graph_1_buffer_data[i].frame_id = 0;
                        graph_1_buffer_data[i].num_of_healthy = 0;
                        graph_1_buffer_data[i].num_of_infected = 0;
                        graph_1_buffer_data[i].num_of_recovered = 0;
                        graph_1_buffer_data[i].num_of_immune = 0;
                        //Debug.Log("Graph 1 was null at frame" + i.ToString());
                    }

                    try
                    {
                        if (graph_2_results[i] != null)
                        {
                            graph_2_buffer_data[i].frame_id = graph_2_results[i + min_frame][1];
                            graph_2_buffer_data[i].num_of_healthy = graph_2_results[i + min_frame][2];
                            graph_2_buffer_data[i].num_of_infected = graph_2_results[i + min_frame][3];
                            graph_2_buffer_data[i].num_of_recovered = graph_2_results[i + min_frame][4];
                            graph_2_buffer_data[i].num_of_immune = graph_2_results[i + min_frame][5];
                        }
                    }
                    catch
                    {
                        graph_2_buffer_data[i].frame_id = 0;
                        graph_2_buffer_data[i].num_of_healthy = 0;
                        graph_2_buffer_data[i].num_of_infected = 0;
                        graph_2_buffer_data[i].num_of_recovered = 0;
                        graph_2_buffer_data[i].num_of_immune = 0;
                        //Debug.Log("Graph 2 was null");
                    }
                }
            }

            int mouse_x_position = (int)Mathf.Floor(Input.mousePosition.x);
            int width_desnity = (int)Mathf.Floor(frame_difference / data_object.texture_width);

            int selected_frame = mouse_x_position * width_desnity;

            if (selected_frame > 0 && selected_frame < frame_difference)
            {
                // Getting data for first graph
                int healthy_1 = graph_1_buffer_data[selected_frame].num_of_healthy;
                int infected_1 = graph_1_buffer_data[selected_frame].num_of_infected;
                int recovering_1 = graph_1_buffer_data[selected_frame].num_of_recovered;
                int immune_1 = graph_1_buffer_data[selected_frame].num_of_immune;

                // Getting data for the second graph
                int healthy_2 = graph_2_buffer_data[selected_frame].num_of_healthy;
                int infected_2 = graph_2_buffer_data[selected_frame].num_of_infected;
                int recovering_2 = graph_2_buffer_data[selected_frame].num_of_recovered;
                int immune_2 = graph_2_buffer_data[selected_frame].num_of_immune;

                // Passing the data to the screen UI
                gui_manager.Update_Graph_Data(healthy_1, infected_1, recovering_1, immune_1);
                gui_manager.Update_Graph_2_Data(healthy_2, infected_2, recovering_2, immune_2);


                // Calculating the R values;
                double graph_1_R_value;
                double graph_2_R_value;
                try
                {
                    graph_1_R_value = (float)graph_1_buffer_data[selected_frame + 1].num_of_infected / infected_1;
                }
                catch
                {
                    // Takes this path if trying to divide by 0
                    graph_1_R_value = 0;
                }
                try
                {
                    graph_2_R_value = (float)graph_2_buffer_data[selected_frame + 1].num_of_infected / infected_2;
                }
                catch
                {
                    // takes this path if trying to divide by 0
                    graph_2_R_value = 0;
                }
                // Passing through the R values
                gui_manager.Update_R_Values(graph_1_R_value, graph_2_R_value);
            }
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // places the result of the graphics onto the screen
        if (data_object.simulation_active)
        {
            Graphics.Blit(simulation_render_texture, destination);
        }
        else if (data_object.graph_active)
        {
            Graphics.Blit(graph_render_texture, destination);
            //data_object.graph_active = false;
        }
        else
        {
            Graphics.Blit(new RenderTexture(data_object.texture_width, data_object.texture_height, 24), destination);
        }
    }

    void UpdateData(Data_Struct[] data)
    {
        data_object.num_of_healthy = data[0].number_of_healthy;
        data_object.num_of_infected = data[0].number_of_infected;
        data_object.num_of_recovered = data[0].number_of_recovered;
        data_object.num_of_immune = data[0].number_of_immune;
    }


    public static void Add_Record_CSV(int frame_id, int number_of_healthy, int number_of_infected, int number_of_recovered, string file_path)
    {
        try
        {
            using (System.IO.StreamWriter csv_file = new System.IO.StreamWriter(@file_path, true))
            {
                csv_file.WriteLine(frame_id.ToString() + "," + number_of_healthy.ToString() + "," + number_of_infected.ToString() + "," + number_of_recovered.ToString());
                //Debug.Log(frame_id.ToString() + "," + number_of_healthy.ToString() + "," + number_of_infected.ToString() + "," + number_of_recovered.ToString());
            }
        }
        catch
        {
            Debug.Log("Application did a little wrongen");
        }
    }
}
