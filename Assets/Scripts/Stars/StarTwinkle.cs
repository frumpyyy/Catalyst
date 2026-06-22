using NUnit.Framework.Constraints;
using System.Runtime.InteropServices;
using UnityEngine;

public class StarTwinkle : MonoBehaviour
{
    public ComputeShader starTwinkleCS;
    public Material starMat;

    private ComputeBuffer starTwinkleBuffer;

    public int starCount = 2000;
    const int threadCount = 256;

    [StructLayout(LayoutKind.Sequential)]
    public struct GPU_Star
    {
        public Vector2 position;
        public float brightness;
        public float speed;
        public float phase;
    }


    private ParticleSystem _starParticles;
    private ParticleSystem.Particle[] _stars;

    private int starKernel;

    private void Start()
    {
        GPU_Star[] stars = new GPU_Star[starCount];

        for (int i = 0; i < starCount; ++i)
        {
            stars[i].position = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-6.0f, 6.0f));
            stars[i].brightness = Random.value;
            stars[i].speed = Random.Range(0.1f, 1.0f);
            stars[i].phase = Random.value * 10.0f;
        }

        int stride = Marshal.SizeOf<GPU_Star>();

        starTwinkleBuffer = new ComputeBuffer(starCount, stride);

        starTwinkleBuffer.SetData(stars);

        starMat.SetBuffer("stars", starTwinkleBuffer);

        starKernel = starTwinkleCS.FindKernel("CSMain");
    }

    private void Update()
    {

        starTwinkleCS.SetBuffer(starKernel, "stars", starTwinkleBuffer);
        starTwinkleCS.SetFloat("time", Time.time);
        starTwinkleCS.SetFloat("dt", Time.deltaTime);
        starTwinkleCS.SetInt("starCount", starCount);

        int starGroups = Mathf.CeilToInt(starCount / (float)threadCount);

        starTwinkleCS.Dispatch(starKernel, starGroups, 1, 1);

        Debug.Log("drawing");
        starMat.SetBuffer("stars", starTwinkleBuffer);
        starMat.SetPass(0);

        Graphics.DrawProcedural(starMat, new Bounds(Vector3.zero, new Vector3(100, 100, 100)), MeshTopology.Triangles, starCount * 6);
    }



    private void OnDestroy()
    {
        starTwinkleBuffer?.Release();
    }
}
