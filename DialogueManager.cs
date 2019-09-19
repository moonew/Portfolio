using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //싱글톤
    private static DialogueManager instance;
    public static DialogueManager GetInstance()
    {
        if(instance==null)
        {
            instance = FindObjectOfType<DialogueManager>();
            if(instance ==null)
            {
                GameObject container = new GameObject("DialogueManager");
                instance = container.AddComponent<DialogueManager>();
            }
        }
        return instance;
    }


    //UI
    public  GameObject     dialogueBox;
    public  Text           dialogueText;
    public  Text           nameText;


    //대화창 옵션
    float typingSpeed = 0.005f;
    bool isClickMode = true;




    //대사를 나눠서 출력할 것인가?
    bool isConnectingSentence = false;


    //코루틴
    bool isRunningTalkCoroutine = false;
    
   

    //대화스크립트를 담을 리스트
    List<string> sentence = new List<string>();
    int sentenceIndex = 1;







    void SettingDialogueOption()
    {
        typingSpeed = PlayerPrefs.GetFloat("customTypingSpeed",0.005f);

        if (PlayerPrefs.GetInt("isClickMode", 0)==0)
        {
            isClickMode = true;
        }
        else
        {
            isClickMode = false;
        }
    }




    public void GetDialogue(string _path)
    {
        sentence.Clear();
        TextAsset data = Resources.Load<TextAsset>("Script/"+_path);

        if (data==null)
        {
            Debug.Log("경로에 파일이 없습니다.");
            return;
        }
        else
        {
            Debug.Log(_path + "파일이 정상적으로 로드됨");
        }

        string dialogueData = data.ToString();

        
        string[] container = dialogueData.Split('\n');
        foreach(string s in container)
        {
            sentence.Add(s);
        }
        
    }



    #region 스크립팅 관련
    public void Talk(float _targetTime = 0.005f)
    {
        StartCoroutine("TalkCoroutine", _targetTime);
    }

    public void Talk()
    {
        typingSpeed = PlayerPrefs.GetFloat("customTypingSpeed", 0.005f);
        StartCoroutine("TalkCoroutine", typingSpeed);
    }
    IEnumerator TalkCoroutine(float _targetTime)
    {

        //코루틴 처리되는 동안 중복 클릭을 막기 위한 변수
        isRunningTalkCoroutine = true;


      

        if (sentenceIndex == sentence.Count - 1)
        {
            Debug.Log(sentence[sentenceIndex]);
            Debug.Log("대화 끝남");
            yield break;
        }

        string[] element = sentence[sentenceIndex].Split(',');

        Debug.Log("이름 : " + element[1]);
        Debug.Log("대사 : " + element[2]);

        //눌 체크 

        //명령어가 입력되어 있을 때.
        if(element.Length>3)
        {
            string[] command = element[3].Split(' ');
            foreach(string s in command)
            {
                ScriptCommand(s);
            }
        }




        //이름이 입력되어있으면
        if (element[1] != "")
        {
            //그대로 입력된 이름이 출력된다.
           nameText.text = element[1];
        }
        //이름이 입력되어있지 않으면
        else
        {
            //이름과 대사가 모두 입력되어 있지 않을때는
            if(element[2] == "")
            {
                //이름창을 비운다. 
                nameText.text = "";
            }
            //이름은 없지만 대사가 입력되어있는 경우에는 바로 전 화자의 이름이 출력된다.
        }

        //문자연결모드가 on이면 대화창을 비우지 않고 전 문장에 바로 이어서 작성한다.
        if (isConnectingSentence == false)
        {
            dialogueText.text = "";
        }
        else
        {
            dialogueText.text += " ";
        }

        //타이핑 코루틴이 출력된다.
        yield return StartCoroutine(Typing(element[2], _targetTime));

        ++sentenceIndex;

        //중복클릭방지 잠금을 해제하여 클릭이 가능하도록 한다.
        isRunningTalkCoroutine = false; 
        yield return null;
        
    }

    IEnumerator Typing(string _sentence ,float _targetTime)
    {
        //캐릭터가 그려지는 동안에는 대화가 시작되지 않는다.
        while (SpriteManager.GetInstance().isDrawingCharacter == true)
        {
            yield return null;
        }

        string script = _sentence;
        foreach(char s in script)
        {
            dialogueText.text += s;
            yield return new WaitForSeconds(_targetTime);
        }
    }
    
    public void OpenDialogueBox(bool _toggle)
    {
        if(_toggle==true)
        {
            dialogueBox.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            dialogueBox.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    //명령어
    void ScriptCommand(string _command)
    {
        Debug.Log("scriptCommand 들어옴");
        string head = _command.Split('(')[0];


        switch(head)
        {
            case "스크립트":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    GetDialogue(parameter[0]);
                    StopCoroutine("TalkCoroutine");
                    sentenceIndex = 1;
                    isRunningTalkCoroutine = false;
                }break;

            case "대사연결":
                {
                    isConnectingSentence = true;
                }
                break;

            case "대사연결제거":
                {
                    isConnectingSentence = false;
                }
                break;





            case "배경출력":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    if (parameter.Length>1)
                    {
                        float changeTime;
                        float.TryParse(parameter[1] , out changeTime);
                        SpriteManager.GetInstance().DrawBackGround(parameter[0], changeTime);
                        return;
                    }
                    SpriteManager.GetInstance().DrawBackGround(parameter[0]);
                }
                break;

            case "배경제거":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    Debug.Log(parameter.Length);

                    if (parameter[0]!="")
                    {
                        float changeTime;
                        float.TryParse(parameter[0], out changeTime);
                        SpriteManager.GetInstance().HideBackGround(changeTime);
                        return;
                    }
                        
                    else
                    {
                        SpriteManager.GetInstance().HideBackGround();
                    }

                }
                break;


            case "캐릭터출력":
                {
                    
                    string[] parameter = MakeFuctionForm(_command);

                    int expression;
                    int.TryParse(parameter[1], out expression);

                    if(parameter.Length ==3)
                    {
                        float drawingSpeed;
                        float.TryParse(parameter[2], out drawingSpeed);
                        SpriteManager.GetInstance().DrawCharacter(parameter[0], expression, drawingSpeed);
                        return;
                    }
                    SpriteManager.GetInstance().DrawCharacter(parameter[0], expression);


                } break;

            case "캐릭터제거":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    SpriteManager.GetInstance().HideCharacter(parameter[0]);
                }
                break;

            case "표정":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    int expressionIndex;
                    int.TryParse(parameter[1], out expressionIndex);
                    if(parameter.Length>2)
                    {
                        float changeTime;
                        float.TryParse(parameter[2], out changeTime);
                        SpriteManager.GetInstance().ChangeExpression(parameter[0], expressionIndex,changeTime);
                        return;
                    }
                    SpriteManager.GetInstance().ChangeExpression(parameter[0], expressionIndex);
                }
                break;

            case "배경음":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    SoundManager.GetInstance().PlayBgm(parameter[0]);
                }
                break;

            case "배경음제거":
                {
                    SoundManager.GetInstance().StopBgm();
                }break;

            case "효과음":
                {
                    string[] parameter = MakeFuctionForm(_command);
                    SoundManager.GetInstance().PlayEffect(parameter[0]);
                }
                break;

            case "효과음제거":
                {
                    SoundManager.GetInstance().StopEffect();
                }
                break;


            default:
                {
                    Debug.Log("명령어를 잘못 입력했습니다.");
                }break;
        }
    }

    string[] MakeFuctionForm(string _command)
    {    
        string[] s = _command.Split(new char[] { '(',')' });
        string[] parameter = s[1].Split('/');

        //공백제거
        for(int i=0; i<parameter.Length; i++)
        {
            parameter[i].Trim();
            Debug.Log("parameter"+i+" : " + parameter[i]);
        }

        
        return parameter;

    }


               
    void SentenceInitializer()
    {
        sentence.Clear();
        sentenceIndex       = 1;
        nameText.text       = "";
        dialogueText.text   = "";
    }

    #endregion


  


    void Start()
    {
        GetDialogue("script1");

    }

    void Update()
    {
        //타이핑이 되고 있거나, 그려지고 있으면 클릭이 불가.
        if(Input.GetKeyDown(KeyCode.A) && isRunningTalkCoroutine==false && SpriteManager.GetInstance().isRunningDrawCoroutine()==false)
        {
            Talk();
        }
        

    }





}


