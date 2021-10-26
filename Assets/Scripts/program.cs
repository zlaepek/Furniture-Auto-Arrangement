using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class program : MonoBehaviour
{
    /**** to link variables in other class ****/
    public GenerateObstacle _generateObstacle;
    public GenerateRoom _generateRoom;
    public GenerateMultipleFuniture _generateMultipleFuniture;

    /**** struct ****/
    //public struct Room
    //{
    //    public int width; //방 사이즈
    //    public int length;
    //    public int wall_length;
    //}

    // 탐색 정점에 대한 4개 점
    public struct Point_min
    {
        public GenerateRoom.Point n1, n2, n3, n4;
    }
    public struct Point2
    {
        public GenerateRoom.Point x1;
        public GenerateRoom.Point x2;
        public GenerateRoom.Point x3;
        public GenerateRoom.Point x4;
    }



    // 경우의 수가 하나 완성될 때마다 이중배열로 저장할 배열의 구조체 타입

    public struct save_fun
    {
        public GenerateRoom.Point n1, n2, n3, n4; // (x,y)
    }


    // 전역 변수 정의
    public int obs_num, obs_i = 0, fun_num, fun_i = 0;
    public int deploy_num = 0, funX = 0, funY = 0, saveCase_num = 0, min;
    public Point_min[] min_point;     // 탐색 포인터 점(최하단)
    public GenerateObstacle.Obstacle[] obs_furn;     // 장애물 배열
    public GenerateObstacle.Obstacle[] door;         // 문 배열
    public GenerateMultipleFuniture.Furniture[] fun;         // 가구 배열 
    public save_fun[,] s_fun;       // 경우의 수 저장할 2차원 배열
    public GenerateRoom.Room room;               // 방 배열
    public int[] saveJ;             // j의 상태값 저장할 배열
    public Point2[,] InC;           // 가구의 여유공간 포함 배열  
    public Point2[,] ToU;           // 가구의 오리지널 크기 배열
    public int change = 0;
    public int move_i = 0;


    // 배치 알고리즘 메소드
    // input : 현재 가구 인덱스 값, 이동(1) 또는 이동안하기(0)
    // output : 1이면 함수 종료
    public  void deploy_fun()
    {
        try
        {
            while (true)
            {
                // 이동
                if (change == 1)
                {
                    pointMove(move_i, saveJ[move_i], min_point[move_i].n1);
                    Console.WriteLine("좌표이동 성공");
                    Console.WriteLine("현재 i 값은 : {0}", move_i);
                }

                // 한바퀴 다 돌았는지 확인
                if (min_point[move_i].n1.x == -1)
                {
                    if (move_i == 0 && saveJ[move_i] == 1)
                    {
                        Console.WriteLine("배치 종료 성공");
                        Console.WriteLine("현재 i 값은 : {0}", move_i);

                        break; // 함수 아예 종료
                    }
                    else if (saveJ[move_i] == 0)
                    {

                        min_point[move_i].n1.x = 0;
                        min_point[move_i].n1.y = 0;
                        saveJ[move_i]++;
                        change = 1;
                        Console.WriteLine("가구 모양 바꾸기 성공");
                        Console.WriteLine("현재 i 값은 : {0}", move_i);

                        continue;
                    }
                    else if (saveJ[move_i] == 1)
                    {
                        min_point[move_i].n1.x = 0;
                        min_point[move_i].n1.y = 0;
                        saveJ[move_i]--;
                        change = 1;
                        move_i -= 1;
                        Console.WriteLine("한단계 아래 가구로 내려가기 성공");
                        Console.WriteLine("현재 i 값은 : {0}", move_i);

                        continue;
                    }
                }

                // 배치 가능여부 체크함수 호출
                int check = match_check(move_i); // 추가적으로 현재 가구 배열의 인덱스 인자값 필요
                Console.WriteLine("배치 가능여부 체크함수 다녀옴");
                Console.WriteLine("현재 i 값은 : {0}", move_i);

                if (check == 1)
                { // 배치가 가능하다면!
                    if (move_i == fun_num - 1)
                    {
                        Console.WriteLine("가구배열방법 1개 나옴 저장함");
                        Console.WriteLine("현재 i 값은 : {0}", move_i);

                        // 가구 배열 저장
                        for (int z = 0; z < fun_num; z++)
                        {
                            s_fun[saveCase_num, z].n1.x = ToU[z, saveJ[z]].x1.x;
                            s_fun[saveCase_num, z].n1.y = ToU[z, saveJ[z]].x1.y;

                            s_fun[saveCase_num, z].n2.x = ToU[z, saveJ[z]].x2.x;
                            s_fun[saveCase_num, z].n2.y = ToU[z, saveJ[z]].x2.y;

                            s_fun[saveCase_num, z].n3.x = ToU[z, saveJ[z]].x3.x;
                            s_fun[saveCase_num, z].n3.y = ToU[z, saveJ[z]].x3.y;

                            s_fun[saveCase_num, z].n4.x = ToU[z, saveJ[z]].x4.x;
                            s_fun[saveCase_num, z].n4.y = ToU[z, saveJ[z]].x4.y;
                        }
                        saveCase_num++;
                        change = 1;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("배치 저장 후 다음 가구로 이동");
                        Console.WriteLine("현재 i 값은 : {0}", move_i);

                        change = 0;
                        move_i++;
                        continue;

                    }
                }
                else // 배치 불가
                {
                    Console.WriteLine("배치 불가 다음 좌표로 이동");
                    Console.WriteLine("현재 i 값은 : {0}", move_i);

                    change = 1;
                    continue;
                }

            }
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine(e.Message);
        }

    }

    // 장애물들과 겹치지 않는 체크하는 메소드
    // 가구 배열을 돌리고 있을때 돌리고 있는 배열의 인덱스를 넘겨받아야함. 그래야 해당 가구의 width 또는 length만큼 체크할 수 있어서?
    // input : 현재 돌리고 있는 가구 인덱스
    // output : 배치가능하면 1, 불가능하면 0 return
     int match_check(int i)
    {
        // 나머지 점 4개에 대한 정보 갱신
        // n1: 왼쪽 아래, n2 : 왼쪽 위, n3 : 오른쪽 아래, n4 : 오른쪽 위

        min_point[i].n2.x = InC[i, saveJ[i]].x2.x;
        min_point[i].n2.y = InC[i, saveJ[i]].x2.y;
        min_point[i].n3.x = InC[i, saveJ[i]].x3.x;
        min_point[i].n3.y = InC[i, saveJ[i]].x3.y;
        min_point[i].n4.x = InC[i, saveJ[i]].x4.x;
        min_point[i].n4.y = InC[i, saveJ[i]].x4.y;

        // (0,0)에 j가 0일때 오른쪽과 아래에 여유공간이 있을때, 왼쪽과 위에 여유공간이 있을때 

        if (min_point[i].n1.x == 0 && min_point[i].n1.y == 0)
        {
            if (saveJ[i] == 0 && fun[i].wallcount == 2 && ((fun[i].left != 0 && fun[i].top != 0) || (fun[i].right != 0 && fun[i].bot != 0)))
            {
                return 0;

            }
        }

        if (min_point[i].n1.x == 0 && min_point[i].n1.y == 0)
        {
            if (saveJ[i] == 1 && fun[i].wallcount == 2 && ((fun[i].left != 0 && fun[i].bot != 0) || (fun[i].right != 0 && fun[i].top != 0)))
            {
                return 0;
            }
        }
        Console.WriteLine("0,0일때 확인 성공");

        for (int obs_i = 0; obs_i < obs_furn.Length; obs_i++)
        {

            Console.WriteLine("obs_furn[{0}].s1.x : {1}, obs_furn[{0}].s_width : {2}, obs_furn[{0}].s1.y : {3}, obs_furn[{0}].s_length : {4}"
                , obs_i, obs_furn[obs_i].s1.x, obs_furn[obs_i].s_width, obs_furn[obs_i].s1.y, obs_furn[obs_i].s_length);

        }

        // 일반 장애물 배열
        for (int obs_i = 0; obs_i < obs_furn.Length; obs_i++)
        {
            // 탐색점의 x좌표가 장애물의 두 x좌표 사이에 있고 && 탐색점의 y좌표가 두 y좌표 사이에 있다면
            if (((min_point[i].n1.x >= obs_furn[obs_i].s1.x) && (min_point[i].n1.x <= (obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width))) &&
                ((min_point[i].n1.y >= obs_furn[obs_i].s1.y) && (min_point[i].n1.y <= (obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length))))
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }


            // 상단 왼쪽 점
            if (((min_point[i].n2.x >= obs_furn[obs_i].s1.x) && (min_point[i].n2.x <= (obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width))) &&
                ((min_point[i].n2.y >= obs_furn[obs_i].s1.y) && (min_point[i].n2.y <= (obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length)))
                )
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }
            // 하단 오른쪽 점
            if (((min_point[i].n3.x >= obs_furn[obs_i].s1.x) && (min_point[i].n3.x <= (obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width))) &&
                ((min_point[i].n3.y >= obs_furn[obs_i].s1.y) && (min_point[i].n3.y <= (obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length)))
                )
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }
            // 상단 오른쪽 점
            if (((min_point[i].n4.x >= obs_furn[obs_i].s1.x) && (min_point[i].n4.x <= (obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width))) &&
                ((min_point[i].n4.y >= obs_furn[obs_i].s1.y) && (min_point[i].n4.y <= (obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length)))
                )
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }

            int x1, x2, y1, y2, min_x = 0, max_x = 0, min_y = 0, max_y = 0;

            x1 = min_point[i].n1.x;
            y1 = min_point[i].n1.y;
            if ((min_point[i].n2.x != x1) && (min_point[i].n2.y != y1))
            {
                x2 = min_point[i].n2.x;
                y2 = min_point[i].n2.y;
            }
            else if ((min_point[i].n3.x != x1) && (min_point[i].n3.y != y1))
            {
                x2 = min_point[i].n3.x;
                y2 = min_point[i].n3.y;
            }
            else
            {
                x2 = min_point[i].n4.x;
                y2 = min_point[i].n4.y;
            }

            // x1과 x2 크기 비교하여 둘 중 작은 값 찾기
            if (x1 > x2)
            {
                min_x = x2; max_x = x1;
            }
            else
            {
                min_x = x1; max_x = x2;
            }
            // y1과 y2 크기 비교하여 둘 중 작은 값 찾기
            if (y1 > y2)
            {
                min_y = y2; max_y = y1;
            }
            else
            {
                min_y = y1; max_y = y2;
            }


            // 탐색점의 x좌표가 장애물의 두 x좌표 사이에 있고 && 탐색점의 y좌표가 두 y좌표 사이에 있다면
            if (((min_x <= obs_furn[obs_i].s1.x) && (max_x >= (obs_furn[obs_i].s1.x))) &&
                ((min_y <= obs_furn[obs_i].s1.y) && (max_y >= (obs_furn[obs_i].s1.y))))
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }
            // 상단 왼쪽 점
            if (((min_x <= obs_furn[obs_i].s1.x) && (max_x >= (obs_furn[obs_i].s1.x))) &&
                ((min_y <= obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length) && (max_y >= (obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length)))
                )
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }
            // 하단 오른쪽 점
            if (((min_x <= obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width) && (max_x >= (obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width))) &&
                ((min_y <= obs_furn[obs_i].s1.y) && (max_y >= (obs_furn[obs_i].s1.y)))
                )
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }
            // 상단 오른쪽 점
            if (((min_x <= obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width) && (max_x >= (obs_furn[obs_i].s1.x + obs_furn[obs_i].s_width))) &&
                ((min_y <= obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length) && (max_y >= (obs_furn[obs_i].s1.y + obs_furn[obs_i].s_length)))
                )
            {
                Console.WriteLine("-------True Test-------");
                return 0;
            }
        }
        Console.WriteLine("matchcheck : 일반 장애물 확인");

        // 배치된 가구 배열
        for (int deploy_i = 0; deploy_i < i; deploy_i++)
        {
            int x1, x2, y1, y2, min_x = 0, max_x = 0, min_y = 0, max_y = 0;

            x1 = InC[deploy_i, saveJ[deploy_i]].x1.x;
            y1 = InC[deploy_i, saveJ[deploy_i]].x1.y;
            if ((InC[deploy_i, saveJ[deploy_i]].x2.x != x1) && (InC[deploy_i, saveJ[deploy_i]].x2.y != y1))
            {
                x2 = InC[deploy_i, saveJ[deploy_i]].x2.x;
                y2 = InC[deploy_i, saveJ[deploy_i]].x2.y;
            }
            else if ((InC[deploy_i, saveJ[deploy_i]].x3.x != x1) && (InC[deploy_i, saveJ[deploy_i]].x3.y != y1))
            {
                x2 = InC[deploy_i, saveJ[deploy_i]].x3.x;
                y2 = InC[deploy_i, saveJ[deploy_i]].x3.y;
            }
            else
            {
                x2 = InC[deploy_i, saveJ[deploy_i]].x4.x;
                y2 = InC[deploy_i, saveJ[deploy_i]].x4.y;
            }

            // x1과 x2 크기 비교하여 둘 중 작은 값 찾기
            if (x1 > x2)
            {
                min_x = x2; max_x = x1;
            }
            else
            {
                min_x = x1; max_x = x2;
            }
            // y1과 y2 크기 비교하여 둘 중 작은 값 찾기
            if (y1 > y2)
            {
                min_y = y2; max_y = y1;
            }
            else
            {
                min_y = y1; max_y = y2;
            }

            Console.WriteLine("min_x : {0}, max_x : {1}, min_y : {2}, max_y : {3}", min_x, max_x, min_y, max_y);

            if ((min_x <= min_point[i].n1.x && min_point[i].n1.x <= max_x) && (min_y <= min_point[i].n1.y && min_point[i].n1.y <= max_y))
            {
                return 0;
            }
            if ((min_x <= min_point[i].n2.x && min_point[i].n2.x <= max_x) && (min_y <= min_point[i].n2.y && min_point[i].n2.y <= max_y))
            {
                return 0;
            }
            if ((min_x <= min_point[i].n3.x && min_point[i].n3.x <= max_x) && (min_y <= min_point[i].n3.y && min_point[i].n3.y <= max_y))
            {
                return 0;
            }
            if ((min_x <= min_point[i].n4.x && min_point[i].n4.x <= max_x) && (min_y <= min_point[i].n4.y && min_point[i].n4.y <= max_y))
            {
                return 0;
            }
        }
        Console.WriteLine("matchcheck : 배치된 가구 확인");

        // 위의 모든 경우와 겹치지 않았다면, 배치해도 좋으므로 1값 반환!
        return 1;
    }

    public void mainFunction ()
    {

        room = _generateRoom.room;
        //int n; // 다양한 오브젝트들의 개수 입력 위한 변수

        //Console.WriteLine("방의 가로폭을 입력해주세요");
        //room.width = int.Parse(Console.ReadLine());
        //Console.WriteLine("방의 세로폭을 입력해주세요");
        //room.length = int.Parse(Console.ReadLine());

        /*Console.WriteLine("방문의 개수를 입력해주세요");
        n = int.Parse(Console.ReadLine());
        door = new Obstacle[n];
        for (int d = 0; d < n; d++)
        {
            Console.WriteLine("방문이 어느 면에 있나요?(1:위, 2:아래, 3:우측, 4:좌측)");
            if (Console.ReadLine() == "1")
            {
                Console.WriteLine("방문의 길이를 입력해주세요");
                door[d].width = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 x좌표를 입력해주세요");
                door[d].n1.x = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 y좌표를 입력해주세요");
                door[d].n1.y = int.Parse(Console.ReadLine());
                Console.WriteLine("방문이 여닫이문이다(y/n)");
                if (Console.ReadLine() == "y")
                {
                    door[d].s1.x = door[d].n1.x;
                    door[d].s1.y = door[d].n1.y - door[d].width;
                    door[d].s_width = door[d].width;
                    door[d].s_length = door[d].width;
                }
            }
            else if (Console.ReadLine() == "2")
            {
                Console.WriteLine("방문의 길이를 입력해주세요");
                door[d].width = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 x좌표를 입력해주세요");
                door[d].n1.x = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 y좌표를 입력해주세요");
                door[d].n1.y = int.Parse(Console.ReadLine());
                Console.WriteLine("방문이 여닫이문이다(y/n)");
                if (Console.ReadLine() == "y")
                {
                    door[d].s1.x = door[d].n1.x;
                    door[d].s1.y = 0;
                    door[d].s_width = door[d].width;
                }
            }
            else if (Console.ReadLine() == "3")
            {
                Console.WriteLine("방문의 길이를 입력해주세요");
                door[d].width = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 x좌표를 입력해주세요");
                door[d].n1.x = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 y좌표를 입력해주세요");
                door[d].n1.y = int.Parse(Console.ReadLine());
                Console.WriteLine("방문이 여닫이문이다(y/n)");
                if (Console.ReadLine() == "y")
                {
                    door[d].s1.x = door[d].n1.x - door[d].width;
                    door[d].s1.y = door[d].n1.y;
                    door[d].width = door[d].width;
                }
            }
            else if (Console.ReadLine() == "4")
            {
                Console.WriteLine("방문의 길이를 입력해주세요");
                door[d].width = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 x좌표를 입력해주세요");
                door[d].n1.x = int.Parse(Console.ReadLine());
                Console.WriteLine("방문의 y좌표를 입력해주세요");
                door[d].n1.y = int.Parse(Console.ReadLine());
                Console.WriteLine("방문이 여닫이문이다(y/n)");
                if (Console.ReadLine() == "y")
                {
                    door[d].s1.x = door[d].n1.x;
                    door[d].s1.y = door[d].n1.y;
                    door[d].width = door[d].width;
                }
            }
        }
        */

        //Console.WriteLine("붙박이가구 또는 문의 개수를 입력해주세요");
        //n = int.Parse(Console.ReadLine());
        obs_furn = _generateObstacle.obs_furn;
        //int option;
        //for (int f = 0; f < n; f++)
        //{
        //    Console.WriteLine("붙박이가구 또는 문 이름 입력");
        //    obs_furn[f].name = Console.ReadLine();
        //    Console.WriteLine("1. 붙박이가구 2. 문");
        //    option = int.Parse(Console.ReadLine());
        //    Console.WriteLine("붙박이가구의 가로폭 입력");
        //    obs_furn[f].width = int.Parse(Console.ReadLine());
        //    Console.WriteLine("붙박이가구의 세로폭 입력");
        //    obs_furn[f].length = int.Parse(Console.ReadLine());
        //    Console.WriteLine("붙박이가구의 x좌표를 입력해주세요");
        //    obs_furn[f].n1.x = int.Parse(Console.ReadLine());
        //    Console.WriteLine("붙박이가구의 y좌표를 입력해주세요");
        //    obs_furn[f].n1.y = int.Parse(Console.ReadLine());
        //    // 문일 때는 0 0 0 0 입력!!
        //    Console.WriteLine("붙박이가구의 윗면 여유공간 입력");
        //    obs_furn[f].top = int.Parse(Console.ReadLine());
        //    if (obs_furn[f].top != 0) obs_furn[f].wallcount++;
        //    Console.WriteLine("붙박이가구의 아랫면 여유공간 입력");
        //    obs_furn[f].bot = int.Parse(Console.ReadLine());
        //    if (obs_furn[f].bot != 0) obs_furn[f].wallcount++;
        //    Console.WriteLine("붙박이가구의 우측면 여유공간 입력");
        //    obs_furn[f].right = int.Parse(Console.ReadLine());
        //    if (obs_furn[f].right != 0) obs_furn[f].wallcount++;
        //    Console.WriteLine("붙박이가구의 좌측면 여유공간 입력");
        //    obs_furn[f].left = int.Parse(Console.ReadLine());
        //    if (obs_furn[f].left != 0) obs_furn[f].wallcount++;

        //    if (option != 1)
        //    {
        //        obs_furn[f].s1.x = obs_furn[f].n1.x;
        //        obs_furn[f].s1.y = obs_furn[f].n1.y;
        //        obs_furn[f].s_width = obs_furn[f].width;
        //        obs_furn[f].s_length = obs_furn[f].width;
        //    }
        //    else
        //    {
        //        if (obs_furn[f].wallcount == 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width;
        //            obs_furn[f].s_length = obs_furn[f].length;
        //        }
        //        else if (obs_furn[f].wallcount == 1 && obs_furn[f].top != 0) //가구 사용공간 계산
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top;
        //        }
        //        else if (obs_furn[f].wallcount == 1 && obs_furn[f].bot != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 1 && obs_furn[f].right != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right;
        //            obs_furn[f].s_length = obs_furn[f].length;
        //        }
        //        else if (obs_furn[f].wallcount == 1 && obs_furn[f].left != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length;
        //        }
        //        else if (obs_furn[f].wallcount == 2 && obs_furn[f].top != 0 && obs_furn[f].left != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top;
        //        }
        //        else if (obs_furn[f].wallcount == 2 && obs_furn[f].left != 0 && obs_furn[f].bot != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 2 && obs_furn[f].bot != 0 && obs_furn[f].right != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 2 && obs_furn[f].right != 0 && obs_furn[f].top != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top;
        //        }
        //        else if (obs_furn[f].wallcount == 2 && obs_furn[f].top != 0 && obs_furn[f].bot != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 2 && obs_furn[f].right != 0 && obs_furn[f].left != 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length;
        //        }
        //        else if (obs_furn[f].wallcount == 3 && obs_furn[f].top == 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 3 && obs_furn[f].bot == 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top;
        //        }
        //        else if (obs_furn[f].wallcount == 3 && obs_furn[f].right == 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 3 && obs_furn[f].left == 0)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top + obs_furn[f].bot;
        //        }
        //        else if (obs_furn[f].wallcount == 4)
        //        {
        //            obs_furn[f].s1.x = obs_furn[f].n1.x - obs_furn[f].width;
        //            obs_furn[f].s1.y = obs_furn[f].n1.y - obs_furn[f].length;
        //            obs_furn[f].s_width = obs_furn[f].width + obs_furn[f].right + obs_furn[f].left;
        //            obs_furn[f].s_length = obs_furn[f].length + obs_furn[f].top + obs_furn[f].bot;
        //        }
        //    }


        //}
        //Console.WriteLine("가구의 개수를 입력해주세요.");
        fun_num = _generateMultipleFuniture.Furniture_index;
        min = _generateMultipleFuniture.min;
        fun = _generateMultipleFuniture.fun;
        saveJ = new int[fun_num];
        //for (int i = 0; i < fun_num; i++)
        //{
        //    Console.WriteLine("가구 이름 입력");
        //    fun[i].name = Console.ReadLine();
        //    Console.WriteLine("가구의 가로폭 입력");
        //    fun[i].width = int.Parse(Console.ReadLine());
        //    Console.WriteLine("가구의 세로폭 입력");
        //    fun[i].length = int.Parse(Console.ReadLine());
        //    // 탐색 폭 최소 단위 구하기
        //    if (min > fun[i].width)
        //    {
        //        min = fun[i].width;
        //    }
        //    if (min > fun[i].length)
        //    {
        //        min = fun[i].length;
        //    }
        //    Console.WriteLine("가구의 윗면 여유공간 입력");
        //    fun[i].top = int.Parse(Console.ReadLine());
        //    if (fun[i].top != 0) fun[i].wallcount++;
        //    Console.WriteLine("가구의 아랫면 여유공간 입력");
        //    fun[i].bot = int.Parse(Console.ReadLine());
        //    if (fun[i].bot != 0) fun[i].wallcount++;
        //    Console.WriteLine("가구의 우측면 여유공간 입력");
        //    fun[i].right = int.Parse(Console.ReadLine());
        //    if (fun[i].right != 0) fun[i].wallcount++;
        //    Console.WriteLine("가구의 좌측면 여유공간 입력");
        //    fun[i].left = int.Parse(Console.ReadLine());
        //    if (fun[i].left != 0) fun[i].wallcount++;

        //    if (fun[i].wallcount == 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width;
        //        fun[i].s_length = fun[i].length;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 1 && fun[i].top != 0) //가구 사용공간 계산
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width;
        //        fun[i].s_length = fun[i].length + fun[i].top;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 1 && fun[i].bot != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width;
        //        fun[i].s_length = fun[i].length + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 1 && fun[i].right != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width + fun[i].right;
        //        fun[i].s_length = fun[i].length;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 1 && fun[i].left != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width + fun[i].left;
        //        fun[i].s_length = fun[i].length;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 2 && fun[i].top != 0 && fun[i].left != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width + fun[i].left;
        //        fun[i].s_length = fun[i].length + fun[i].top;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 2 && fun[i].left != 0 && fun[i].bot != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width + fun[i].left;
        //        fun[i].s_length = fun[i].length + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 2 && fun[i].bot != 0 && fun[i].right != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width + fun[i].right;
        //        fun[i].s_length = fun[i].length + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 2 && fun[i].right != 0 && fun[i].top != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width + fun[i].right;
        //        fun[i].s_length = fun[i].length + fun[i].top;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 2 && fun[i].top != 0 && fun[i].bot != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width;
        //        fun[i].s_length = fun[i].length + fun[i].top + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 2 && fun[i].right != 0 && fun[i].left != 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width + fun[i].right + fun[i].left;
        //        fun[i].s_length = fun[i].length;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 3 && fun[i].top == 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width + fun[i].right + fun[i].left;
        //        fun[i].s_length = fun[i].length + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 3 && fun[i].bot == 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y;
        //        fun[i].s_width = fun[i].width + fun[i].right + fun[i].left;
        //        fun[i].s_length = fun[i].length + fun[i].top;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 3 && fun[i].right == 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width + fun[i].left;
        //        fun[i].s_length = fun[i].length + fun[i].top + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 3 && fun[i].left == 0)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width + fun[i].right;
        //        fun[i].s_length = fun[i].length + fun[i].top + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //    else if (fun[i].wallcount == 4)
        //    {
        //        //fun[i].s1.x = fun[i].n1.x - fun[i].width;
        //        //fun[i].s1.y = fun[i].n1.y - fun[i].length;
        //        fun[i].s_width = fun[i].width + fun[i].right + fun[i].left;
        //        fun[i].s_length = fun[i].length + fun[i].top + fun[i].bot;
        //        fun[i].area = fun[i].s_width * fun[i].s_length;
        //    }
        //}

        // 경우의 수(최대 200개라고 가정)가 하나 완성될 때마다 그 정보를 저장할 배열 할당
        s_fun = new save_fun[100, fun_num];

        // InC 초기화
        InC = new Point2[fun_num, 2];

        // ToU 초기화
        ToU = new Point2[fun_num, 2];

        min_point = new Point_min[fun_num];

        // 탐색 점의 기본 디폴트 값으로 초기화, 원점에서부터 시작하므로!
        for (int i = 0; i < fun_num; i++)
        {
            min_point[i].n1.x = 0;
            min_point[i].n1.y = 0;
            min_point[i].n2.x = 0;
            min_point[i].n2.y = 0;
            min_point[i].n3.x = 0;
            min_point[i].n3.y = 0;
            min_point[i].n4.x = 0;
            min_point[i].n4.y = 0;
        }

        // 정렬 전 확인
        /*
        for (int i = 0; i < fun_num; i++)
        {
            Console.WriteLine(fun[i].area);
        }
        Console.WriteLine("--------after sorting---------");
        */

        // 내림차순 정렬 (넓이 기준으로 큰 것부터!)
        //일단 정렬 주석 처리
        //Array.Sort<Furniture>(fun, (x, y) => x.area.CompareTo(y.area));
        //Array.Reverse<Furniture>(fun);

        /*
        // 정렬 후 확인
        for (int i = 0; i < fun_num; i++)
        {
            Console.WriteLine(fun[i].area);
        }
        */

        for (int i = 0; i < fun_num; i++)
        {
            InC[i, 0].x1.x = 0;
            InC[i, 0].x1.y = 0;
            InC[i, 0].x2.x = fun[i].s_width;
            InC[i, 0].x2.y = 0;
            InC[i, 0].x3.x = 0;
            InC[i, 0].x3.y = fun[i].s_length;
            InC[i, 0].x4.x = fun[i].s_width;
            InC[i, 0].x4.y = fun[i].s_length;
            ToU[i, 0].x1.x = 0;
            ToU[i, 0].x1.y = 0;
            ToU[i, 0].x2.x = fun[i].width;
            ToU[i, 0].x2.y = 0;
            ToU[i, 0].x3.x = 0;
            ToU[i, 0].x3.y = fun[i].length;
            ToU[i, 0].x4.x = fun[i].width;
            ToU[i, 0].x4.y = fun[i].length;
        }

        for (int i = 0; i < fun_num; i++)
        {
            InC[i, 1].x1.x = 0;
            InC[i, 1].x1.y = 0;
            InC[i, 1].x2.x = fun[i].s_length;
            InC[i, 1].x2.y = 0;
            InC[i, 1].x3.x = 0;
            InC[i, 1].x3.y = fun[i].s_width;
            InC[i, 1].x4.x = fun[i].s_length;
            InC[i, 1].x4.y = fun[i].s_width;
            ToU[i, 1].x1.x = 0;
            ToU[i, 1].x1.y = 0;
            ToU[i, 1].x2.x = fun[i].length;
            ToU[i, 1].x2.y = 0;
            ToU[i, 1].x3.x = 0;
            ToU[i, 1].x3.y = fun[i].width;
            ToU[i, 1].x4.x = fun[i].length;
            ToU[i, 1].x4.y = fun[i].width;
        }

        for (int i = 0; i < saveJ.Length; i++)
        {
            saveJ[i] = 0;
        }

        for (int i = 0; i < obs_furn.Length; i++)
        {
            Console.WriteLine("{0}번째 장애물의 x좌표 1 : {1}", i + 1, obs_furn[i].s1.x);
            Console.WriteLine("{0}번째 장애물의 y좌표 1 : {1}", i + 1, obs_furn[i].s1.y);
            Console.WriteLine("{0}번째 장애물의 x좌표 2 : {1}", i + 1, obs_furn[i].s1.x + obs_furn[i].s_width);
            Console.WriteLine("{0}번째 장애물의 y좌표 2 : {1}", i + 1, obs_furn[i].s1.y + obs_furn[i].s_length);
        }

        // 배치 알고리즘 함수 호출
        deploy_fun();

        for (int saveN = 0; saveN < saveCase_num; saveN++)
        {
            Console.WriteLine("----------------{0}번째 경우의 수----------------", saveN + 1);
            for (int i = 0; i < fun_num; i++)
            {
                Console.WriteLine("{0}번째 가구의 n1의 x좌표 : {1}", i + 1, s_fun[saveN, i].n1.x);
                Console.WriteLine("{0}번째 가구의 n1의 y좌표 : {1}", i + 1, s_fun[saveN, i].n1.y);
                Console.WriteLine("{0}번째 가구의 n2의 x좌표 : {1}", i + 1, s_fun[saveN, i].n2.x);
                Console.WriteLine("{0}번째 가구의 n2의 y좌표 : {1}", i + 1, s_fun[saveN, i].n2.y);
                Console.WriteLine("{0}번째 가구의 n3의 x좌표 : {1}", i + 1, s_fun[saveN, i].n3.x);
                Console.WriteLine("{0}번째 가구의 n3의 y좌표 : {1}", i + 1, s_fun[saveN, i].n3.y);
                Console.WriteLine("{0}번째 가구의 n4의 x좌표 : {1}", i + 1, s_fun[saveN, i].n4.x);
                Console.WriteLine("{0}번째 가구의 n4의 y좌표 : {1}", i + 1, s_fun[saveN, i].n4.y);
                Console.WriteLine("\n");
            }
            Console.WriteLine("\n");
        }


    }

     void pointMove(int i, int j, GenerateRoom.Point sp)
    {
        Console.WriteLine("i : {0}, j : {1}, sp.x : {2}, sp.y : {3}", i, j, sp.x, sp.y);
        // 첫번째 모양
        if (j == 0)
        {
            //여유공간이 0이거나 1
            if (fun[i].wallcount == 0 || fun[i].wallcount == 1)
            {
                // 왼쪽 벽
                if (sp.x == 0 && sp.y != room.length)
                {
                    // 방 크기를 벗어나면 위쪽 벽으로 재위치시킴
                    if (sp.y + min + fun[i].s_length >= room.length)
                    {
                        Console.WriteLine("min : {0}, fun[i].s_length : {1}, room_length : {2}", min, fun[i].s_length, room.length);
                        min_point[i].n1.y = room.length;
                        sp.y = room.length;

                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].length;
                        ToU[i, j].x3.x = sp.x + fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x + fun[i].width;
                        ToU[i, j].x4.y = sp.y - fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_length;
                        InC[i, j].x3.x = sp.x + fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x + fun[i].s_width;
                        InC[i, j].x4.y = sp.y - fun[i].s_length;
                        Console.WriteLine("iToU[i, j].x1.x : {0}, ToU[i, j].x1.y : {1}", ToU[i, j].x1.x, ToU[i, j].x1.y);
                    }
                    else // 위로 이동
                    {
                        Console.WriteLine("min : {0}, fun[i].s_length : {1}, room_length : {2}", min, fun[i].s_length, room.length);
                        min_point[i].n1.y += min;
                        sp.y += min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y + fun[i].length;
                        ToU[i, j].x3.x = sp.x + fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x + fun[i].width;
                        ToU[i, j].x4.y = sp.y + fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y + fun[i].s_length;
                        InC[i, j].x3.x = sp.x + fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x + fun[i].s_width;
                        InC[i, j].x4.y = sp.y + fun[i].s_length;


                    }

                }
                // 위쪽벽
                else if (sp.y == room.length && sp.x != room.width)
                {
                    // 방크기를 벗어나면 오른쪽 벽으로 재위치
                    if (sp.x + fun[i].s_width + min >= room.width)
                    {
                        min_point[i].n1.x = room.width;
                        sp.x = room.width;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].length;
                        ToU[i, j].x3.x = sp.x - fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].width;
                        ToU[i, j].x4.y = sp.y - fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_length;
                        InC[i, j].x3.x = sp.x - fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_width;
                        InC[i, j].x4.y = sp.y - fun[i].s_length;
                    }
                    // 옆으로 이동
                    else
                    {
                        min_point[i].n1.x += min;
                        sp.x += min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].length;
                        ToU[i, j].x3.x = sp.x + fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x + fun[i].width;
                        ToU[i, j].x4.y = sp.y - fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_length;
                        InC[i, j].x3.x = sp.x + fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x + fun[i].s_width;
                        InC[i, j].x4.y = sp.y - fun[i].s_length;
                    }
                }
                // 오른쪽 벽
                else if (sp.x == room.width && sp.y != 0)
                {
                    // 방크기를 벗어나면 아래 벽으로 이동
                    if (sp.y - fun[i].s_length - min <= 0)
                    {
                        min_point[i].n1.y = 0;
                        sp.y = 0;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y + fun[i].length;
                        ToU[i, j].x3.x = sp.x - fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].width;
                        ToU[i, j].x4.y = sp.y + fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y + fun[i].s_length;
                        InC[i, j].x3.x = sp.x - fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_width;
                        InC[i, j].x4.y = sp.y + fun[i].s_length;
                    }
                    // 아래로 이동
                    else
                    {
                        min_point[i].n1.y -= min;
                        sp.y -= min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].length;
                        ToU[i, j].x3.x = sp.x - fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].width;
                        ToU[i, j].x4.y = sp.y - fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_length;
                        InC[i, j].x3.x = sp.x - fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_width;
                        InC[i, j].x4.y = sp.y - fun[i].s_length;
                    }

                }
                // 아래쪽 벽
                else if (sp.y == 0 && sp.x != 0)
                {
                    // 방 크기를 벗어나면
                    if (sp.x - fun[i].s_width - min <= 0)
                    {
                        min_point[i].n1.x = -1;
                        return;

                    }
                    // 옆으로 이동
                    else
                    {
                        min_point[i].n1.x -= min;
                        sp.x -= min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y + fun[i].length;
                        ToU[i, j].x3.x = sp.x - fun[i].width;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].width;
                        ToU[i, j].x4.y = sp.y + fun[i].length;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y + fun[i].s_length;
                        InC[i, j].x3.x = sp.x - fun[i].s_width;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_width;
                        InC[i, j].x4.y = sp.y + fun[i].s_length;
                    }

                }
            }
            // 여유공간이 두 개 일 때
            else if (fun[i].wallcount == 2)
            {
                // 오른, 왼 여유공간
                if (fun[i].right != 0 && fun[i].left != 0)
                {
                    //왼쪽벽 못붙음 바로 윗벽으로 이동
                    if (sp.x == 0)
                    {
                        sp.x = min;
                        sp.y = room.length;
                        min_point[i].n1.x = min;
                        min_point[i].n1.y = room.length;
                    }
                    //윗벽일때
                    else if (sp.x == 0 && sp.y == room.length)
                    {
                        // 크기 벗어나면 바로 아래쪽 벽으로 이동(오른쪽 벽 건너뜀)
                        if (sp.x + fun[i].s_width >= room.width - min)
                        {
                            sp.x = room.width - min;
                            sp.y = 0;
                            min_point[i].n1.x = room.width - min;
                            min_point[i].n1.y = 0;

                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }
                        // 옆으로 이동
                        else
                        {
                            min_point[i].n1.x += min;
                            sp.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                    }
                    //아래쪽 벽일때
                    else if (sp.y == 0)
                    {
                        // 크기벗어나면
                        if (sp.x - fun[i].s_width <= min)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        // 옆으로 이동
                        else
                        {
                            min_point[i].n1.x -= min;
                            sp.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }
                    }
                }
                // 위 아래에 여유공간이 있을 때
                else if (fun[i].top != 0 && fun[i].bot != 0)
                {
                    // 왼쪽 벽
                    if (sp.x == 0)
                    {
                        // 방크기 벗어나면 바로 오른쪽 벽으로 이동
                        if (sp.y + fun[i].s_length >= room.length - min)
                        {
                            sp.x = room.width;
                            sp.y = room.length - min;
                            min_point[i].n1.x = room.width;
                            min_point[i].n1.y = room.length - min;

                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;

                        }
                        // 옆으로 이동
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;


                        }
                    }
                    // 오른쪽 벽
                    else if (sp.x == room.width)
                    {
                        // 방크기 벗어나면
                        if (sp.y - fun[i].s_length <= min)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        // 밑으로 이동
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                    }
                }
                // 왼위, 오른아래 에 여유공간 있을 때
                else if ((fun[i].top != 0 && fun[i].left != 0) || (fun[i].right != 0 && fun[i].bot != 0))
                {
                    //왼아래끝, 오른위끝 건너뛰는 것 빼고 여유공간 0,1 때랑 똑같음
                    if (sp.x == 0 && sp.y != room.length)
                    {
                        if (sp.y + min + fun[i].s_length > room.length)
                        {
                            sp.y = room.length;
                            min_point[i].n1.y = room.length;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;


                        }

                    }
                    else if (sp.y == room.length && sp.x != room.width)
                    {
                        if (sp.x + fun[i].s_width + min > room.width)
                        {
                            // 오른위 끝 건너뜀
                            sp.y = room.length - min;
                            sp.x = room.width;
                            min_point[i].n1.x = room.width;
                            min_point[i].n1.y = room.length - min;

                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                        else
                        {
                            min_point[i].n1.x += min;
                            sp.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                    }
                    else if (sp.x == room.width && sp.y != 0)
                    {
                        if (sp.y - fun[i].s_length - min < 0)
                        {
                            min_point[i].n1.y = 0;
                            sp.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }
                        else
                        {
                            min_point[i].n1.y -= min;
                            sp.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }

                    }
                    else if (sp.y == 0 && sp.x != 0)
                    {
                        if (sp.x - fun[i].s_width - min < 0)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            min_point[i].n1.x -= min;
                            sp.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }

                    }
                }
                // 여유공간이 왼 아래, 위 오른 에 있을 때
                else if ((fun[i].top != 0 && fun[i].right != 0) || (fun[i].bot != 0 && fun[i].left != 0))
                {
                    // 왼위 끝, 오른 아래끝 건너 뛰는 것 빼고 여유공간 0,1 일때랑 똑같음
                    if (sp.x == 0 && sp.y != room.length)
                    {
                        if (sp.y + min + fun[i].s_length > room.length)
                        {
                            // 왼위 끝 건너뜀
                            sp.x = min;
                            sp.y = room.length;
                            min_point[i].n1.x = min;
                            min_point[i].n1.y = room.length;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;


                        }

                    }
                    else if (sp.y == room.length && sp.x != room.width)
                    {
                        if (sp.x + fun[i].s_width + min > room.width)
                        {
                            min_point[i].n1.x = room.width;
                            sp.x = room.width;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                        else
                        {
                            min_point[i].n1.x += min;
                            sp.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                    }
                    else if (sp.x == room.width && sp.y != 0)
                    {
                        if (sp.y - fun[i].s_length - min < 0)
                        {
                            // 오른아래 건너뜀
                            sp.x = room.width - min;
                            sp.y = 0;
                            min_point[i].n1.x = room.width - min;
                            min_point[i].n1.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }

                    }
                    else if (sp.y == 0 && sp.x != 0)
                    {
                        if (sp.x - fun[i].s_width - min < 0)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.x -= min;
                            min_point[i].n1.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }

                    }
                }
            }
            // 여유공간이 3개 일때
            else if (fun[i].wallcount == 3)
            {
                // 여유공간이 왼,위,오 거나 왼,오,아 일 때 => 아래벽, 윗벽에만 붙도록
                if ((fun[i].left != 0 && fun[i].top != 0 && fun[i].right != 0) || (fun[i].left != 0 && fun[i].right != 0 && fun[i].bot != 0))
                {
                    if (sp.x == 0)
                    {
                        sp.x = min;
                        sp.y = room.length;
                        min_point[i].n1.x = min;
                        min_point[i].n1.y = room.length;
                    }
                    else if (sp.y == room.length)
                    {
                        if (sp.x + fun[i].s_width >= room.width - min)
                        {
                            sp.x = room.width - min;
                            sp.y = 0;
                            min_point[i].n1.x = room.width - min;
                            min_point[i].n1.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }
                        else
                        {
                            sp.x += min;
                            min_point[i].n1.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                    }
                    else if (sp.y == 0)
                    {
                        if (sp.x - fun[i].s_width <= min)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.x -= min;
                            min_point[i].n1.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;
                        }
                    }
                }
                // 여유공간이 왼,위,아 거나 위,오,아 일때 -> 오른벽, 왼벽에만 붙도록
                else if ((fun[i].left != 0 && fun[i].top != 0 && fun[i].bot != 0) || (fun[i].top != 0 && fun[i].right != 0 && fun[i].bot != 0))
                {
                    if (sp.x == 0)
                    {
                        if (sp.y + fun[i].s_length >= room.length - min)
                        {
                            sp.x = room.width;
                            sp.y = room.length - min;
                            min_point[i].n1.x = room.width;
                            min_point[i].n1.y = room.length - min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].length;
                            ToU[i, j].x3.x = sp.x + fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].width;
                            ToU[i, j].x4.y = sp.y + fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_length;
                            InC[i, j].x3.x = sp.x + fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_width;
                            InC[i, j].x4.y = sp.y + fun[i].s_length;


                        }
                    }
                    else if (sp.x == room.width)
                    {
                        if (sp.y - fun[i].s_length <= min)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].length;
                            ToU[i, j].x3.x = sp.x - fun[i].width;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].width;
                            ToU[i, j].x4.y = sp.y - fun[i].length;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_length;
                            InC[i, j].x3.x = sp.x - fun[i].s_width;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_width;
                            InC[i, j].x4.y = sp.y - fun[i].s_length;
                        }
                    }

                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        else if (j == 1)
        {
            if (fun[i].wallcount == 0 || fun[i].wallcount == 1)
            {
                if (sp.x == 0 && sp.y != room.length)
                {
                    if (sp.y + min + fun[i].s_width > room.length)
                    {
                        min_point[i].n1.y = room.length;
                        sp.y = room.length;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].width;
                        ToU[i, j].x3.x = sp.x + fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x + fun[i].length;
                        ToU[i, j].x4.y = sp.y - fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_width;
                        InC[i, j].x3.x = sp.x + fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x + fun[i].s_length;
                        InC[i, j].x4.y = sp.y - fun[i].s_width;

                    }
                    else
                    {
                        min_point[i].n1.y += min;
                        sp.y += min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y + fun[i].width;
                        ToU[i, j].x3.x = sp.x + fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x + fun[i].length;
                        ToU[i, j].x4.y = sp.y + fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y + fun[i].s_width;
                        InC[i, j].x3.x = sp.x + fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x + fun[i].s_length;
                        InC[i, j].x4.y = sp.y + fun[i].s_width;


                    }

                }
                else if (sp.y == room.length && sp.x != room.width)
                {
                    if (sp.x + fun[i].s_length + min > room.width)
                    {
                        min_point[i].n1.x = room.width;
                        sp.x = room.width;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].width;
                        ToU[i, j].x3.x = sp.x - fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].length;
                        ToU[i, j].x4.y = sp.y - fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_width;
                        InC[i, j].x3.x = sp.x - fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_length;
                        InC[i, j].x4.y = sp.y - fun[i].s_width;
                    }
                    else
                    {
                        min_point[i].n1.x += min;
                        sp.x += min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].width;
                        ToU[i, j].x3.x = sp.x + fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x + fun[i].length;
                        ToU[i, j].x4.y = sp.y - fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_width;
                        InC[i, j].x3.x = sp.x + fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x + fun[i].s_length;
                        InC[i, j].x4.y = sp.y - fun[i].s_width;
                    }
                }
                else if (sp.x == room.width && sp.y != 0)
                {
                    if (sp.y - fun[i].s_width - min < 0)
                    {
                        min_point[i].n1.y = 0;
                        sp.y = 0;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y + fun[i].width;
                        ToU[i, j].x3.x = sp.x - fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].length;
                        ToU[i, j].x4.y = sp.y + fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y + fun[i].s_width;
                        InC[i, j].x3.x = sp.x - fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_length;
                        InC[i, j].x4.y = sp.y + fun[i].s_width;
                    }
                    else
                    {
                        min_point[i].n1.y -= min;
                        sp.y -= min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y - fun[i].width;
                        ToU[i, j].x3.x = sp.x - fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].length;
                        ToU[i, j].x4.y = sp.y - fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y - fun[i].s_width;
                        InC[i, j].x3.x = sp.x - fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_length;
                        InC[i, j].x4.y = sp.y - fun[i].s_width;
                    }

                }
                else if (sp.y == 0 && sp.x != 0)
                {
                    if (sp.x - fun[i].s_length - min < 0)
                    {
                        min_point[i].n1.x = -1;
                        return;
                    }
                    else
                    {
                        min_point[i].n1.x -= min;
                        sp.x -= min;
                        ToU[i, j].x1.x = sp.x;
                        ToU[i, j].x1.y = sp.y;
                        ToU[i, j].x2.x = sp.x;
                        ToU[i, j].x2.y = sp.y + fun[i].width;
                        ToU[i, j].x3.x = sp.x - fun[i].length;
                        ToU[i, j].x3.y = sp.y;
                        ToU[i, j].x4.x = sp.x - fun[i].length;
                        ToU[i, j].x4.y = sp.y + fun[i].width;

                        InC[i, j].x1.x = sp.x;
                        InC[i, j].x1.y = sp.y;
                        InC[i, j].x2.x = sp.x;
                        InC[i, j].x2.y = sp.y + fun[i].s_width;
                        InC[i, j].x3.x = sp.x - fun[i].s_length;
                        InC[i, j].x3.y = sp.y;
                        InC[i, j].x4.x = sp.x - fun[i].s_length;
                        InC[i, j].x4.y = sp.y + fun[i].s_width;
                    }

                }
            }
            else if (fun[i].wallcount == 2)
            {
                if (fun[i].top != 0 && fun[i].bot != 0)
                {
                    if (sp.x == 0)
                    {
                        sp.x = min;
                        sp.y = room.length;
                        min_point[i].n1.x = min;
                        min_point[i].n1.y = room.length;
                    }
                    else if (sp.y == room.length)
                    {
                        if (sp.x + fun[i].s_length >= room.width - min)
                        {
                            sp.x = room.width - min;
                            sp.y = 0;
                            min_point[i].n1.x = room.width - min;
                            min_point[i].n1.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }
                        else
                        {
                            sp.x += min;
                            min_point[i].n1.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                    }
                    else if (sp.y == 0)
                    {
                        if (sp.x - fun[i].s_length <= min)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.x -= min;
                            min_point[i].n1.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }
                    }
                }
                else if (fun[i].right != 0 && fun[i].left != 0)
                {
                    if (sp.x == 0)
                    {
                        if (sp.y + fun[i].s_width >= room.length - min)
                        {
                            sp.x = room.width;
                            sp.y = room.length - min;
                            min_point[i].n1.x = room.width;
                            min_point[i].n1.y = room.length - min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;


                        }
                    }
                    else if (sp.x == room.width)
                    {
                        if (sp.y - fun[i].s_width <= min)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                    }
                }
                else if ((fun[i].top != 0 && fun[i].right != 0) || (fun[i].bot != 0 && fun[i].left != 0))
                {
                    if (sp.x == 0 && sp.y != room.length)
                    {
                        if (sp.y + min + fun[i].s_width > room.length)
                        {
                            sp.y = room.length;
                            min_point[i].n1.y = room.length;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;


                        }

                    }
                    else if (sp.y == room.length && sp.x != room.width)
                    {
                        if (sp.x + fun[i].s_length + min > room.width)
                        {
                            // 오른위 끝 건너뜀
                            sp.y = room.length - min;
                            sp.x = room.width;
                            min_point[i].n1.y = room.length - min;
                            min_point[i].n1.x = room.width;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                        else
                        {
                            sp.x += min;
                            min_point[i].n1.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                    }
                    else if (sp.x == room.width && sp.y != 0)
                    {
                        if (sp.y - fun[i].s_width - min < 0)
                        {
                            sp.y = 0;
                            min_point[i].n1.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }

                    }
                    else if (sp.y == 0 && sp.x != 0)
                    {
                        if (sp.x - fun[i].s_length - min < 0)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.x -= min;
                            min_point[i].n1.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }

                    }
                }

                else if ((fun[i].top != 0 && fun[i].left != 0) || (fun[i].right != 0 && fun[i].bot != 0))
                {
                    if (sp.x == 0 && sp.y != room.length)
                    {
                        if (sp.y + min + fun[i].s_width > room.length)
                        {
                            // 왼위 끝 건너뜀
                            sp.x = min;
                            sp.y = room.length;
                            min_point[i].n1.x = min;
                            min_point[i].n1.y = room.length;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;


                        }

                    }
                    else if (sp.y == room.length && sp.x != room.width)
                    {
                        if (sp.x + fun[i].s_length + min > room.width)
                        {
                            sp.x = room.width;
                            min_point[i].n1.x = room.width;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                        else
                        {
                            sp.x += min;
                            min_point[i].n1.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                    }
                    else if (sp.x == room.width && sp.y != 0)
                    {
                        if (sp.y - fun[i].s_width - min < 0)
                        {
                            // 오른아래 건너뜀
                            sp.x = room.width - min;
                            sp.y = 0;
                            min_point[i].n1.x = room.width - min;
                            min_point[i].n1.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }

                    }
                    else if (sp.y == 0 && sp.x != 0)
                    {
                        if (sp.x - fun[i].s_length - min < 0)
                        {
                            min_point[i].n1.x = -1;
                            return;
                        }
                        else
                        {
                            sp.x -= min;
                            min_point[i].n1.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }

                    }
                }

            }
            else if (fun[i].wallcount == 3)
            {
                // 여유공간이 왼,위,오 거나 왼,오,아 일 때 
                if ((fun[i].left != 0 && fun[i].top != 0 && fun[i].right != 0) || (fun[i].left != 0 && fun[i].right != 0 && fun[i].bot != 0))
                {
                    if (sp.x == 0)
                    {
                        if (sp.y + fun[i].s_width >= room.length - min)
                        {
                            sp.x = room.width;
                            sp.y = room.length - min;
                            min_point[i].n1.x = room.width;
                            min_point[i].n1.y = room.length - min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;

                        }
                        else
                        {
                            sp.y += min;
                            min_point[i].n1.y += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;


                        }
                    }
                    else if (sp.x == room.width)
                    {
                        if (sp.y - fun[i].s_width <= min)
                        {
                            min_point[i].n1.x = -1;
                        }
                        else
                        {
                            sp.y -= min;
                            min_point[i].n1.y -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                    }

                }
                // 여유공간이 왼,위,아 거나 위,오,아 일때 
                else if ((fun[i].left != 0 && fun[i].top != 0 && fun[i].bot != 0) || (fun[i].top != 0 && fun[i].right != 0 && fun[i].bot != 0))
                {
                    if (sp.x == 0)
                    {
                        sp.x = min;
                        sp.y = room.length;
                        min_point[i].n1.x = min;
                        min_point[i].n1.y = room.length;
                    }
                    else if (sp.x == 0 && sp.y == room.length)
                    {
                        if (sp.x + fun[i].s_length >= room.width - min)
                        {
                            sp.x = room.width - min;
                            sp.y = 0;
                            min_point[i].n1.x = room.width - min;
                            min_point[i].n1.y = 0;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }
                        else
                        {
                            sp.x += min;
                            min_point[i].n1.x += min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y - fun[i].width;
                            ToU[i, j].x3.x = sp.x + fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x + fun[i].length;
                            ToU[i, j].x4.y = sp.y - fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y - fun[i].s_width;
                            InC[i, j].x3.x = sp.x + fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x + fun[i].s_length;
                            InC[i, j].x4.y = sp.y - fun[i].s_width;
                        }
                    }
                    else if (sp.y == 0)
                    {
                        if (sp.x - fun[i].s_length <= min)
                        {
                            min_point[i].n1.x = -1;
                        }
                        else
                        {
                            sp.x -= min;
                            min_point[i].n1.x -= min;
                            ToU[i, j].x1.x = sp.x;
                            ToU[i, j].x1.y = sp.y;
                            ToU[i, j].x2.x = sp.x;
                            ToU[i, j].x2.y = sp.y + fun[i].width;
                            ToU[i, j].x3.x = sp.x - fun[i].length;
                            ToU[i, j].x3.y = sp.y;
                            ToU[i, j].x4.x = sp.x - fun[i].length;
                            ToU[i, j].x4.y = sp.y + fun[i].width;

                            InC[i, j].x1.x = sp.x;
                            InC[i, j].x1.y = sp.y;
                            InC[i, j].x2.x = sp.x;
                            InC[i, j].x2.y = sp.y + fun[i].s_width;
                            InC[i, j].x3.x = sp.x - fun[i].s_length;
                            InC[i, j].x3.y = sp.y;
                            InC[i, j].x4.x = sp.x - fun[i].s_length;
                            InC[i, j].x4.y = sp.y + fun[i].s_width;
                        }
                    }


                }
            }
        }

        Console.WriteLine("1번째 좌표 ({0},{1})", ToU[i, saveJ[i]].x1.x, ToU[i, saveJ[i]].x1.y);
        Console.WriteLine("2번째 좌표 ({0},{1})", ToU[i, saveJ[i]].x2.x, ToU[i, saveJ[i]].x2.y);
        Console.WriteLine("3번째 좌표 ({0},{1})", ToU[i, saveJ[i]].x3.x, ToU[i, saveJ[i]].x3.y);
        Console.WriteLine("4번째 좌표 ({0},{1})", ToU[i, saveJ[i]].x4.x, ToU[i, saveJ[i]].x4.y);
        Console.WriteLine("-------------------------------------------------------------------");


    }
}