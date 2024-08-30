using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader compute_shader;
    [SerializeField]
    private RenderTexture render_texture;

    // DataObject that holds all the needed data
    public DataObject data_object;

    // Struct that is sent through to the compute buffer
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
    }

   
    private int buffer_size; // size of each person in bits
    private Person[] buffer_data; // array to save population data to

    private int data_buffer_size; // sizee of Data_Struct in bits
    private Data_Struct[] data_buffer_data;


    private Vector2[] debug_buffer_data;

    public DatabaseManager database_manager;

    // Start is called before the first frame update
    void Start()
    {

        data_object.frame_id = 0;

        // a new texture is creates and we enable RandomWrite so the texture can be modified
        render_texture = new RenderTexture(data_object.texture_width, data_object.texture_height, 24);
        render_texture.enableRandomWrite = true;
        render_texture.Create();

        // Passes render_texture to the compute shader under the variable name Result
        compute_shader.SetTexture(0, "Result", render_texture);



        // Compute Buffer to hold data being sent to GPU
        buffer_size = sizeof(float) * 6 + sizeof(int);
        buffer_data = new Person[data_object.population_count];

        debug_buffer_data = new Vector2[data_object.population_count];

        data_buffer_size = sizeof(int) * 3;
        data_buffer_data = new Data_Struct[1]; // only one instance as there is no need for more counters.

        // create the population with some random values
        for (int i = 0; i < data_object.population_count; i++)
        {
            // Gives each new point a random staring position within the texture bounds
            buffer_data[i].position = new Vector2(Random.Range(0, data_object.texture_width), Random.Range(0, data_object.texture_height));
            // Gives each new point a random target position within the texture bounds
            buffer_data[i].target_position = new Vector2(Random.Range(0, data_object.texture_width), Random.Range(0, data_object.texture_height));
            // Gives each point a speed percentage that creates variance
            buffer_data[i].speed_percentage = Random.Range(25, 100);
            // fills the remaining time values with the max time. 
            buffer_data[i].remaining_time = data_object.infection_time.y;

            // creates initial infected person
            if( i == 0)
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
        }

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
        compute_shader.SetBuffer(0, "buffer", buffer);
        compute_shader.SetBuffer(0, "data_buffer", data_buffer);

        compute_shader.SetFloat("global_speed", data_object.global_speed);
        compute_shader.SetFloat("min_distance", data_object.min_distance);
        compute_shader.SetFloat("PI", Mathf.PI);
        compute_shader.SetInt("texture_width", data_object.texture_width);
        compute_shader.SetInt("texture_height", data_object.texture_height);
        compute_shader.SetFloat("radius", data_object.radius);
        compute_shader.SetInt("number_of_sensors", data_object.number_of_sensors);
        compute_shader.SetBool("show_sensors", data_object.show_sensors);
        compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);
        compute_shader.SetFloat("deltatime", Time.deltaTime);
        compute_shader.SetFloat("min_infectious_time", data_object.infection_time.x);
        compute_shader.SetFloat("max_infectious_time", data_object.infection_time.y);
        compute_shader.SetFloat("min_recovering_time", data_object.recovering_time.x);
        compute_shader.SetFloat("max_recovering_time", data_object.recovering_time.y);
        compute_shader.SetBool("is_recursive", data_object.is_reccuring);

        // Allows the Compute Shader to run, with the needed threads
        compute_shader.Dispatch(0, 1024, 1, 1);

        // retrieves the processed data from the buffer then deleting the buffer
        buffer.GetData(buffer_data);
        buffer.Dispose();

        data_buffer.GetData(data_buffer_data);
        data_buffer.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        // clears the texture so there are no trails
        render_texture.Release();
        compute_shader.SetTexture(0, "Result", render_texture);

        // creates a new buffer and fills it with the data from the buffer_data array
        ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
        buffer.SetData(buffer_data);
        compute_shader.SetBuffer(0, "buffer", buffer);

        // another buffer for debugging
        ComputeBuffer debug_buffer = new ComputeBuffer(debug_buffer_data.Length, sizeof(float)*2);
        debug_buffer.SetData(debug_buffer_data);
        compute_shader.SetBuffer(0, "debug_buffer", debug_buffer);

        // Compute Buffer for the Data retrived
        ComputeBuffer data_buffer = new ComputeBuffer(data_buffer_data.Length, data_buffer_size);
        data_buffer.SetData(data_buffer_data);
        compute_shader.SetBuffer(0, "data_buffer", data_buffer);

        // passes through the global variables for the Compute shader to use.
        compute_shader.SetFloat("global_speed", data_object.global_speed);
        compute_shader.SetFloat("min_distance", data_object.min_distance);
        compute_shader.SetFloat("PI", Mathf.PI);
        compute_shader.SetInt("texture_width", data_object.texture_width);
        compute_shader.SetInt("texture_height", data_object.texture_height);
        compute_shader.SetFloat("radius", data_object.radius);
        compute_shader.SetInt("number_of_sensors", data_object.number_of_sensors);
        compute_shader.SetBool("show_sensors", data_object.show_sensors);
        compute_shader.SetFloat("deltatime", Time.deltaTime);
        compute_shader.SetFloat("min_infectious_time", data_object.infection_time.x);
        compute_shader.SetFloat("max_infectious_time", data_object.infection_time.y);
        compute_shader.SetFloat("min_recovering_time", data_object.recovering_time.x);
        compute_shader.SetFloat("max_recovering_time", data_object.recovering_time.y);
        compute_shader.SetBool("is_reccursive", data_object.is_reccuring);

        compute_shader.Dispatch(0, 1024, 1, 1);

        // retrieves data from the compute buffers and then deletes the buffers.
        buffer.GetData(buffer_data);
        debug_buffer.GetData(debug_buffer_data);

        //Debug.Log(debug_buffer_data[0]);

        buffer.Dispose();
        debug_buffer.Dispose();

        data_buffer.GetData(data_buffer_data);
        data_buffer.Dispose();
        UpdateData(data_buffer_data);

        database_manager.Save_Simulation_Results();


        Add_Record_CSV(data_object.frame_id, data_object.num_of_healthy, data_object.num_of_infected, data_object.num_of_recovered, "C:\\Users\\willb\\Unity Projects\\Computer Science Coursework - Epidemic Spread Simulation\\Assets\\Results\\Results.csv");
        data_object.frame_id++;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // places the result of the graphics onto the screen
        Graphics.Blit(render_texture, destination);
    }

    void UpdateData(Data_Struct[] data)
    {
        data_object.num_of_healthy = data[0].number_of_healthy;
        data_object.num_of_infected = data[0].number_of_infected;
        data_object.num_of_recovered = data[0].number_of_recovered;
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
