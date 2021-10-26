using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateObstacle : MonoBehaviour
{
    /**** variables ****/
    public GameObject ObstaclePrefeb;
    // obstacle name
    public InputField nameField;
    // obstacle pos
    public InputField obstaclePosX;
    public InputField obstaclePosY;
    // obstacle size
    public InputField obstacleValueX;
    public InputField obstacleValueY;
    // obstacle use area on/off
    public Toggle upUse;
    public Toggle downUse;
    public Toggle leftUse;
    public Toggle rightUse;
    // obstacle use area size
    public InputField upUseSize;
    public InputField downUseSize;
    public InputField leftUseSize;
    public InputField rightUseSize;

    public Dropdown dropdown;

    public struct Obstacle
    {
        public string name;
        public GenerateRoom.Point n1, n2, n3, n4;
        public GenerateRoom.Point s1, s2, s3, s4;
        public int width; //가로
        public int length; //세로
        public int s_width; //여유공간 가로
        public int s_length; //여유공간 세로
        public int top; //여유공간 붙일 면
        public int bot;
        public int right;
        public int left;
        public int wallcount; //사용할 면 수
    }

    public Obstacle[] obs_furn; // 장애물 배열
    public int obs_num = 0;

    /**** to get drop down change ****/
    public void Start()
    {
        obs_furn = new Obstacle[100];
        dropdown.onValueChanged.AddListener(delegate {
            inputFieldOn();
        });
        obs_num = 0;
    }

    /**** input field change by drop down ****/
    public void inputFieldOn()
    {
        /* obstacle furniture */
        if (dropdown.value == 0)
        {
            nameField.text = "";
        }
        /* door - 미닫이 */
        else if (dropdown.value == 1)
        {
            nameField.text = "미닫이";
        }
        /* door - 여닫이 */
        else if (dropdown.value == 2)
        {
            nameField.text = "여닫이";
        }

    }

    public void generateObstacle()
    {

        /**** get input ****/
        // get name, size
        obs_furn[obs_num].name = nameField.text;
        // pos
        obs_furn[obs_num].n1.x = int.Parse(obstaclePosX.text);
        obs_furn[obs_num].n1.y = int.Parse(obstaclePosY.text);
        // size
        obs_furn[obs_num].width = int.Parse(obstacleValueX.text);
        obs_furn[obs_num].length = int.Parse(obstacleValueY.text);
        // get use area of furniture
        {
            if (upUse.isOn)
            {
                obs_furn[obs_num].top = int.Parse(upUseSize.text);
                obs_furn[obs_num].wallcount++;
            }
            else
            {
                obs_furn[obs_num].top = 0;
            }
            if (downUse.isOn)
            {
                obs_furn[obs_num].bot = int.Parse(downUseSize.text);
                obs_furn[obs_num].wallcount++;
            }
            else
            {
                obs_furn[obs_num].bot = 0;
            }
            if (leftUse.isOn)
            {
                obs_furn[obs_num].left = int.Parse(leftUseSize.text);
                obs_furn[obs_num].wallcount++;
            }
            else
            {
                obs_furn[obs_num].left = 0;
            }
            if (rightUse.isOn)
            {
                obs_furn[obs_num].right = int.Parse(rightUseSize.text);
                obs_furn[obs_num].wallcount++;
            }
            else
            {
                obs_furn[obs_num].right = 0;
            }
        }

        // input in console
        {
            /*
            Console.WriteLine("붙박이가구 이름 입력");
            obs_furn[obs_num].name = Console.ReadLine();
            Console.WriteLine("붙박이가구의 가로폭 입력");
            obs_furn[obs_num].width = int.Parse(Console.ReadLine());
            Console.WriteLine("붙박이가구의 세로폭 입력");
            obs_furn[obs_num].length = int.Parse(Console.ReadLine());
            Console.WriteLine("붙박이가구의 x좌표를 입력해주세요");
            obs_furn[obs_num].n1.x = int.Parse(Console.ReadLine());
            Console.WriteLine("붙박이가구의 y좌표를 입력해주세요");
            obs_furn[obs_num].n1.y = int.Parse(Console.ReadLine());
            Console.WriteLine("붙박이가구의 윗면 여유공간 입력");
            obs_furn[obs_num].top = int.Parse(Console.ReadLine());
            if (obs_furn[obs_num].top != 0) obs_furn[obs_num].wallcount++;
            Console.WriteLine("붙박이가구의 아랫면 여유공간 입력");
            obs_furn[obs_num].bot = int.Parse(Console.ReadLine());
            if (obs_furn[obs_num].bot != 0) obs_furn[obs_num].wallcount++;
            Console.WriteLine("붙박이가구의 우측면 여유공간 입력");
            obs_furn[obs_num].right = int.Parse(Console.ReadLine());
            if (obs_furn[obs_num].right != 0) obs_furn[obs_num].wallcount++;
            Console.WriteLine("붙박이가구의 좌측면 여유공간 입력");
            obs_furn[obs_num].left = int.Parse(Console.ReadLine());
            if (obs_furn[obs_num].left != 0) obs_furn[obs_num].wallcount++;
            */
        }

        /**** calculate s1, s_width, s_length ****/
        if (dropdown.value == 0)
        {
            if (obs_furn[obs_num].wallcount == 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length;
            }
            else if (obs_furn[obs_num].wallcount == 1 && obs_furn[obs_num].top != 0) //가구 사용공간 계산
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top;
            }
            else if (obs_furn[obs_num].wallcount == 1 && obs_furn[obs_num].bot != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 1 && obs_furn[obs_num].right != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length;
            }
            else if (obs_furn[obs_num].wallcount == 1 && obs_furn[obs_num].left != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length;
            }
            else if (obs_furn[obs_num].wallcount == 2 && obs_furn[obs_num].top != 0 && obs_furn[obs_num].left != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top;
            }
            else if (obs_furn[obs_num].wallcount == 2 && obs_furn[obs_num].left != 0 && obs_furn[obs_num].bot != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 2 && obs_furn[obs_num].bot != 0 && obs_furn[obs_num].right != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 2 && obs_furn[obs_num].right != 0 && obs_furn[obs_num].top != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top;
            }
            else if (obs_furn[obs_num].wallcount == 2 && obs_furn[obs_num].top != 0 && obs_furn[obs_num].bot != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 2 && obs_furn[obs_num].right != 0 && obs_furn[obs_num].left != 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length;
            }
            else if (obs_furn[obs_num].wallcount == 3 && obs_furn[obs_num].top == 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 3 && obs_furn[obs_num].bot == 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top;
            }
            else if (obs_furn[obs_num].wallcount == 3 && obs_furn[obs_num].right == 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 3 && obs_furn[obs_num].left == 0)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top + obs_furn[obs_num].bot;
            }
            else if (obs_furn[obs_num].wallcount == 4)
            {
                obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x - obs_furn[obs_num].width;
                obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y - obs_furn[obs_num].length;
                obs_furn[obs_num].s_width = obs_furn[obs_num].width + obs_furn[obs_num].right + obs_furn[obs_num].left;
                obs_furn[obs_num].s_length = obs_furn[obs_num].length + obs_furn[obs_num].top + obs_furn[obs_num].bot;
            }


        }
        // door - 미닫이
        else if (dropdown.value == 1)
        {
            obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
            obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
            obs_furn[obs_num].s_width = obs_furn[obs_num].width;
            obs_furn[obs_num].s_length = obs_furn[obs_num].width;

        }
        // door - 여닫이 
        else if (dropdown.value == 2)
        {
            obs_furn[obs_num].s1.x = obs_furn[obs_num].n1.x;
            obs_furn[obs_num].s1.y = obs_furn[obs_num].n1.y;
            obs_furn[obs_num].s_width = obs_furn[obs_num].width;
            obs_furn[obs_num].s_length = obs_furn[obs_num].width;

        }


        /**** prerender obstacle ****/
        {
            // position(x, y)
            Vector3 position = new Vector3((float)obs_furn[obs_num].n1.x / 20 + (float)obs_furn[obs_num].width / 40, (float)obs_furn[obs_num].n1.y / 20 + (float)obs_furn[obs_num].length / 40, -0.01f);

            // size (height, width) 
            Vector3 scaleChange = new Vector3((float)obs_furn[obs_num].width / 20, (float)obs_furn[obs_num].length / 20, 0.1f);
            ObstaclePrefeb.transform.localScale = scaleChange;

            // direction
            Quaternion rotation = new Quaternion(0, 0, 0, 0);

            //generate obstacle
            GameObject newObstacle = Instantiate(ObstaclePrefeb, position, rotation) as GameObject;
            Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();

            // obstacle background color : white
            obstacleRenderer.material.shader = Shader.Find("Standard");
            obstacleRenderer.material.SetColor("_Color", new Color(0.2f, 0.2f, 0.2f));

            obs_num++;
        }
        /**** empty text field ****/
        /* obstacle furniture */
        if (dropdown.value == 0)
        {
            nameField.text = "";
        }
        /* door - 미닫이 */
        else if (dropdown.value == 1)
        {
            nameField.text = "미닫이";
        }
        /* door - 여닫이 */
        else if (dropdown.value == 2)
        {
            nameField.text = "여닫이";
        }
        // obstacle pos
        obstaclePosX.text = "";
        obstaclePosY.text = "";
        // obstacle size
        obstacleValueX.text = "";
        obstacleValueY.text = "";
        upUseSize.text = "";
        downUseSize.text = "";
        leftUseSize.text = "";
        rightUseSize.text = "";

    }
}

