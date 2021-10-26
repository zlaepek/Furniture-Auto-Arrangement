using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMultipleFuniture : MonoBehaviour
{
    /**** to link variables in other class ****/
    public program _program;

    /**** variables ****/
    public GameObject furniture;
    private List<GameObject> objList;
        // furniture name
    public InputField nameField;
        // furniture size
    public InputField furnitureValueX;
    public InputField furnitureValueY;
        // furniture use area on/off
    public Toggle upUse;
    public Toggle downUse;
    public Toggle leftUse;
    public Toggle rightUse;
        // furniture use area size
    public InputField upUseSize;
    public InputField downUseSize;
    public InputField leftUseSize;
    public InputField rightUseSize;

    public int list_index;

    /**** furniture struct ****/
    public struct Furniture
    {
        public string name;
        public GenerateRoom.Point n1, n2, n3, n4;
        public GenerateRoom.Point s1, s2, s3, s4;
        public int width; //가로
        public int length; //세로
        public int area;
        public int s_width; //여유공간 가로
        public int s_length; //여유공간 세로
        public int top; //여유공간 붙일 면
        public int bot;
        public int right;
        public int left;
        public int wallcount; //사용할 면 수
    }

    /**** furniture info array ****/
    public Furniture[] fun = new Furniture[100];
    
    public int Furniture_index = 0;
    public int min = 1000000;

    List<List<GameObject>> mylist = new List<List<GameObject>>();


    /* to generate furniture */
    private float widthMin, widthMax;
    private float heightMin, heightMax;

    /**** make input ****/
    public void GenerateNewFurniture() {
        /**** get input ****/
        {
            // get name, size
            fun[Furniture_index].name = nameField.text;
            fun[Furniture_index].width = int.Parse(furnitureValueX.text);
            fun[Furniture_index].length = int.Parse(furnitureValueY.text);

            // get use area of furniture
            if (upUse.isOn)
            {
                fun[Furniture_index].top = int.Parse(upUseSize.text);
            }
            if (downUse.isOn)
            {
                fun[Furniture_index].bot = int.Parse(downUseSize.text);
            }
            if (leftUse.isOn)
            {
                fun[Furniture_index].left = int.Parse(leftUseSize.text);
            }
            if (rightUse.isOn)
            {
                fun[Furniture_index].right = int.Parse(rightUseSize.text);
            }
            

            // hide console
            {
                /*
                    Console.WriteLine("가구 이름 입력");
                    fun[Furniture_index].name = Console.ReadLine();
                    Console.WriteLine("가구의 가로폭 입력");
                    fun[Furniture_index].width = int.Parse(Console.ReadLine());
                    Console.WriteLine("가구의 세로폭 입력");
                    fun[Furniture_index].length = int.Parse(Console.ReadLine());
                */
            }

            /**** min 어디다 쓰는지 모르겠슴 ****/
            // 탐색 폭 최소 단위 구하기
            {

                if (min > fun[Furniture_index].width)
                {
                    min = fun[Furniture_index].width;
                }
                if (min > fun[Furniture_index].length)
                {
                    min = fun[Furniture_index].length;
                }
            }
            // hide console
            {
                /*
                Console.WriteLine("가구의 윗면 여유공간 입력");
                fun[Furniture_index].top = int.Parse(Console.ReadLine());
                if (fun[Furniture_index].top != 0) fun[Furniture_index].wallcount++;
                Console.WriteLine("가구의 아랫면 여유공간 입력");
                fun[Furniture_index].bot = int.Parse(Console.ReadLine());
                if (fun[Furniture_index].bot != 0) fun[Furniture_index].wallcount++;
                Console.WriteLine("가구의 우측면 여유공간 입력");
                fun[Furniture_index].right = int.Parse(Console.ReadLine());
                if (fun[Furniture_index].right != 0) fun[Furniture_index].wallcount++;
                Console.WriteLine("가구의 좌측면 여유공간 입력");
                fun[Furniture_index].left = int.Parse(Console.ReadLine());
                if (fun[Furniture_index].left != 0) fun[Furniture_index].wallcount++;
                */
            }
        }
        /**** use space calculate ****/
        {
            if (fun[Furniture_index].wallcount == 1 && fun[Furniture_index].top != 0) //가구 사용공간 계산
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 1 && fun[Furniture_index].bot != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 1 && fun[Furniture_index].right != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right;
                fun[Furniture_index].s_length = fun[Furniture_index].length;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 1 && fun[Furniture_index].left != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 2 && fun[Furniture_index].top != 0 && fun[Furniture_index].left != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 2 && fun[Furniture_index].left != 0 && fun[Furniture_index].bot != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 2 && fun[Furniture_index].bot != 0 && fun[Furniture_index].right != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 2 && fun[Furniture_index].right != 0 && fun[Furniture_index].top != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 2 && fun[Furniture_index].top != 0 && fun[Furniture_index].bot != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 2 && fun[Furniture_index].right != 0 && fun[Furniture_index].left != 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 3 && fun[Furniture_index].top == 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 3 && fun[Furniture_index].bot == 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 3 && fun[Furniture_index].right == 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 3 && fun[Furniture_index].left == 0)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
            else if (fun[Furniture_index].wallcount == 4)
            {
                fun[Furniture_index].s1.x = fun[Furniture_index].n1.x - fun[Furniture_index].width;
                fun[Furniture_index].s1.y = fun[Furniture_index].n1.y - fun[Furniture_index].length;
                fun[Furniture_index].s_width = fun[Furniture_index].width + fun[Furniture_index].right + fun[Furniture_index].left;
                fun[Furniture_index].s_length = fun[Furniture_index].length + fun[Furniture_index].top + fun[Furniture_index].bot;
                fun[Furniture_index].area = fun[Furniture_index].s_width * fun[Furniture_index].s_length;
            }
        }
        /**** empty text field ****/
        Debug.Log(Furniture_index);
        Furniture_index++;
        nameField.text = "";
        // furniture size
        furnitureValueX.text = "";
        furnitureValueY.text = "";
        upUseSize.text = "";
        downUseSize.text = "";
        leftUseSize.text = "";
        rightUseSize.text = "";
    }

    public void renderFurnitureList()
    {
        {
            //for (int x = 0; x < _program.saveCase_num; x++)
            //{
            //    mylist.Add(new List<GameObject>());
            //    for (int y = 0; y < Furniture_index; y++)
            //    {
            //        widthMin = Mathf.Min((float)_program.s_fun[x, y].n1.x, (float)_program.s_fun[x, y].n2.x, (float)_program.s_fun[x, y].n3.x, (float)_program.s_fun[x, y].n4.x);
            //        widthMax = Mathf.Max((float)_program.s_fun[x, y].n1.x, (float)_program.s_fun[x, y].n2.x, (float)_program.s_fun[x, y].n3.x, (float)_program.s_fun[x, y].n4.x);
            //        heightMin = Mathf.Min((float)_program.s_fun[x, y].n1.y, (float)_program.s_fun[x, y].n2.y, (float)_program.s_fun[x, y].n3.y, (float)_program.s_fun[x, y].n4.y);
            //        heightMax = Mathf.Max((float)_program.s_fun[x, y].n1.y, (float)_program.s_fun[x, y].n2.y, (float)_program.s_fun[x, y].n3.y, (float)_program.s_fun[x, y].n4.y);
            //        /* render */ 
            //        // position(x, y)
            //        Vector3 position = new Vector3((widthMax + widthMin) / 40 , (heightMax + heightMin) / 40, -0.01f);

            //        // direction
            //        Quaternion rotation = new Quaternion(1, 1, 1, 1);

            //        // generate furniture
            //        GameObject newFurniture = Instantiate(furniture, position, rotation) as GameObject;
            //        mylist[x].Add(newFurniture);
            //        Debug.Log("x,y"+x+y);

            //        // size (height, width)

            //        Vector3 scaleChange = new Vector3((widthMax - widthMin) / 20, (heightMax - heightMin) / 20, 0.1f);
            //        furniture.transform.localScale = scaleChange;

            //        Renderer furnitureRenderer = newFurniture.GetComponent<Renderer>();

            //        // random color
            //        furnitureRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1.0f, 1.0f, 0.7f, 0.7f, 0.5f, 0.5f));

            //        // shader + transparency
            //        furnitureRenderer.material.shader = Shader.Find("Transparent/Diffuse");
            //    }
            //}
        }
        for (int x = 0; x < _program.saveCase_num; x++)
        {
            mylist.Add(new List<GameObject>());
            for (int y = 0; y < _program.fun_num; y++)
            {
                widthMin = Mathf.Min((float)_program.s_fun[x, y].n1.x, (float)_program.s_fun[x, y].n2.x, (float)_program.s_fun[x, y].n3.x, (float)_program.s_fun[x, y].n4.x);
                widthMax = Mathf.Max((float)_program.s_fun[x, y].n1.x, (float)_program.s_fun[x, y].n2.x, (float)_program.s_fun[x, y].n3.x, (float)_program.s_fun[x, y].n4.x);
                heightMin = Mathf.Min((float)_program.s_fun[x, y].n1.y, (float)_program.s_fun[x, y].n2.y, (float)_program.s_fun[x, y].n3.y, (float)_program.s_fun[x, y].n4.y);
                heightMax = Mathf.Max((float)_program.s_fun[x, y].n1.y, (float)_program.s_fun[x, y].n2.y, (float)_program.s_fun[x, y].n3.y, (float)_program.s_fun[x, y].n4.y);
                /* render */
                // position(x, y)
                Vector3 position = new Vector3((widthMax + widthMin) / 40, (heightMax + heightMin) / 40, -0.01f);
                Debug.Log("P" + (widthMax + widthMin) + " " + (heightMax + heightMin));
                // direction
                Quaternion rotation = new Quaternion(0, 0, 0, 0);

                // generate furniture
                GameObject newFurniture = Instantiate(furniture, position, rotation) as GameObject;
                newFurniture.name = x + "_" + y;
                newFurniture.SetActive(false);
                mylist[x].Add(newFurniture);
                Debug.Log("x,y" + x + y);

                // size (height, width)
                Vector3 scaleChange = new Vector3(((float)widthMax - widthMin) / 20, ((float)heightMax - heightMin) / 20, 0.1f);
                furniture.transform.localScale = scaleChange;

                Renderer furnitureRenderer = newFurniture.GetComponent<Renderer>();

                // random color
                furnitureRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1.0f, 1.0f, 0.7f, 0.7f, 0.5f, 0.5f));

                // shader + transparency
                furnitureRenderer.material.shader = Shader.Find("Transparent/Diffuse");
            }
        }
        show_furniture_list();
    }

    public void show_furniture_list()
    {
        for (int i = 0; i < _program.saveCase_num; i++)
        {
            mylist[list_index][i].SetActive(true);
            if (list_index > 0)
            {
                mylist[list_index - 1][i].SetActive(false);
            }
            if (list_index < _program.saveCase_num)
            {
                mylist[list_index + 1][i].SetActive(false);
            }
            
        }
    }
}
