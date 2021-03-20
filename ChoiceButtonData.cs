using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButtonData : MonoBehaviour
{

    
    public ButtonInfo buttonInfo = new ButtonInfo();

    

    public void OnClickButton()
    {
        ChoiceManager choiceManager = GameObject.Find("ChoiceManager").GetComponent<ChoiceManager>();

        if (choiceManager.b_clicked == true)
        {
            return;
        }
        choiceManager.b_clicked = true;


        if (buttonInfo.광고인가 == true)
        {
            choiceManager.nextScenario = buttonInfo.광고_다음시나리오;
            StartCoroutine(choiceManager.C_CloseChoicePanel());
            return;
        }


        int targetIndex = 0;


        //버튼에 확률을 이용할때 먼저 확률값에 따라서 특정 인덱스를 뽑고
        //특정 인덱스에 해당하는 분기 조건들을 검사하고 만족하면 그 분기에 해당하는 시나리오 출력
        //만족하지 못하면 제일 마지막분기(조건 없는 분기)를 출력
        if (buttonInfo.확률.Length != 0)
        {
            int random = Random.Range(0, 100);
            Debug.Log("뽑힌 랜덤 숫자 : " + random);

            for (int i = 0; i < buttonInfo.다음시나리오.Length; i++)
            {
                if (i == 0)
                {
                    int min = 0;
                    int max = buttonInfo.확률[0];

                    Debug.Log(string.Format("{2}번째분기>> 최소값 : {0}, 최대값 : {1}", min, max, (i + 1)));
                    if (random >= min && random < max)
                    {
                        targetIndex = i;
                    }
                }
                else if (i == buttonInfo.다음시나리오.Length - 1)
                {
                    int min = 0;
                    for (int j = 0; j < (i); j++)
                    {
                        min += buttonInfo.확률[j];
                    }

                    int max = 100;
                    Debug.Log(string.Format("{2}번째분기>> 최소값 : {0}, 최대값 : {1}", min, max, (i + 1)));
                    if (random >= min && random < max)
                    {
                        targetIndex = i;
                    }
                }
                else
                {
                    int min = 0;
                    for (int j = 0; j < (i); j++)
                    {
                        min += buttonInfo.확률[j];
                    }
                    int max = 0;
                    for (int j = 0; j < (i + 1); j++)
                    {
                        max += buttonInfo.확률[j];
                    }

                    Debug.Log(string.Format("{2}번째분기>> 최소값 : {0}, 최대값 : {1}", min, max, (i + 1)));
                    if (random >= min && random < max)
                    {
                        targetIndex = i;
                    }
                }

            }

            bool b_met = true;

            Debug.Log((targetIndex + 1) + "번째 분기로 간다.");


            //스탯으로 나누는 분기가 존재하는가?
            if (buttonInfo.분기_스탯.Length != 0)
            {

                //타겟 인덱스에서 스탯으로 분기를 나누는 값이 입력되어있는가?
                if (buttonInfo.분기_스탯[targetIndex] == "")
                {
                    //비어있으면 걍 넘어간다.
                }
                else
                {
                    //여러 스탯들이 있으면 쉼표로 나눈다.
                    string[] 분기_스탯 = buttonInfo.분기_스탯[targetIndex].Split(',');
                    for (int j = 0; j < 분기_스탯.Length; j++)
                    {
                        //스탯데이터를 다시 헤더와 수치로 나눈다.
                        string 스탯 = 분기_스탯[j].Split('_')[0];
                        int 수치;
                        int.TryParse(분기_스탯[j].Split('_')[1], out 수치);

                        //조건과 플레이어 데이터와 비교한다.
                        switch (스탯)
                        {
                            case "체력": { if (DataManager.GetInstance().autoData.hp < 수치) b_met = false; } break;
                            case "정신력": { if (DataManager.GetInstance().autoData.mp < 수치) b_met = false; } break;
                            case "지능": { if (DataManager.GetInstance().autoData.iq < 수치) b_met = false; } break;
                            case "돈": { if (DataManager.GetInstance().autoData.money < 수치) b_met = false; } break;
                            case "지유호감도": { if (DataManager.GetInstance().autoData.jiyooLove < 수치) b_met = false; } break;
                            case "루리호감도": { if (DataManager.GetInstance().autoData.ruliLove < 수치) b_met = false; } break;
                            case "한나호감도": { if (DataManager.GetInstance().autoData.hannahLove < 수치) b_met = false; } break;
                        }
                        Debug.Log(string.Format("조건{0}:{1}__{2}", 스탯, 수치, b_met));
                    }
                }

            }

            //버튼에 특성으로 분기를 나누는가?
            if (buttonInfo.분기_특성.Length != 0)
            {
                //타겟 인덱스에서 특성으로 분기를 나누는 값이 입력되어있는가?
                if (buttonInfo.분기_특성[targetIndex] == "")
                {
                    //비어있으면 걍 넘어간다.
                }
                else
                {
                    string[] 분기_특성 = buttonInfo.분기_특성[targetIndex].Split(',');
                    for (int j = 0; j < 분기_특성.Length; j++)
                    {
                        if (!DataManager.GetInstance().playerAbility.Contains(분기_특성[j]))
                        {
                            Debug.Log(분기_특성[j] + "를 가지고 있지 않습니다.");
                            b_met = false; break;
                        }
                        else
                        {
                            Debug.Log(분기_특성[j] + "를 가지고 있음");
                        }
                    }
                }

            }


            if (b_met == true)
            {
                choiceManager.nextScenario = buttonInfo.다음시나리오[targetIndex];

            }
            else
            {
                choiceManager.nextScenario = buttonInfo.다음시나리오[buttonInfo.다음시나리오.Length - 1];
            }



        }

        //확률을 이용하지 않을때  처음에 기입된 분기 조건부터 검사하고
        //만약에 분기 조건이 만족되는 대로 break를 걸고 시나리오를 출력함.
        else
        {
            bool b_met = false;

            for (int i = 0; i < buttonInfo.다음시나리오.Length - 1; i++)
            {
                bool b_elementMet = true;

                //이 버튼은 스탯으로 분기를 나누는가?
                if (buttonInfo.분기_스탯.Length != 0)
                {
                    //해당 분기에 대한 스탯분기가 작성되어있는가?
                    if (buttonInfo.분기_스탯[i] == "")
                    {
                        //없으면 넘어간다.
                    }

                    //해당 분기에 대한 스탯분기가 작성되어있는가?
                    else
                    {
                        //만약 스탯 조건이 여러개라면 쉼표로 스탯데이터를 나눈다.
                        string[] 분기_스탯 = buttonInfo.분기_스탯[i].Split(',');

                        //분기 특성 확률을 스탯별로 확인한다.
                        for (int j = 0; j < 분기_스탯.Length; j++)
                        {
                            string 스탯 = 분기_스탯[j].Split('_')[0];
                            int 수치;
                            int.TryParse(분기_스탯[j].Split('_')[1], out 수치);
                            switch (스탯)
                            {
                                case "체력": { if (DataManager.GetInstance().autoData.hp < 수치) b_elementMet = false; } break;
                                case "정신력": { if (DataManager.GetInstance().autoData.mp < 수치) b_elementMet = false; } break;
                                case "지능": { if (DataManager.GetInstance().autoData.iq < 수치) b_elementMet = false; } break;
                                case "돈": { if (DataManager.GetInstance().autoData.money < 수치) b_elementMet = false; } break;
                                case "지유호감도": { if (DataManager.GetInstance().autoData.jiyooLove < 수치) b_elementMet = false; } break;
                                case "루리호감도": { if (DataManager.GetInstance().autoData.ruliLove < 수치) b_elementMet = false; } break;
                                case "한나호감도": { if (DataManager.GetInstance().autoData.hannahLove < 수치) b_elementMet = false; } break;
                            }
                            Debug.Log(string.Format("조건{0}:{1}__{2}", 스탯, 수치, b_elementMet));
                        }
                    }

                }

                if (buttonInfo.분기_특성.Length != 0)
                {
                    if (buttonInfo.분기_특성[i] == "")
                    {

                    }
                    else
                    {
                        string[] 분기_특성 = buttonInfo.분기_특성[i].Split(',');
                        for (int j = 0; j < 분기_특성.Length; j++)
                        {
                            if (!DataManager.GetInstance().playerAbility.Contains(분기_특성[j]))
                            {
                                Debug.Log(분기_특성[j] + "를 가지고 있지 않습니다.");
                                b_elementMet = false;
                                break;
                            }
                            else
                            {
                                Debug.Log(분기_특성[j] + "를 가지고 있습니다");
                            }
                        }
                    }
                }

                if (b_elementMet == true)
                {
                    b_met = true;
                    choiceManager.nextScenario = buttonInfo.다음시나리오[i];
                    break;
                }
            }

            if (b_met == false)
            {
                choiceManager.nextScenario = buttonInfo.다음시나리오[buttonInfo.다음시나리오.Length - 1];
            }
        }

        StartCoroutine(choiceManager.C_CloseChoicePanel());

    }



}
