using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateRoom : MonoBehaviour
{
    /* variables */
    public InputField roomValueX;
    public InputField roomValueY;
    public InputField roomPosX;
    public InputField roomPosY;

    public Toggle upWall;
    public Toggle downWall;
    public Toggle leftWall;
    public Toggle rightWall;

    public GameObject roomObj;
    public List<GameObject> roomList = new List<GameObject>();

    public int roomIndex = -1;

    public struct Room
    {
        public int width; //방 사이즈
        public int length;
        //public char[,] room_status; // 방의 가구 상태정보 이차원 배열로 핸들링
        public int wall_length;
    }

    public struct Point
    {
        public int x;
        public int y;
    }
    public Room room = new Room();
    public Point roomPoint = new Point();

    public void generateRoom() {
        
        /* get input */
        room.width = int.Parse(roomValueX.text);
        room.length = int.Parse(roomValueY.text);

        roomPoint.x = 0/*int.roomPoint.x*/;
        roomPoint.y = 0/*int.roomPoint.y*/;

        /* draw room */
        // position(x, y) 
        Vector3 position = new Vector3((float)roomPoint.x / 20 + (float)room.width / 40, (float)roomPoint.y / 20 + (float)room.length / 40, 0);
        // size (height, width)
        Vector3 scaleChange = new Vector3((float)room.width / 20, (float)room.length/ 20, 0.1f);
        roomObj.transform.localScale = scaleChange;
        // direction 
        Quaternion rotation = new Quaternion(0, 0, 0, 0);

        // generate room
        roomList.Add(Instantiate(roomObj, position, rotation) as GameObject);
        roomIndex++;
        Renderer roomRenderer = roomList[roomIndex].GetComponent<Renderer>();

        /* notice position */
        //GameObject text = new GameObject();
        //TextMesh t = text.AddComponent<TextMesh>();
        //t.text = "new text set";
        //t.fontSize = 30;
        //t.transform.localPosition += new Vector3(56f, 3f, 40f);

        // room background color : white 
        roomRenderer.material.shader = Shader.Find("Standard");
        roomRenderer.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f));

        /* generate wall */
        if (upWall.isOn)
        {
            position = new Vector3((float)roomPoint.x / 20 + (float)room.width / 40, 
                (float)roomPoint.y / 20 + float.Parse(roomValueY.text) / 40 + (float)room.length / 40, 0.01f);
            scaleChange = new Vector3(float.Parse(roomValueX.text) / 20 + 0.2f, 0.2f, 0.1f);
            roomObj.transform.localScale = scaleChange;
            rotation = new Quaternion(0, 0, 0, 0);
            GameObject newWall = Instantiate(roomObj, position, rotation) as GameObject;
            Renderer wallRenderer = newWall.GetComponent<Renderer>();

            wallRenderer.material.shader = Shader.Find("Standard");
            wallRenderer.material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f));
        }
        if (downWall.isOn)
        {
            position = new Vector3((float)roomPoint.x / 20 + (float)room.width / 40, 
                (float)roomPoint.y / 20 - float.Parse(roomValueY.text) / 40 + (float)room.length / 40, 0.01f);
            scaleChange = new Vector3(float.Parse(roomValueX.text) / 20 + 0.2f, 0.2f, 0.1f);
            roomObj.transform.localScale = scaleChange;
            rotation = new Quaternion(0, 0, 0, 0);
            GameObject newWall = Instantiate(roomObj, position, rotation) as GameObject;
            Renderer wallRenderer = newWall.GetComponent<Renderer>();

            wallRenderer.material.shader = Shader.Find("Standard");
            wallRenderer.material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f));
        }
        if (leftWall.isOn)
        {
            position = new Vector3((float)roomPoint.x / 20 - float.Parse(roomValueX.text) / 40 + (float)room.width / 40, 
                (float)roomPoint.y / 20 + (float)room.length / 40, 0.01f);
            scaleChange = new Vector3(0.2f, float.Parse(roomValueY.text) / 20 + 0.2f,  0.1f);
            roomObj.transform.localScale = scaleChange;
            rotation = new Quaternion(0, 0, 0, 0);
            GameObject newWall = Instantiate(roomObj, position, rotation) as GameObject;
            Renderer wallRenderer = newWall.GetComponent<Renderer>();

            wallRenderer.material.shader = Shader.Find("Standard");
            wallRenderer.material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f));
        }
        if (rightWall.isOn)
        {
            position = new Vector3((float)roomPoint.x / 20 + float.Parse(roomValueX.text) / 40 + (float)room.width / 40, 
                (float)roomPoint.y / 20 + (float)room.length / 40, 0.01f);
            scaleChange = new Vector3(0.2f, float.Parse(roomValueY.text) / 20 + 0.2f, 0.1f);
            roomObj.transform.localScale = scaleChange;
            rotation = new Quaternion(0, 0, 0, 0);
            GameObject newWall = Instantiate(roomObj, position, rotation) as GameObject;
            Renderer wallRenderer = newWall.GetComponent<Renderer>();

            wallRenderer.material.shader = Shader.Find("Standard");
            wallRenderer.material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f));
        }
        Debug.Log(room.width + room.length);
    }
}