using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    public GameObject roomPanel;
    public GameObject obstaclePanel;
    public GameObject furniturePanel;

    public GameObject buttonleft;
    public GameObject buttonrjght;

    public program _program;
    public GenerateMultipleFuniture _generateMultipleFuniture;

    // public int wid = 10, hei = 20;
    // public int x, y;

    /* panel change at the end of task */
    public void endRoom()
    {
        roomPanel.gameObject.SetActive(false);
        obstaclePanel.gameObject.SetActive(true);
    }

    public void endObstacle()
    {
        obstaclePanel.gameObject.SetActive(false);
        furniturePanel.gameObject.SetActive(true);
    }

    public void endFurniture()
    {
        furniturePanel.gameObject.SetActive(false);
        buttonleft.gameObject.SetActive(true);
        buttonrjght.gameObject.SetActive(true);

        
        //_program.s_fun = new program.save_fun[200, 200];

        //for (x = 0; x < 5; x++)
        //{
        //    wid = 10;
        //    for (y = 0; y < 4; y++)
        //    {
        //        _program.s_fun[x, y].n1.x = wid;
        //        _program.s_fun[x, y].n2.x = wid;
        //        _program.s_fun[x, y].n3.x = wid + 8;
        //        _program.s_fun[x, y].n4.x = wid + 8;

        //        _program.s_fun[x, y].n1.y = hei;
        //        _program.s_fun[x, y].n2.y = hei;
        //        _program.s_fun[x, y].n3.y = hei + 5;
        //        _program.s_fun[x, y].n4.y = hei + 5;
        //        wid = wid + 10;
        //    }
        //    hei = hei + 20;
        //}

        _program.mainFunction();
        _generateMultipleFuniture.renderFurnitureList();

    }

    public void rightListRender()
    {
        _generateMultipleFuniture.list_index++;
        _generateMultipleFuniture.show_furniture_list();
    }

    public void leftListRender()
    {
        _generateMultipleFuniture.list_index--;
        _generateMultipleFuniture.show_furniture_list();
    }

    public void endProgram()
    {
        Application.Quit();
    }
}
