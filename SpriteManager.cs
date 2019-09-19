using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    private static SpriteManager instance;
    public static SpriteManager GetInstance()
    {
        if(instance ==null)
        {
            instance = FindObjectOfType<SpriteManager>();
            if(instance==null)
            {
                GameObject container = new GameObject("SpriteManager");
                instance =container.AddComponent<SpriteManager>();
            }
        }
        return instance;
    }


    //UI
    public Image backGroundImage;
    public Image characterPrefab;    



    //캐릭터 스프라이트
    public Sprite[] body;
    public Sprite[] jisooExpression;
    public Sprite[] hannahExpression;
    public Sprite[] jieunExpression;





    //코루틴 순서용 변수
    bool isHidingBackGround = false;
    bool isDrawingBackGround = false;
    [HideInInspector]
    public bool isDrawingCharacter = false;
    bool isChangingExpression = false;
    bool isHidingCharacter = false;

    public bool isRunningDrawCoroutine()
    {
        if(isHidingBackGround || isDrawingBackGround || isDrawingCharacter || isHidingCharacter)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    



    #region 캐릭터 생성 삭제 표정변화 관련

    public void DrawCharacter(string _charName, int _expression, float _targetTime=0.7f)
    {
        Image character = Instantiate(characterPrefab, GameObject.Find("Character").transform);
        character.name = _charName;

        if (_charName.Contains("지유"))
        {
            character.sprite = GetBodySprite("지유");
            GameObject.Find(_charName).transform.GetChild(0).GetComponent<Image>().sprite =
                GetExpressionSprite("지유", _expression);
        }
        else if (_charName.Contains("한나"))
        {
            character.sprite = GetBodySprite("한나");
            GameObject.Find(_charName).transform.GetChild(0).GetComponent<Image>().sprite =
                GetExpressionSprite("한나", _expression);
        }
        else if (_charName.Contains("루리"))
        {
            character.sprite = GetBodySprite("루리");
            GameObject.Find(_charName).transform.GetChild(0).GetComponent<Image>().sprite =
                GetExpressionSprite("루리", _expression);
        }
        else
        {
            Debug.Log("스프라이트 로드 에러! 스크립트를 확인해주세요.");
            return;
        }

        StartCoroutine(DrawCharacterCoroutine(_charName, _targetTime));
    }
    

    IEnumerator DrawCharacterCoroutine(string _gameObject, float _targetTime)
    {
        isDrawingCharacter = true;
        //배경을 그리는 동안 캐릭터가 그려지지 않도록
        while(isDrawingBackGround)
        {
            yield return null;
        }
        
        Image body = GameObject.Find(_gameObject).GetComponent<Image>();
        Image expression = GameObject.Find(_gameObject).transform.GetChild(0).GetComponent<Image>();
        if (body == null)
        {
            Debug.Log("오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        float nowTime = 0;
        float targetTime = _targetTime;
        float r = body.GetComponent<Image>().color.r;
        float g = body.GetComponent<Image>().color.g;
        float b = body.GetComponent<Image>().color.b;
        float a;
        yield return null;
        while (nowTime < _targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(0, 1, nowTime / targetTime);
            body.color = new Color(r, g, b, a);
            expression.color = new Color(r, g, b, a);
            yield return null;
        }
        isDrawingCharacter = false;
        yield return null;
    }

    public void HideCharacter(string _charName)
    {
        StartCoroutine("HideCharacterCoroutine",_charName);
    }

    IEnumerator HideCharacterCoroutine(string _charName)
    {
        isHidingCharacter = true;
        
        while(isChangingExpression||isDrawingCharacter)
        {
            yield return null;
        }

        Image characterImage = GameObject.Find(_charName).GetComponent<Image>();
        Image expressionImage = GameObject.Find(_charName).transform.GetChild(0).GetComponent<Image>();
        if(characterImage==null)
        {
            Debug.Log("캐릭터가 생성되지 않았거나, 잘못된 이름을 입력했습니다");
            yield break;
        }
        float nowTime = 0;
        float targetTime = 0.5f;
        float a;

        while(nowTime<targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(1, 0, nowTime / targetTime);
            characterImage.color = new Color(1, 1, 1, a);
            expressionImage.color = new Color(1, 1, 1, a);
            yield return null;
        }
        yield return null;
        isHidingCharacter = false;
        Destroy(GameObject.Find(_charName));
    }
    
    public void ChangeExpression(string _gameObject, int expressionIndex, float _targetTime = 0.7f)
    {
        StartCoroutine(ChangeExpressionCoroutine(_gameObject, expressionIndex, _targetTime));
    }
    IEnumerator ChangeExpressionCoroutine(string _gameObject, int expressionIndex, float _targetTime)
    {
        isChangingExpression = true;
        while(isDrawingCharacter)
        {
            yield return null;
        }
        float nowTime = 0;
        float targetTime = _targetTime;
        Image expression = GameObject.Find(_gameObject).transform.GetChild(0).GetComponent<Image>();
        Image newExpression = GameObject.Find(_gameObject).transform.GetChild(1).GetComponent<Image>();
        newExpression.sprite = GetExpressionSprite(_gameObject, expressionIndex);
        float a;
        float aa;

        while (nowTime < targetTime)
        {
            nowTime += Time.deltaTime;
            a = Mathf.Lerp(1, 0, nowTime / targetTime);
            aa = Mathf.Lerp(0.6f, 1, nowTime / targetTime);
            expression.color = new Color(1, 1, 1, a);
            newExpression.color = new Color(1, 1, 1, aa);
            yield return null;
        }
        expression.sprite = newExpression.sprite;
        expression.color = new Color(1, 1, 1, 1);
        newExpression.color = new Color(1, 1, 1, 0);
        yield return null;
        isChangingExpression = false;

    }



    Sprite GetBodySprite(string _charName)
    {
        if (_charName.Contains("지유"))
        {
            return SpriteManager.GetInstance().body[0];
        }
        else if (_charName.Contains("한나"))
        {
            return SpriteManager.GetInstance().body[1];
        }
        else if (_charName.Contains("루리"))
        {
            return SpriteManager.GetInstance().body[2];
        }
        else
        {
            Debug.Log("스프라이트 로드 에러! 스크립트를 확인해주세요.");
            return null;
        }
    }

    Sprite GetExpressionSprite(string _charName, int _expressionIndex)
    {
        if (_charName.Contains("지유"))
        {
            return SpriteManager.GetInstance().jisooExpression[_expressionIndex];
        }

        else if (_charName.Contains("한나"))
        {
            return SpriteManager.GetInstance().hannahExpression[_expressionIndex];
        }
        else if (_charName.Contains("루리"))
        {
            return SpriteManager.GetInstance().jieunExpression[_expressionIndex];
        }
        else
        {
            Debug.Log("스프라이트 로드 에러! 스크립트를 확인해주세요.");
            return null;
        }
    }


    #endregion

    #region 배경 생성 삭제

    public void DrawBackGround(string _path, float _targetTime = 0.75f)
    {
        StartCoroutine(DrawBackGroundCoroutine(_path, _targetTime));
    }

    IEnumerator DrawBackGroundCoroutine(string _path, float _targetTime)
    {
        while(isHidingBackGround==true)
        {
            yield return null;
        }

        isDrawingBackGround = true;

        DialogueManager.GetInstance().OpenDialogueBox(false);
        yield return new WaitForEndOfFrame();

        Sprite sprite = GetBackGroundSprite(_path);
        yield return null;
        backGroundImage.sprite = sprite;
        float alpha;
        float nowTime = 0;
        float targetTime = _targetTime;
        while(nowTime<targetTime)
        {
            nowTime += Time.deltaTime;
            alpha = Mathf.Lerp(0, 1, nowTime / targetTime);
            backGroundImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        isDrawingBackGround = false;
        yield return new WaitForSeconds(0.5f);
        DialogueManager.GetInstance().OpenDialogueBox(true);

    }

    public void HideBackGround(float _targetTime = 1.0f)
    {
        
        StartCoroutine(HideBackGroundCoroutine(_targetTime));
    }

    IEnumerator HideBackGroundCoroutine(float _targetTime)
    {
        isHidingBackGround = true;

        float nowTime = 0;
        float targetTime = _targetTime;
        float alpha;
        DialogueManager.GetInstance().OpenDialogueBox(false);
        yield return null;

        while(nowTime<targetTime)
        {
            nowTime += Time.deltaTime;
            alpha = Mathf.Lerp(1, 0, nowTime / targetTime);
            backGroundImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        yield return null;
        isHidingBackGround = false;
    }

    Sprite GetBackGroundSprite(string _path)
    {
        Sprite sprite;
        sprite = Resources.Load<Sprite>("BackGround/" + _path);
        if(sprite!=null)
        {
            return sprite;
        }
        else
        {
            Debug.Log("배경 스프라이트가 로드되지 않았습니다");
            return null;
        }
    }

    #endregion
}
