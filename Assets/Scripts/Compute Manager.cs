using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader compute_shader;
    [SerializeField]
    private RenderTexture render_texture;


    public int texture_width;
    public int texture_height;


    public struct Person
    {
        public Vector2 position;
        public float speed_percentage;
        public int health_state;
    }

    public int population_count;
    private int buffer_size; // size of each person in bits
    private Person[] buffer_data; // array to save population data to

    // Start is called before the first frame update
    void Start()
    {
        render_texture = new RenderTexture(texture_width, texture_height, 24);
        render_texture.enableRandomWrite = true;
        render_texture.Create();

        // Passes render_texture to the compute shader under the variable name Result
        compute_shader.SetTexture(0, "Result", render_texture);

        // Compute Buffer to hold data being sent to GPU
        buffer_size = sizeof(float) * 3 + sizeof(int);
        buffer_data = new Person[population_count];

        // create the population with some random values
        for(int i = 0; i < population_count; i++)
        {
            buffer_data[i].position = new Vector2(Random.Range(0, texture_width), Random.Range(0, texture_height));
            buffer_data[i].speed_percentage = (Random.Range(50, 100) / 100);
            buffer_data[i].health_state = 0;
        }

        Debug.Log(buffer_data);
        

        ComputeBuffer buffer = new ComputeBuffer(buffer_data.Length, buffer_size);
        buffer.SetData(buffer_data);

        compute_shader.SetBuffer(0, "buffer", buffer);

        // Allows the Compute Shader to run, with the needed threads
        compute_shader.Dispatch(0, population_count/64 , 1 , 1);


        buffer.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // places the result of the graphics onto the screen
        Graphics.Blit(render_texture, destination);
    }
}
