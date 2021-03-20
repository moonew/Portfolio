using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class ChoiceManager : MonoBehaviour
{


    public ChoiceInfo       choiceInfo;
    public GameObject       choicePanel;
    public GameObject       choiceButtonPrefab;









    AsyncOperationHandle    choiceHandle;


    public bool     b_runningCoroutine      = false;
    public bool     b_clicked               = false;
    public string   nextScenario;



    IEnumerator C_GetChoiceData(string _fileName)
    {


        choiceHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/ChoiceData/"+_fileName+".json");
        yield return choiceHandle;


        string jsonData         = choiceHandle.Result.ToString();
        string container        = jsonData.ToString();


        choiceInfo = JsonUtility.FromJson<ChoiceInfo>(container);






    }



    IEnumerator C_SetChoiceData()
    {

        Text questionText                                               = choicePanel.transform.GetChild(0).GetComponent<Text>();
        questionText.text                                               = choiceInfo.questionSting;



        if(choiceInfo.questionSting == "이어 하시겠습니까?")
        {

            string newQuestionString = "이어 하시겠습니까?" + "<size=50><color=#ffb8c6>   메모리얼티켓 : " + DataManager.GetInstance().autoData.memorialTicket + "장</color></size>";
            questionText.text = newQuestionString;

        }


        Transform  choiceButtonTransform  =  choicePanel.transform.GetChild(1);
        


        for (int i = 0; i < choiceInfo.buttonInfo.Length; i++)
        {


            GameObject choiceButton                                     = Instantiate(choiceButtonPrefab, choiceButtonTransform);

            ButtonInfo buttonInfo                                       = choiceInfo.buttonInfo[i];
            choiceButton.GetComponent<ChoiceButtonData>().buttonInfo    = buttonInfo;


            Text      choiceText                                        = choiceButton.transform.GetChild(0).GetComponent<Text>();
            Text      choiceBottomText                                  = choiceButton.transform.GetChild(1).GetComponent<Text>();
            Text      conditionText                                     = choiceButton.transform.GetChild(2).GetComponent<Text>();


            GameObject adsImage                                         = choiceButton.transform.GetChild(3).GetChild(0).gameObject;
            GameObject diceImage                                        = choiceButton.transform.GetChild(3).GetChild(1).gameObject;



            choiceText.text                                             = buttonInfo.선택지;
            choiceBottomText.text                                       = buttonInfo.광고_보상;


            if (buttonInfo.광고인가 == true) adsImage.SetActive(true);
            else if (buttonInfo.확률성선택지인가 == true) diceImage.SetActive(true);





            //체크 및 활성화 관련
            {


               if( choiceInfo.questionSting == "이어 하시겠습니까?")
                {

                    if(DataManager.GetInstance().autoData.memorialTicket<1)
                    {

                        choiceButtonTransform.GetChild(0).GetComponent<Button>().interactable = false;

                    }

                }


















                bool b_activate = true;




                if(buttonInfo.활성화조건_스탯 != "")
                {


                    string[] activeStatCondtion = buttonInfo.활성화조건_스탯.Split(',');




                    for (int j = 0; j < activeStatCondtion.Length; j++)
                    {

                        string stat     = activeStatCondtion[j].Split('_')[0];
                        int statAmount; int.TryParse(activeStatCondtion[j].Split('_')[1], out statAmount);



                        switch (stat)
                        {

                            case "체력":
                                {

                                    if (DataManager.GetInstance().autoData.hp >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;



                            case "정신력":
                                {

                                    if (DataManager.GetInstance().autoData.mp >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";
                                        
                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;


                            case "지능":
                                {

                                    if (DataManager.GetInstance().autoData.iq >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;



                            case "돈":
                                {

                                    if (DataManager.GetInstance().autoData.money >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;



                            case "지유호감도":
                                {

                                    if (DataManager.GetInstance().autoData.jiyooLove >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;



                            case "루리호감도":
                                {

                                    if (DataManager.GetInstance().autoData.ruliLove >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;



                            case "한나호감도":
                                {

                                    if (DataManager.GetInstance().autoData.hannahLove >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;



                            case "메모리얼티켓":
                                {

                                    if (DataManager.GetInstance().autoData.memorialTicket >= statAmount)
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=green>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=green>" + stat + "</color>";

                                    }
                                    else
                                    {

                                        if (conditionText.text =="") conditionText.text += "<color=red>" + stat + "</color>";
                                        else conditionText.text += "\n" + "<color=red>" + stat + "</color>";



                                        b_activate = false;
                                    }

                                }
                                break;

                        }

                    }

                }
                




                if(buttonInfo.활성화조건_특성 != "")
                {


                    string[] activeAbilityCondtion = buttonInfo.활성화조건_특성.Split(',');




                    for (int j = 0; j < activeAbilityCondtion.Length; j++)
                    {


                        string ability = activeAbilityCondtion[j];
                        ability        = ability.Trim();


                        if(DataManager.GetInstance().playerAbility.Contains(ability))
                        {

                            if (conditionText.text =="") conditionText.text += "<color=green>" + ability + "</color>";
                            else conditionText.text        += "\n" + "<color=green>" + ability + "</color>";

                        }
                        else
                        {

                            if (conditionText.text =="") conditionText.text += "<color=red>" + ability + "</color>";
                            else conditionText.text += "\n" + "<color=red>" + ability + "</color>";



                            b_activate = false;
                        }



                    }

                }





                if (buttonInfo.비활성화조건_디버프 != "")
                {


                    string[] debuffCondtion = buttonInfo.비활성화조건_디버프.Split(',');




                    for (int j = 0; j < debuffCondtion.Length; j++)
                    {


                        string ability = debuffCondtion[j];
                        ability = ability.Trim();


                        if (DataManager.GetInstance().playerAbility.Contains(ability))
                        {

                            if (conditionText.text == "") conditionText.text += "<color=red>" + ability + "</color>";
                            else conditionText.text += "\n" + "<color=red>" + ability + "</color>";




                            b_activate = false;
                        }

                    }

                }




                if (b_activate==false)
                {

                    choiceButton.GetComponent<Button>().interactable = false;

                }


            }










        }

        yield return null;


    }




    public IEnumerator C_OpenChoicePanel(string _fileName)
    {

        b_runningCoroutine = true;



        //초기화
        foreach(Transform child in choicePanel.transform.GetChild(1))
        {
            Destroy(child.gameObject);
        }

        b_clicked       = false;

        nextScenario    = "";








        choicePanel.SetActive(true);


        CanvasGroup choicePanelCanvasGroup = choicePanel.GetComponent<CanvasGroup>();


        yield return StartCoroutine(C_GetChoiceData(_fileName));


        yield return StartCoroutine(C_SetChoiceData());


        yield return StartCoroutine(C_FadeIn(choicePanelCanvasGroup));


        b_runningCoroutine = false;

    }




    public IEnumerator C_CloseChoicePanel()
    {
        

        b_runningCoroutine = true;


        CanvasGroup choicePanelCanvasGroup = choicePanel.GetComponent<CanvasGroup>();


        yield return StartCoroutine(C_FadeOut(choicePanelCanvasGroup));


        choicePanel.SetActive(false);


        b_runningCoroutine = false;


        GameObject.Find("DialogueManager").GetComponent<DialogueManager>().GetDialogue(nextScenario);

    }












    [ContextMenu("게임오버")]
    void Test()
    {

        StartCoroutine(C_OpenChoicePanel("C_END1"));

    }






    #region SupportFunction
    IEnumerator C_FadeIn(Image _targetImage, float _drawingTime = 0.75f)
    {
        float nowTime = 0;
        float targetTime = _drawingTime;
        float a;

        while (nowTime <= targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(0, 1, nowTime / targetTime);
            _targetImage.color = new Color(1, 1, 1, a);
            yield return null;
        }
    }


    IEnumerator C_FadeOut(Image _targetImage, float _drawingTime = 0.75f)
    {
        float nowTime = 0;
        float targetTime = _drawingTime;
        float a;

        while (nowTime <= targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(1, 0, nowTime / targetTime);
            _targetImage.color = new Color(1, 1, 1, a);
            yield return null;
        }
    }


    IEnumerator C_FadeIn(CanvasGroup _canvasGroup, float _drawingTime = 0.75f)
    {
        float nowTime = 0;
        float targetTime = _drawingTime;
        float a;


        while (nowTime <= targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(0, 1, nowTime / targetTime);
            _canvasGroup.alpha = a;
            yield return null;
        }

    }


    IEnumerator C_FadeOut(CanvasGroup _canvasGroup, float _drawingTime = 0.75f)
    {


        float nowTime = 0;
        float targetTime = _drawingTime;
        float a;
        

        while (nowTime <= targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(1, 0, nowTime / targetTime);
            _canvasGroup.alpha = a;
            yield return null;
        }
    }


    #endregion
}









[System.Serializable]
public struct ButtonInfo
{

    //출력될 선택지 스트링
    public string 선택지;

    //광고가 필요한 선택지인가? 맞다면 비디오마크가 떠야한다.

    public bool 확률성선택지인가;

    public bool 광고인가;

    public string 광고_보상;

    public string 광고_다음시나리오;





    //선택지 활성화 스탯조건과 수치
    public string 활성화조건_스탯;

    //버튼 활성화 특성 조건
    public string 활성화조건_특성;
    //선택지 비활성화 특성 조건
    public string 비활성화조건_디버프;

    //다음 시나리오들
    public string[] 다음시나리오;

    //해당 시나리오로 갈 수 있는 확률
    public int[] 확률;

    //해당 시나리오로 갈 수 있는 조건 스텟과 수치
    public string[] 분기_스탯;

    //해당 시나리오로 갈 수 있는 조건 스텟과 수치
    public string[] 분기_특성;

}


[System.Serializable]
public class ChoiceInfo
{

    public string questionSting;
    public ButtonInfo[] buttonInfo;
}