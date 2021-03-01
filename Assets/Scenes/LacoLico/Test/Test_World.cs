using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_World : MonoBehaviour
{
    // Start is called before the first frame update
    private const int mapSize = 100;
    public int[,,] world;

    void Start()
    {
        world = new int[mapSize, mapSize, mapSize];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateWorld(Vector3 pos, int blockID) {
        this.world[(int)pos.x, (int)pos.y, (int)pos.z] = blockID;
        drawWorld();
    }

    private void drawWorld() {
        for(int z=0; z<mapSize; z++) {
            for(int x=0; x<mapSize; x++) {
                for(int y=0; y<mapSize; y++) {
                    if(world[x,y,z] > 0) {
                        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        cube.position = new Vector3(x, y, z);
                    }
                }
            }
        }
    }
}
