using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCore : MonoBehaviour
{
    /* Public Field */
    [SerializeField]
    private Camera cam; // 블록 설치에 사용되는 카메라

    [SerializeField]
    private World world; // 상호작용할 World

    [SerializeField]
    private GameObject placeBlock;

    [SerializeField]
    private GameObject selectedBlock;

    [SerializeField]
    private float reach = 8f; // 블럭 상호작용 거리

    /* Private Field */
    private float checkIncrement = 0.1f;
    
    void Start()
    {
        // Camera 기본설정을 안했을 경우 가장 먼저 찾은 카메라로 설정
        if(cam == null)
        {
            this.SetCamera(FindObjectOfType<Camera>());
        }
        // 나중에 저장된 데이터에서 현재 캐릭터가 존재하는 월드를 받아오도록 설정
        if(world == null)
        {
            this.SetWorld(FindObjectOfType<World>());
        }
    }

    public void SetCamera(Camera _cam)
    {
        cam = _cam;
    }
    public void SetWorld(World _world)
    {
        world = _world;
    }

    public void PlaceBlock(byte blockIndex)
    {
        if (placeBlock != null && placeBlock.gameObject.activeSelf)
        {
            world.GetChunkFromVector3(placeBlock.transform.position).EditVoxel(placeBlock.transform.position, blockIndex);
        }
    }

    public void DestroyBlock()
    {
        if (selectedBlock != null && selectedBlock.gameObject.activeSelf)
        {
            world.GetChunkFromVector3(selectedBlock.transform.position).EditVoxel(selectedBlock.transform.position, 0); // Air로 대체
        }
    }

    private void placeCursorBlocks () 
    {
        float step      = checkIncrement;
        Vector3 lastPos = new Vector3();
        Transform camTrans = cam.transform;

        while (step < reach) 
        {
            Vector3 pos = camTrans.position + (camTrans.forward * step);
            if (world.CheckForVoxel(pos)) 
            {
                selectedBlock.transform.position = new Vector3
                (
                    Mathf.FloorToInt(pos.x), 
                    Mathf.FloorToInt(pos.y), 
                    Mathf.FloorToInt(pos.z)
                );
                placeBlock.transform.position = lastPos;

                selectedBlock.SetActive(true);
                placeBlock.SetActive(true);
                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }

        selectedBlock.SetActive(false);
        placeBlock.SetActive(false);
    }

    /* Update */
    private void Update() 
    {
        placeCursorBlocks();
    }
}
