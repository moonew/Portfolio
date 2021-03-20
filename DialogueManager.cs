using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class DialogueManager : MonoBehaviour
{


    //매니저클래스 참조
    public SpriteManager spriteManager;
    public SoundManager  soundManager;




    //오브젝트 참조
    public GameObject       dialogueBox;
    public GameObject       skipButton;
    public Sprite[]         nameBoxSprite;
    
    


    public CanvasGroup      sideUI;
    public CanvasGroup      upperUI;




    //Dialogue Talk에 관련된 변수들
    public 스크립트   dialogue;
    public string     dialogueName;
    public int        index                 = 0;
    public bool       b_readyToClick        = true;
    bool              b_tailFlicking        = false;
    




    //핸들
    AsyncOperationHandle<TextAsset> dialogueHandle;
    Coroutine   flickDialgoueTailCoroutine;
    Coroutine   autoClickCoroutine;

















    //스크립트 로드
    public void GetDialogue(string _fileName, int _index = 0)
    {

        StartCoroutine(C_GetDialogue(_fileName, _index));

    }
       
    


    IEnumerator C_GetDialogue(string _fileName, int _index = 0)
    {


        if(dialogueHandle.IsValid()==true)
        {

            Addressables.Release(dialogueHandle);

        }

        yield return null;






        index = _index;


        dialogueName = _fileName;


        dialogueHandle = Addressables.LoadAssetAsync<TextAsset>(_fileName);
        yield return dialogueHandle;



        string jsonData     = dialogueHandle.Result.ToString();
        dialogue            = JsonUtility.FromJson<스크립트>(jsonData);




        //스킵치트키가 활성화 되어있을때
        if(DataManager.GetInstance().CheckSkipCheatKey()==true)
        {

            skipButton.GetComponent<Button>().interactable = true;

        }


        //스킵치트키가 활성화 되어있지 않을때
        else
        {

            //봤던 스크립트이면 시나리오 활성화
            if (DataManager.GetInstance().CheckSkipButton(_fileName) == true)
            {

                skipButton.GetComponent<Button>().interactable = true;


            }

            else
            {

                skipButton.GetComponent<Button>().interactable = false;

            }

        }

      


        b_readyToClick = true;
               


        StartCoroutine(C_Click());



        DataManager.GetInstance().SaveAutoData();



        yield return new WaitForSeconds(0.5f);



        GameObject.Find("SkipController").GetComponent<SkipController>().b_alreadySkipped = false;


    }
          



 



    public void Click()
    {

        if(DataManager.GetInstance().optionData.b_touchMode==true) StartCoroutine(C_Click());
        else
        {
            GameObject.Find("OptionManager").GetComponent<OptionManager>().OnClickTouchMode();

        }

    }




   IEnumerator C_Click()
    {

        //대화 인덱스가 끝나면 클릭을 했을 경우 Yield break 된다.
        if (dialogue.단위.Length == index)
        {


            if (flickDialgoueTailCoroutine != null)
            {
                StopCoroutine(flickDialgoueTailCoroutine);
            }
            RemoveDialogueTail();


            yield return StartCoroutine(GameObject.Find("EventController").GetComponent<EventController>().C_OpenNextDayPanel());
            


            yield break;

        }

        else if(dialogue.단위.Length-1 == index)
        {


            DataManager.GetInstance().ActiveSkipButton(dialogueName);


        }




        //중복방지 코드
        if (b_readyToClick == false) yield break; b_readyToClick = false;




        //로그 추가
        GameObject.Find("LogPanelController").GetComponent<LogPanelController>().AddLog();



        //다음 대사 나올 때 기존 대사들은 멈춤
        soundManager.JiyooSpeaker.Stop();
        soundManager.RuliSpeaker.Stop();
        soundManager.HannahSpeaker.Stop();




        //이름 박스 관련 처리
        {

            if (dialogue.단위[index].이름 != "")
            {

                GameObject nameBox = dialogueBox.transform.GetChild(0).gameObject;


                switch (dialogue.단위[index].이름)
                {

                    case "최범"       : { nameBox.GetComponent<Image>().sprite = nameBoxSprite[0]; } break;
                    case "김지유"     : { nameBox.GetComponent<Image>().sprite = nameBoxSprite[1]; } break;
                    case "강한나"     : { nameBox.GetComponent<Image>().sprite = nameBoxSprite[2]; } break;
                    case "이루리"     : { nameBox.GetComponent<Image>().sprite = nameBoxSprite[3]; } break;
                    default           : { nameBox.GetComponent<Image>().sprite = nameBoxSprite[4]; } break;

                }


                Text nameText                                = nameBox.transform.GetChild(0).GetComponent<Text>();
                nameText.text                                = dialogue.단위[index].이름;
                nameBox.GetComponent<CanvasGroup>().alpha    = 1;

            }
            else
            {

                //나레이션을 하고 있을 때는 네임박스를 가린다.
                if (dialogue.단위[index].대사 != "")
                {

                    GameObject nameBox                          = dialogueBox.transform.GetChild(0).gameObject;
                    nameBox.GetComponent<CanvasGroup>().alpha   = 0;

                }

            }

        }


        //명령어 처리
        if(dialogue.단위[index].명령어 != "")
        {

            string head           = dialogue.단위[index].명령어.Split('(', ')')[0];
            string[] parameter    = dialogue.단위[index].명령어.Split('(', ')')[1].Split('/');


            //연출상의 명령어가 아닌 것들 
            {

                switch (head)
                {

                    case "시나리오출력":
                        {

                            string fileName = parameter[0];
                            GetDialogue(fileName);
                            yield break;

                        }


                    case "선택지출력":
                        {

                            string fileName = parameter[0];


                            yield return StartCoroutine(GameObject.Find("ChoiceManager").GetComponent<ChoiceManager>().C_OpenChoicePanel(fileName));
                            yield break;


                        }


                    case "알바선택지출력":
                        {


                            yield return StartCoroutine(PartTimeJobChoiceManager.GetInstance().C_OpenPartTimeJobPanel());

                            yield break;

                        }
                        

                    case "가챠출력":
                        {

                            string fileName = parameter[0];
                            

                            yield return StartCoroutine(GameObject.Find("GatchaManager").GetComponent<GatchaManager>().C_OpenGatcha(fileName));
                            yield break;
                        }


                    case "문자출력":
                        {

                            string fileName = parameter[0];
                            StartCoroutine(GameObject.Find("CellphoneController").GetComponent<CellphoneController>().C_GetCellphoneDialogue(fileName));
                            yield break;

                        }


                    case "수요일이벤트출력":
                        {

                            StartCoroutine (WednesdayManager.GetInstance().C_OpenWednesdayCellphone());
                            yield break;

                        }



                    case "게임오버":
                        {

                            yield return new WaitForSeconds(1.75f);
                            DataManager.GetInstance().GameOver();
                            yield break;
                        }


                    case "게임오버바로":
                        {

                            DataManager.GetInstance().GameOver();
                            yield break;
                        }



                    case "엔딩크레딧출력":
                        {

                            EndingController.GetInstance().PlayEndingCredit();
                            yield break;

                        }


                    case "쉰다스크립트출력":
                        {

                            
                            int randomNumber = Random.Range(1, 11);

                            while(randomNumber==3 || randomNumber ==7)
                            {

                                randomNumber = Random.Range(1, 11);

                            }


                            string fileName = "FUN_EVENT_" + randomNumber;
                            GetDialogue(fileName);
                            yield break;

                        }


                    case "공부한다스크립트출력":
                        {

                            int randomNumber = Random.Range(1, 14);
                            if(randomNumber==9)
                            {
                                if(DataManager.GetInstance().autoData.jiyooIndex==7)
                                {
                                    randomNumber = 1;
                                }
                            }

                            while (randomNumber == 2 || randomNumber == 6 || randomNumber == 7)
                            {
                                randomNumber = Random.Range(1, 14);
                            }

                            string fileName = "STUDY_EVENT_" + randomNumber;
                            GetDialogue(fileName);
                            yield break;

                        }

                }

            }


            yield return StartCoroutine(C_Command());

        }


        //대사가 없을 경우
        if(dialogue.단위[index].대사 == "")
        {

            index++;
            b_readyToClick = true;
            StartCoroutine(C_Click());
            yield break;

        }


        else if(dialogue.단위[index].대사 != "")
        {

            yield return StartCoroutine(C_Typing(dialogue.단위[index].대사));

        }





        //키보드 난타 방지
        yield return new WaitForSeconds(0.2f);





        b_readyToClick = true;


        index++;


    }
   



    //해당 시나리오 연출상의 명령어들
    IEnumerator C_Command()
    {
        
        string head         = dialogue.단위[index].명령어.Split('(', ')')[0];
        string[] parameter  = dialogue.단위[index].명령어.Split('(', ')')[1].Split('/');


        switch(head)
        {



            //캐릭터 출력 관련
            case "캐릭터출력":
                {

                    if(parameter.Length==4)
                    {

                        string charInfo = parameter[0];
                        int pose;       int.TryParse(parameter[1], out pose);
                        int clothe;     int.TryParse(parameter[2], out clothe);
                        int expression; int.TryParse(parameter[3], out expression);


                        yield return StartCoroutine(spriteManager.C_DrawCharacter(charInfo, pose, clothe, expression));

                    }
                    else if(parameter.Length==5)
                    {

                        string charInfo = parameter[0];
                        int pose;           int.TryParse(parameter[1], out pose);
                        int clothe;         int.TryParse(parameter[2], out clothe);
                        int expression;     int.TryParse(parameter[3], out expression);
                        float drawingTime;  float.TryParse(parameter[4], out drawingTime);


                        yield return StartCoroutine(spriteManager.C_DrawCharacter(charInfo, pose, clothe, expression, drawingTime));

                    }


                } break;


            case "엑스트라출력":
                {

                    if(parameter.Length==1)
                    {

                        string charInfo = parameter[0];


                        yield return StartCoroutine(spriteManager.C_DrawCharacter(charInfo));

                    }
                    else if(parameter.Length==2)
                    {

                        string charInfo         = parameter[0];
                        float drawingTime;      float.TryParse(parameter[1], out drawingTime);


                        yield return StartCoroutine(spriteManager.C_DrawCharacter(charInfo, drawingTime));

                    }

                } break;


            case "캐릭터이동":
                {

                    if(parameter.Length==3)
                    {

                        string charName         = parameter[0];
                        float targetPosX;       float.TryParse(parameter[1], out targetPosX);
                        float speed;            float.TryParse(parameter[2], out speed);


                        yield return StartCoroutine(spriteManager.C_MoveCharacter(charName, targetPosX, speed));

                    }

                    //캐릭터가 이동하면서 제거됨
                    else if(parameter.Length==4)
                    {

                        string charName         = parameter[0];
                        float targetPosX;       float.TryParse(parameter[1], out targetPosX);
                        float speed;            float.TryParse(parameter[2], out speed);
                        string fade             = parameter[3];


                        yield return StartCoroutine(spriteManager.C_MoveCharacter(charName, targetPosX, speed, fade));

                    }

                }break;
                

            case "캐릭터끄덕":
                {

                    string charName = parameter[0];


                    yield return StartCoroutine(spriteManager.C_NodCharacter(charName));


                }break;
                

            case "캐릭터제거":
                {

                    if(parameter.Length==1)
                    {

                        string charName = parameter[0];


                        yield return StartCoroutine(spriteManager.C_RemoveCharacter(charName));

                    }

                    else if(parameter.Length==2)
                    {

                        string charName         = parameter[0];
                        float deletingTime;     float.TryParse(parameter[1], out deletingTime);


                        yield return StartCoroutine(spriteManager.C_RemoveCharacter(charName, deletingTime));

                    }
                    
                    

                   
                }break;
              

            case "캐릭터모두제거":
                {

                    if (parameter[0] == "")
                    {

                        yield return StartCoroutine(spriteManager.C_RemoveAllCharacter());

                    }

                    else 
                    {
                        
                        float deletingTime; float.TryParse(parameter[0], out deletingTime);


                        yield return StartCoroutine(spriteManager.C_RemoveAllCharacter(deletingTime));

                    }

                }
                break;
               

            case "이모티콘출력":
                {

                    string charName     = parameter[0];
                    string emotion      = parameter[1];


                    StartCoroutine(spriteManager.C_PlayEmoticon(charName, emotion));

                }break;











            //배경 관련
            case "배경출력":
                {

                    if (parameter.Length == 1)
                    {
                        string fileName = parameter[0];


                        yield return StartCoroutine(spriteManager.C_DrawBackground(fileName));

                    }

                    else if (parameter.Length == 2)
                    {

                        string fileName = parameter[0];
                        float drawingTime; float.TryParse(parameter[1], out drawingTime);


                        yield return StartCoroutine(spriteManager.C_DrawBackground(fileName, drawingTime));

                    }


                }break;


            case "배경제거":
                {

                    if(parameter[0]=="")
                    {

                        yield return StartCoroutine(spriteManager.C_RemoveBackground());

                    }
                    else if(parameter[0]!= "")
                    {

                        float deletingTime; float.TryParse(parameter[0], out deletingTime);


                        yield return StartCoroutine(spriteManager.C_RemoveBackground(deletingTime));

                    }

                }break;


            case "트랜지션출력":
                {

                    if (parameter.Length == 1)
                    {
                        string fileName = parameter[0];


                        yield return StartCoroutine(spriteManager.C_Transition(fileName));

                    }

                    else if (parameter.Length == 2)
                    {

                        string fileName = parameter[0];
                        float drawingTime; float.TryParse(parameter[1], out drawingTime);


                        yield return StartCoroutine(spriteManager.C_Transition(fileName, drawingTime));

                    }

                }
                break;


            case "날씨출력":
                {

                    string weather = parameter[0];

                    GameObject.Find("WeatherController").GetComponent<WeatherController>().TurnOnWeather(weather);

                }break;


            case "날씨제거":
                {

                    GameObject.Find("WeatherController").GetComponent<WeatherController>().TurnOffWeather();

                }
                break;


            case "SD출력":
                {

                    string fileName = parameter[0];
                    yield return StartCoroutine(spriteManager.C_DrawSd(fileName));

                }break;


            case "SD제거":
                {

                    yield return StartCoroutine(spriteManager.C_RemoveSd());

                }
                break;


            case "이벤트씬출력":
                {

                    string fileName = parameter[0];
                    yield return StartCoroutine(spriteManager.C_DrawCinematic(fileName));

                }
                break;


            case "이벤트씬제거":
                {

                    yield return StartCoroutine(spriteManager.C_RemoveCinematic());

                }
                break;


            case "오브젝트출력":
                {

                    string fileName = parameter[0];
                    yield return StartCoroutine(spriteManager.C_DrawObject(fileName));

                }
                break;


            case "오브젝트제거":
                {

                    yield return StartCoroutine(spriteManager.C_RemoveObject());

                }
                break;


            case "글씨효과출력":
                {

                    string letter = parameter[0];
                    float posX; float.TryParse(parameter[1], out posX);
                    float posY; float.TryParse(parameter[2], out posY);


                   StartCoroutine( LetterAnimController.GetInstance().C_DrawingLetter(letter, posX, posY));


                }break;


            case "거리":
                {

                    int distance; int.TryParse(parameter[0], out distance);
                    spriteManager.distance = distance;

                }break;

            case "위치":
                {
                    float posX; float.TryParse(parameter[0], out posX);
                    spriteManager.posX = posX;
                }
                break;





            //특수효과

            case "이의있소":
                {
                    yield return StartCoroutine(CameraShaker.GetInstance().C_Flash());
                }
                break;

            case "진동출력":
                {
                    //진동출력(진동폭/스피드/무조건 홀수로 해야함)

                    float arrange, speed;
                    int shakeTime;
                    float.TryParse(parameter[0], out arrange);
                    float.TryParse(parameter[1], out speed);
                    int.TryParse(parameter[2], out shakeTime);

                    StartCoroutine(CameraShaker.GetInstance().C_ShakeTest(arrange, speed, shakeTime));
                }
                break;
            case "좌우진동출력":
                {
                    //진동출력(진동폭/스피드/무조건 홀수로 해야함)

                    float arrange, speed;
                    int shakeTime;
                    float.TryParse(parameter[0], out arrange);
                    float.TryParse(parameter[1], out speed);
                    int.TryParse(parameter[2], out shakeTime);

                    StartCoroutine(CameraShaker.GetInstance().C_ShakeTest_Side(arrange, speed, shakeTime));
                }
                break;






            //음성 관련
            case "배경음출력":
                {

                    int fileNumber; int.TryParse(parameter[0], out fileNumber);
                    StartCoroutine(soundManager.C_PlayBgm(fileNumber));


                }break;


            case "배경음제거":
                {

                    StartCoroutine(soundManager.C_StopBgm());

                }break;


            case "효과음출력":
                {

                    int fileNumber; int.TryParse(parameter[0], out fileNumber);
                    StartCoroutine(soundManager.C_PlayEffect(fileNumber));

                }
                break;


            case "효과음제거":
                {

                    StartCoroutine(soundManager.C_StopEffect());

                }
                break;


            case "대사출력":
                {
                    if (parameter.Length == 1)
                    {

                        string fileName = parameter[0];
                        StartCoroutine(soundManager.C_PlayVoice(fileName));

                    }
                    else if(parameter.Length == 2)
                    {

                        string fileName = parameter[0];
                        string charName = parameter[1];
                        StartCoroutine(soundManager.C_PlayVoice(fileName, charName));

                    }


                }break;








            //특성획득
            case "스탯변화":
                {

                    string stat = parameter[0];


                    if (parameter[1].Split('~').Length == 2)
                    {


                        string min = parameter[1].Split('~')[0];
                        string max = parameter[1].Split('~')[1];

                        int minContainer; int.TryParse(min, out minContainer);
                        int maxContainer; int.TryParse(max, out maxContainer);

                        int amount = Random.Range(minContainer, maxContainer);

                        StartCoroutine(GameObject.Find("PlayerDataController").GetComponent<PlayerDataController>().C_ShowStatChange(stat, amount));

                    }
                    else
                    {

                        int amount; int.TryParse(parameter[1], out amount);
                        StartCoroutine(GameObject.Find("PlayerDataController").GetComponent<PlayerDataController>().C_ShowStatChange(stat, amount));


                    }



                } break;



            case "특성변화":
                {

                    string ability = parameter[0];

                    bool b_obtain = parameter[1] == "1" ? true : false;

                    StartCoroutine(GameObject.Find("PlayerDataController").GetComponent<PlayerDataController>().C_ShowAbilityChange(ability, b_obtain));

                } break;



            case "이어하기차감":
                {
                    DataManager.GetInstance().autoData.memorialTicket--;

                }break;










            //기타
            case "튜토리얼스킵":
                {



                }break;

        }


    }




    //타이핑
    IEnumerator C_Typing(string _sentence)
    {

        Text dialogueText        = dialogueBox.transform.GetChild(1).GetComponent<Text>();
        float typingTime;


        if(dialogue.단위[index].대사연결 == false)  dialogueText.text = "";

        
        switch(DataManager.GetInstance().optionData.typingSpeedLevel)
        {

            case 1: { typingTime = 0.05f; }break;
            case 2: { typingTime = 0.025f; } break;
            case 3: { typingTime = 0.01f; } break;
            case 4: { typingTime = 0.001f; } break;
            default: { Debug.Log("yield break"); yield break; }

        }


        foreach(char word in _sentence)
        {

            if(word=='/')
            {

                dialogueText.text += "\n";

            }

            else
            {

                switch (dialogue.단위[index].글자색)
                {
                    case 색깔.검정: { dialogueText.text += word; } break;
                    case 색깔.빨강: { dialogueText.text += "<color=red>" + word + "</color>"; } break;
                    case 색깔.초록: { dialogueText.text += "<color=green>" + word + "</color>"; } break;
                }

            }
            
            yield return new WaitForSeconds(typingTime);

        }


        if(DataManager.GetInstance().optionData.b_touchMode == true)
        {
            
            flickDialgoueTailCoroutine = StartCoroutine(C_FlickDialogueTail());

        }
        else
        {

            autoClickCoroutine = StartCoroutine(C_AutoClick());

        }

    }




    //다이얼로그 테일 깜빡이는 효과
    IEnumerator C_FlickDialogueTail()
    {

        if (b_tailFlicking == true) yield break; b_tailFlicking = true;

        yield return new WaitForSeconds(1f);


        Image dialogueTail  = dialogueBox.transform.GetChild(2).GetComponent<Image>();


        bool b_fadeIn       = true;


        float nowTime           = 0;
        float targetTime        = 0.65f;
        float delayTime         = 0.95f;


        float targetAlpha      = 0.8f;
        float alpha;




        while (true)
        {

            //깜빡이던 중, 클릭을 하면 알파 값이 0되고 깜빡임이 멈춘다. 
            if(b_readyToClick==false)
            {

                dialogueTail.color      = new Color(1, 1, 1, 0);
                b_tailFlicking          = false;
                yield break;

            }




            nowTime += Time.deltaTime;



            //페이드 인 또는 페이드 아웃에 소요되는 시간
            if(nowTime<=targetTime)
            {

                if (b_fadeIn)
                {

                    alpha = Mathf.Lerp(0, targetAlpha, nowTime / targetTime);
                    dialogueTail.color = new Color(1, 1, 1, alpha);

                }


                else if (!b_fadeIn)
                {

                    alpha = Mathf.Lerp(targetAlpha, 0, nowTime / targetTime);
                    dialogueTail.color = new Color(1, 1, 1, alpha);

                }

            }


            //페이드 인 또는 페이드 아웃이 되고난 후 잠시동안 딜레이 되는 시간
            else if(nowTime<= delayTime)
            {

                yield return null;
                
            }


            //딜레이 시간이 끝나면, 시간을 초기화 한다.
            else if(nowTime>delayTime)
            {

                nowTime = 0;
                b_fadeIn = !b_fadeIn;

            }


            yield return null;

        }

    }


    //다이얼로그 테일 없애기
    void RemoveDialogueTail()
    {

        Image dialogueTail      = dialogueBox.transform.GetChild(2).GetComponent<Image>();
        dialogueTail.color      = new Color(1, 1, 1, 0);
        b_tailFlicking          = false;

    }



    //오토모드 감지 후 자동클릭 활성화
    public IEnumerator C_AutoClick()
    {

        if (DataManager.GetInstance().optionData.b_touchMode == true) yield break;


        float nowTime       = 0;
        float targetTime    = 1.4f;


        while (true)
        {
            if (b_readyToClick == true && soundManager.b_talking == false)
            {
                while (nowTime <= targetTime)
                {
                    nowTime += Time.deltaTime;
                    if (DataManager.GetInstance().optionData.b_touchMode == true)
                    {
                        StartCoroutine(C_FlickDialogueTail());
                        yield break;
                    }
                    yield return null;
                }
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(C_Click());



    }



    //대화창 숨기기
    public void HideDialogueBox()
    {

        b_readyToClick = false;
        dialogueBox.GetComponent<CanvasGroup>().alpha = 0;


        sideUI.interactable = false;
        sideUI.alpha = 0;


        upperUI.alpha = 0;


        Transform interactableUI = GameObject.Find("InteractableUI").transform.GetChild(3);
        interactableUI.gameObject.SetActive(true);


    }



    //대화창 열기
    public void OpenDialogueBox()
    {

        b_readyToClick = true;
        dialogueBox.GetComponent<CanvasGroup>().alpha = 1;


        sideUI.interactable = true;
        sideUI.alpha = 1;


        upperUI.alpha = 1;


        Transform interactableUI = GameObject.Find("InteractableUI").transform.GetChild(3);
        interactableUI.gameObject.SetActive(false);

    }











    #region Support Function

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





    public void OnAutoClick(int _panel)
    {
        StartCoroutine(C_OnAutoClick(_panel));
    }


    IEnumerator C_OnAutoClick(int _pannel)
    {

        if(DataManager.GetInstance().optionData.b_touchMode==false)
        {

            if (_pannel == (int)패널.옵션)
            {
                OptionManager optionManager = GameObject.Find("OptionManager").GetComponent<OptionManager>();




                while (optionManager.b_runningCoroutine == true) yield return null;


                autoClickCoroutine = StartCoroutine(C_AutoClick());
            }

            else if (_pannel == (int)패널.핸드폰)
            {

                CellphoneController cellphoneController = GameObject.Find("CellphoneController").GetComponent<CellphoneController>();




                while (cellphoneController.b_runningCoroutine == true) yield return null;


                autoClickCoroutine = StartCoroutine(C_AutoClick());

            }

        }

        

    }


    public void OffAutoClick()
    {

        if (DataManager.GetInstance().optionData.b_touchMode == false)
            StopCoroutine(autoClickCoroutine);


    }

    #endregion






    [ContextMenu("불러오기 테스트")]
    void TestFlickingDialogueTail()
    {

        GetDialogue("WED_rlR_2_1A");
    }


}























[System.Serializable]
public struct 스크립트
{
    public string 현재스크립트;
    public bool 스킵가능;
    public 단위[] 단위;
}


[System.Serializable]
public struct 단위
{
    public bool         대사연결;
    public string       이름;
    public 색깔         글자색;

    
    public string       대사;
    public string       명령어;
}

[SerializeField]
public enum 색깔 { 검정, 빨강, 초록 };



[SerializeField]
public enum 패널 { 옵션, 핸드폰};










/*
[ContextMenu("반짝이는 것 테스트")]
void TestFlickingDialogueTail()
{
    StartCoroutine(C_FlickDialogueTail());
}
*/