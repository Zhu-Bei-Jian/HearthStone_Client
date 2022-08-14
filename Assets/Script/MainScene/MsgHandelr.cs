using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Google.Protobuf;
using System;
using System.Threading;

public class MsgHandelr : MonoBehaviour
{
    public VideoPlayer vp;
    //public AudioSource audioSrouce;
    public GameObject Btn;
    public GameObject BtnSelectHero;

    public static Dictionary<string, bool> MsgQueue;

    public static List<IMessage> syncList, respList, noticeList;

    public static int offset;

    public static int mySeatId;

    public GameObject p0HP, p0WeaponAttack, p0WeaponDurablity, p0Armor, p0Crystal, p0CrystalLimit, p0Attack, p0SubHp;

    public GameObject p1HP, p1WeaponAttack, p1WeaponDurablity, p1Armor, p1Crystal, p1CrystalLimit, p1Attack, p1SubHp;

    public static List<int> cardPool1, cardPool2;
    public static List<int> hand1, hand2;

    public static Dictionary<int, GameObject> id2Card;

    public static Action action;
    public static string myName;
    public static string toName;
    public static string faqiren;


    

    // Start is called before the first frame update
    void Awake()
    {
        cardPool1 = new List<int>();
        cardPool2 = new List<int>();
        hand1 = new List<int>();
        hand2 = new List<int>();
        id2Card = new Dictionary<int, GameObject>();
        MsgQueue = new Dictionary<string, bool>();
        syncList = new List<IMessage>();
        respList = new List<IMessage>();
        noticeList = new List<IMessage>();

    }

    private void Start()
    {
        id2Card.Add(0, GameObject.FindGameObjectWithTag("p0"));
        id2Card.Add(1, GameObject.FindGameObjectWithTag("p1"));
        
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    Sprite sp = Resources.Load<Sprite>("海盗战/南海船工");
        //    Debug.Log(sp);
        //    GameObject.FindGameObjectWithTag("Canvas").transform.Find("Login").GetComponent<Image>().sprite = sp;

        //}

        if (syncList.Count > 0)
        {
            Type tp = syncList[0].GetType();
            Debug.Log(tp);

            if (tp == typeof(SSyncCamp))
            {

                SSyncCamp msg = (SSyncCamp)syncList[0];
                msg.SeatId = (msg.SeatId + mySeatId) % 2;
                SyncCamp(msg);

            }
            if (tp == typeof(SSyncCard))
            {
                SSyncCard msg = (SSyncCard)syncList[0];
                msg.SeatId = (msg.SeatId + mySeatId) % 2;
               
                SyncCard(msg);
            }
            if (tp == typeof(SSyncAct))
            {
                SSyncAct msg = (SSyncAct)syncList[0];
                SyncAction(msg);
            }
            
                syncList.RemoveAt(0);
        }

        if (noticeList.Count > 0)
        {
            Type tp = noticeList[0].GetType();
            try
            {
                if (tp == typeof(SNoticeDrawCard))
                {

                  
                    SNoticeDrawCard msg = (SNoticeDrawCard)noticeList[0];
                    msg.OpSeatId ^= mySeatId;
                     
                
                    DrawCard(msg);
                  


                }
                if (tp == typeof(SNoticeEnterPhase))
                {
                    Debug.Log("收到回合开始信息");
                    SNoticeEnterPhase msg = (SNoticeEnterPhase)noticeList[0];
                    if (msg.Phase ==  GamePhase.PhaseBegin)
                    {
                        StartMyTurn();
                    }

                }
                if (tp == typeof(SNoticeGameOver))
                {
                    Debug.Log("游戏结束");
                    SNoticeGameOver msg = (SNoticeGameOver)noticeList[0];
                    if (mySeatId == msg.WinSeat)
                    {
                        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("win").gameObject.SetActive(true);
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("lose").gameObject.SetActive(true);
                    }
                     
                }
                if (tp == typeof(SNoticeReqFriend))
                {
                    SNoticeReqFriend msg = (SNoticeReqFriend)noticeList[0];
                    GameObject.FindGameObjectWithTag("Canvas").transform.Find("BWindow").gameObject.SetActive(true);
                    GameObject.FindGameObjectWithTag("Canvas").transform.Find("BWindow").Find("Text").GetComponent<Text>().text = msg.Nickname + "向你发起好友申请";
                    faqiren = msg.Nickname;
                }
                if (tp == typeof(SNoticeAccept))
                {
                    SNoticeAccept msg = (SNoticeAccept)noticeList[0];
                    GameObject.FindGameObjectWithTag("Canvas").transform.Find("Wjieshou").gameObject.SetActive(true);
                    GameObject.FindGameObjectWithTag("Canvas").transform.Find("Wjieshou").Find("Text").GetComponent<Text>().text = "对方已同意";
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

            }

            noticeList.RemoveAt(0);
        }

        if (respList.Count > 0)
        {
            Type tp = respList[0].GetType();
            if (tp == typeof(ChatMsg))
            {
                ChatMsg msg = (ChatMsg)respList[0];
                GameObject.FindGameObjectWithTag("Canvas").transform.Find("menuBackground").Find("menu").Find("msgBox").gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("To").GetComponent<Text>().text = MsgHandelr.toName;
                GameObject.FindGameObjectWithTag("msgShow").GetComponent<Text>().text += "\n" + msg.From+"   "+ DateTime.Now.ToLocalTime().ToString() + " \n"+msg.Content;
            }
            if (tp == typeof(SRespGameScene))
            {
                 
                SRespGameScene msg = (SRespGameScene)respList[0];
                mySeatId = msg.MySeatId;
                GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
                if (mySeatId == 0)
                {
                    Debug.Log("先手行动");
                    canvas.transform.Find("Game").Find("GameImg").Find("myTurn").gameObject.SetActive(true);
                    StartCoroutine(SetMyTurn());
                    canvas.transform.Find("Game").Find("GameImg").Find("jieshuhuihe").gameObject.SetActive(true);
                    canvas.transform.Find("Game").Find("GameImg").Find("duishouhuihe").gameObject.SetActive(false);

                }
                else
                {
                    Debug.Log("后手行动");
                    canvas.transform.Find("Game").Find("GameImg").Find("jieshuhuihe").gameObject.SetActive(false);
                    canvas.transform.Find("Game").Find("GameImg").Find("duishouhuihe").gameObject.SetActive(true);
                }
                GameObject.FindGameObjectWithTag("p0").GetComponent<Image>().sprite = Resources.Load<Sprite>("luShiSuCai/"+VS.SShero);
                GameObject.FindGameObjectWithTag("p1").GetComponent<Image>().sprite = Resources.Load<Sprite>("luShiSuCai/" + VS.eHero);

            }
            if (tp == typeof(SRespMatch))
            {
                SRespMatch msg = (SRespMatch)respList[0];
                GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
                canvas.transform.Find("SelectHero").Find("match").gameObject.SetActive(false);
                canvas.transform.Find("SelectHero").Find("Button").gameObject.SetActive(true);
                VS.eHero = msg.EHero;
                GameStart();
            }
            if (tp == typeof(SRespLogin))
            {

                SRespLogin msg = (SRespLogin)respList[0];
                
                if (msg.ErrCode == SRespLogin.Types.ErrCode.Success)
                {
                    Debug.Log("登入成功");
                    GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
                    canvas.transform.Find("Login").localPosition = new Vector3(1000, 0, 0);
                    //canvas.transform.Find("SelectHero").localPosition = new Vector3(0, 0, 0);
                    canvas.transform.Find("menuBackground").gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("密码错误");
                }
            }

            if (tp == typeof(SRespUserInfo))
            {
                Debug.Log("更新好友列表");
                SRespUserInfo msg = (SRespUserInfo)respList[0];
                GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
                canvas.transform.Find("menuBackground").Find("menu").Find("myInfo").Find("name").GetComponent<Text>().text = msg.Userinfo.NickName;
                myName = msg.Userinfo.NickName;
                GameObject info = GameObject.FindGameObjectWithTag("info");
                for(int i = info.transform.parent.childCount-1; i > 0; i--)
                {
                    Destroy(info.transform.parent.GetChild(i));
                }
                for (int i = 0; i < msg.Userinfo.Friends.Count; i++)
                {
                    GameObject now =Instantiate(info, info.transform.parent);
                    //now.transform.Find("icon").GetComponent<Image>().sprite=
                    now.transform.Find("nick").GetComponent<Text>().text = msg.Userinfo.Friends[i].Name;
                    if (msg.Userinfo.Friends[i].Online)
                    {
                        now.transform.Find("online").GetComponent<Text>().text = "在线";
                    }
                    else
                    {
                        now.transform.Find("online").GetComponent<Text>().text = "不在线";
                    }
                    
                }
             
               
            }

            respList.RemoveAt(0);
        }

    }

    public void back2(string back)
    {
        for (int i = GameObject.FindGameObjectWithTag("Canvas").transform.childCount - 1; i >= 0; i--)
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject.FindGameObjectWithTag("Canvas").transform.Find(back).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Canvas").transform.Find(back).localPosition = Vector3.zero;

    }

    public void guanbishipin()
    {
        Debug.Log("关掉音频");
        vp.Stop();
        GameObject.FindGameObjectWithTag("shipin").SetActive(false);
    }
    private void EndMyTurn()
    {
        CReqAct req = new CReqAct();
        req.ActionType =  ActionType.EndTurn;
        Send(req);
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("Game").Find("GameImg").Find("jieshuhuihe").gameObject.SetActive(false);
        canvas.transform.Find("Game").Find("GameImg").Find("duishouhuihe").gameObject.SetActive(true);
    }

    void StartMyTurn()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("Game").Find("GameImg").Find("myTurn").gameObject.SetActive(true);
        StartCoroutine(SetMyTurn());
        canvas.transform.Find("Game").Find("GameImg").Find("jieshuhuihe").gameObject.SetActive(true);
        canvas.transform.Find("Game").Find("GameImg").Find("duishouhuihe").gameObject.SetActive(false);
    }

    public void ClickContinueButton()
    {
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("win").gameObject.SetActive(false);

        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("lose").gameObject.SetActive(false);

        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").gameObject.SetActive(false);
    }
    IEnumerator SetMyTurn()
    {
        yield return new WaitForSeconds(1);
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("Game").Find("GameImg").Find("myTurn").gameObject.SetActive(false);
    }
    public void SyncCard(SSyncCard s)
    {
        Debug.LogFormat("s.Changes.Count={0}", s.Changes.Count);
        for (int i = 0; i < s.Changes.Count; i++)
        {
            Debug.LogFormat("同步卡牌 类型={0}", s.Changes[i].ChangeType);
            if (s.Changes[i].ChangeType == SSyncCard.Types.ChangeType.Attack)
            {
                id2Card[s.CardId].transform.Find("mask").Find("attack").Find("Text").GetComponent<Text>().text = s.Changes[i].New.ToString();
            }
            else if (s.Changes[i].ChangeType == SSyncCard.Types.ChangeType.Hp)
            {
                id2Card[s.CardId].transform.Find("mask").Find("hp").Find("Text").GetComponent<Text>().text = s.Changes[i].New.ToString();
                id2Card[s.CardId].transform.Find("mask").Find("subHp").Find("Text").GetComponent<Text>().text = s.Changes[i].Change_.ToString();
                if (s.Changes[i].Change_ < 0)
                {
                    s.Changes[i].Change_ = -s.Changes[i].Change_;
                }
                id2Card[s.CardId].transform.Find("mask").Find("subHp").Find("Text").GetComponent<Text>().text ="-"+ s.Changes[i].Change_.ToString();
                
                id2Card[s.CardId].transform.Find("mask").Find("subHp").gameObject.SetActive(true);
                id2Card[s.CardId].transform.Find("mask").Find("subHp").GetComponent<Image>().enabled = false;
                StartCoroutine(SetActive(id2Card[s.CardId].transform.Find("mask").Find("subHp").gameObject, false));
                id2Card[s.CardId].transform.Find("mask").Find("subHp").GetChild(0).GetComponent<Text>().color = Color.green;
            }
            else if (s.Changes[i].ChangeType == SSyncCard.Types.ChangeType.Place)
            {
                Debug.Log("对手放置卡牌");
                id2Card[s.CardId].transform.GetComponent<cardClick>().PlaceCard(1);
            }
            else if(s.Changes[i].ChangeType == SSyncCard.Types.ChangeType.Die)
            {
                Debug.Log("卡牌死亡");
                StartCoroutine(SetDead(s.CardId));
            }
            else if (s.Changes[i].ChangeType == SSyncCard.Types.ChangeType.Summon)
            {
                GameObject emptyCard = GameObject.FindGameObjectWithTag("tagDraw");
                GameObject cardPanel = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg").Find("p1minion").gameObject;

                if (s.SeatId == 0)
                {
                    Debug.Log("我方召唤帕奇斯");
                    cardPanel = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg").Find("p0minion").gameObject;
                }
                else if (s.SeatId == 1)
                {
                    Debug.Log("对方召唤帕奇斯");
                    cardPanel = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg").Find("p1minion").gameObject;

                }

                GameObject newCard = Instantiate(emptyCard, GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg"), false);
                newCard.SetActive(true);
                newCard.transform.localPosition = new Vector3(300f, -36f, -12f);


                shuxing kapai = newCard.transform.GetComponent<shuxing>();
                kapai.cd = new shuxing.Card();
                kapai.cd.hp = s.Card.Hp;
                kapai.cd.attack = s.Card.Attack;
                kapai.cd.id = s.Card.Id;
                kapai.cd.name = s.Card.Name;
                kapai.cd.rawName = kapai.cd.name;

 

                kapai.SyncUI();

                Sequence sequence = DOTween.Sequence();
                float x = -68f + (cardPanel.transform.childCount - 4) * 60f;
                Vector3 movePos = new Vector3(108f, -27f, -12f);
                newCard.GetComponent<cardClick>().pos = movePos;
                newCard.transform.rotation = Quaternion.Euler(0, 0, 0);
                Tweener a1 = newCard.transform.DOLocalMove(movePos, 1f);
                //Tweener a2 = newCard.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
                //sequence.AppendInterval(0.5f);
                sequence.Append(a1);
                MsgHandelr.id2Card.Add(kapai.cd.id, newCard);
                
                StartCoroutine(ISummon(newCard,cardPanel));
            }
            else if (s.Changes[i].ChangeType == SSyncCard.Types.ChangeType.GainMinion)
            {

            }

        }

    }
    IEnumerator ISummon(GameObject newCard,GameObject cardPanel)
    {
        yield return new WaitForSeconds(1f);
        newCard.transform.SetParent(cardPanel.transform);
        newCard.transform.rotation = Quaternion.Euler(0, 0, 0);
        newCard.transform.Find("mask").GetComponent<Mask>().enabled = true;

        newCard.transform.Find("mask").GetChild(1).gameObject.SetActive(true);
        //transform.Find("mask").GetComponent<Image>().enabled = true;
        newCard.transform.Find("mask").Find("attack").gameObject.SetActive(true);
        newCard.transform.Find("mask").Find("hp").gameObject.SetActive(true);
        newCard.GetComponent<cardClick>().placed = true;
        newCard.GetComponent<shuxing>().SyncAttackHP();
        newCard.GetComponent<shuxing>().ResetName();
    }
    IEnumerator SetDead(int id)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(id2Card[id]);
    }
    public void SyncCamp(SSyncCamp msg)
    {

        Debug.LogFormat("同步camp ，seatId={0}, myseatId={1}", msg.SeatId, mySeatId);
        if (msg.SeatId == 0)
        {
            for (int i = 0; i < msg.Changes.Count; i++)
            {
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.WeaponAttack)
                {
                    p0WeaponAttack.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.WeaponDurability)
                {
                    p0WeaponDurablity.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Hp)
                {
                    p0HP.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                    p0SubHp.GetComponent<Text>().text = msg.Changes[i].Change_.ToString();
                    p0SubHp.transform.parent.gameObject.SetActive(true);
                    StartCoroutine(SetActive(p0SubHp.transform.parent.gameObject, false));

                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Attack)
                {
                    p0Attack.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Crystal)
                {
                    p0Crystal.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.CrystalLimit)
                {
                    p0CrystalLimit.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Armor)
                {
                    p0Armor.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }

            }
        }


        if (msg.SeatId == 1)
        {
            for (int i = 0; i < msg.Changes.Count; i++)
            {
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.WeaponAttack)
                {
                    p1WeaponAttack.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.WeaponDurability)
                {
                    p1WeaponDurablity.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Hp)
                {
                    p1HP.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                    p1SubHp.GetComponent<Text>().text = msg.Changes[i].Change_.ToString();
                    p1SubHp.transform.parent.gameObject.SetActive(true);
                    StartCoroutine(SetActive(p1SubHp.transform.parent.gameObject, false));
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Attack)
                {
                    p1Attack.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Crystal)
                {
                    p1Crystal.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.CrystalLimit)
                {
                    p1CrystalLimit.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }
                if (msg.Changes[i].ChangeType == SSyncCamp.Types.ChangeType.Armor)
                {
                    p1Armor.GetComponent<Text>().text = msg.Changes[i].New.ToString();
                }

            }
        }


    }
    
    public void SyncAction(SSyncAct msg)
    {
        Debug.Log("同步行动");
        Debug.Log(msg.ActType);
        if (msg.SrcId < 2)
        {
            msg.SrcId ^= mySeatId;
        }

        if (msg.TargetId < 2)
        {
            msg.TargetId ^= mySeatId;
        }
        GameObject src, tar;
        src = id2Card[msg.SrcId];
        tar = id2Card[msg.TargetId];
        switch (msg.ActType)
        {
            case ActionType.Attack:             
                action.DoAttack(src, tar, false);
                break;
            case ActionType.Damage:
                StartCoroutine(DoDamage(src, tar,-msg.Value));
                break;
            default:
                break;

        }

    }
    IEnumerator DoDamage(GameObject src,GameObject tar,int value)
    {
        yield return new WaitForSeconds(1f);  
        action.DoDamage(src, tar,value);
   
    }
    IEnumerator SetActive(GameObject go, bool bl)
    {
        yield return new WaitForSeconds(1);
        go.SetActive(bl);
        go.GetComponent<Image>().enabled = true;
        go.transform.GetChild(0).GetComponent<Text>().color= Color.white;
    }

    public void Stop()
    {
        vp.Stop();
        Debug.Log("Stop()");
        //audioSrouce.Stop();
        Btn.SetActive(false);
    }

    public void GameStart()
    {
        //Debug.Log("---游戏开始---");
        //BtnSelectHero.transform.DOMoveX(0, 1.3f);
        //CReqGameScene req = new CReqGameScene();
        //Send(req);
        back2("Game");
        //GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        //canvas.transform.Find("SelectHero").gameObject.SetActive(false);
        //canvas.transform.Find("Game").DOLocalMove(new Vector3(0, 0, 0), 1f);
    }

    public void ReqMatch()
    {

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("SelectHero").Find("match").gameObject.SetActive(true);
        canvas.transform.Find("SelectHero").Find("Button").gameObject.SetActive(false);
        CReqMatch req = new CReqMatch();
        req.Uid = 1;
        req.MyHero = VS.SShero;
        Send(req);


    }
    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("SelectHero").Find("match").gameObject.SetActive(false);
        canvas.transform.Find("SelectHero").Find("Button").gameObject.SetActive(true);
        GameStart();
    }

    public void CancelMatch()
    {
        Send(new CCancelMatch());

        try
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            canvas.transform.Find("SelectHero").Find("match").gameObject.SetActive(false);
            canvas.transform.Find("SelectHero").Find("Button").gameObject.SetActive(true);

        }
        catch (Exception ex)
        {

            Debug.LogException(ex);
        }
    }

    
    public void BLogin()
    {
        CReqLogin cReqLogin = new CReqLogin();

        //SocketConnect.client.Send()

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        cReqLogin.Account = canvas.transform.Find("Login").transform.Find("Input_account").transform.Find("Text").GetComponent<Text>().text;
        cReqLogin.Password = canvas.transform.Find("Login").transform.Find("Input_password").transform.GetComponent<InputField>().text;

        Send(cReqLogin);

    }

    public void EnterRegister()
    {
        //CReqRegister cReqRegister = new CReqRegister();
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("Login").gameObject.SetActive(false);
        canvas.transform.Find("Register").gameObject.SetActive(true);
        canvas.transform.Find("Register").localPosition=new Vector3(0,0,0);
    }

    public void BExit()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("menuBackground").gameObject.SetActive(true);
        //canvas.transform.Find("menuBackground").localPosition = new Vector3(0, 0, 0);

    }

    public void BRegister()
    {
        CReqRegister cReqRegister = new CReqRegister();
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        cReqRegister.Account= canvas.transform.Find("Register").transform.Find("Input_account").transform.Find("Text").GetComponent<Text>().text;
        cReqRegister.Password = canvas.transform.Find("Register").transform.Find("Input_password").transform.GetComponent<InputField>().text;
        cReqRegister.NickName= canvas.transform.Find("Register").transform.Find("nickname").transform.Find("Text").GetComponent<Text>().text;
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Wjieshou").gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Canvas").transform.Find("Wjieshou").Find("Text").GetComponent<Text>().text = "注册成功";
        Send(cReqRegister);
    }

    public void BSelectHero()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        canvas.transform.Find("Game").localPosition = new Vector3(0, 0, 0);
        canvas.transform.Find("SelectHero").localPosition = new Vector3(-1000, 0, 0);
    }
    public void DrawCard(SNoticeDrawCard msg)
    {

        //emptyCard.AddComponent<Image>();
        GameObject emptyCard = GameObject.FindGameObjectWithTag("tagDraw");
        GameObject cardPanel = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg").Find("cardPanel1").gameObject;

        if (msg.OpSeatId == 0)
        {
            Debug.Log("我方抽卡");
            cardPanel = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg").Find("cardPanel0").gameObject;
        }
        else if (msg.OpSeatId == 1)
        {
            Debug.Log("对方抽卡");
            cardPanel = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Game").Find("GameImg").Find("cardPanel1").gameObject;

        }

        GameObject newCard = Instantiate(emptyCard, cardPanel.transform, false);
        newCard.SetActive(true);
        newCard.transform.localPosition = new Vector3(363f, 161f, 10f);


        shuxing kapai = newCard.transform.GetComponent<shuxing>();
        kapai.cd = new shuxing.Card();
        kapai.cd.hp = msg.Hp;
        kapai.cd.attack = msg.Attack;
        kapai.cd.id = msg.Id;
        kapai.cd.name = msg.Name;
        kapai.cd.rawName = kapai.cd.name;
        
        if (msg.OpSeatId == 1)
        {
            kapai.cd.name = "黄金卡背";
        }

        kapai.SyncUI();

        Sequence sequence = DOTween.Sequence();
        float x = -68f + (cardPanel.transform.childCount - 4) * 60f;
        Vector3 movePos = new Vector3(x, 20f, 0f);
        newCard.GetComponent<cardClick>().pos = movePos;

        Tweener a1 = newCard.transform.DOLocalMove(movePos, 1f);
        Tweener a2 = newCard.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f);
        sequence.Append(a2);
        sequence.AppendInterval(0.5f);
        sequence.Append(a1);
        MsgHandelr.id2Card.Add(kapai.cd.id, newCard);

    }
    public static void Send(IMessage msg)
    {
        byte[] data = msg.ToByteArray();

        string fullName = msg.Descriptor.FullName;

        byte[] data2 = new byte[data.Length + 2 + fullName.Length + 1];
        if (fullName.Length < 10)
        {
            data2[0] = (byte)'0';
            data2[1] = (byte)('0' + fullName.Length);

        }
        else
        {
            data2[0] = (byte)('0' + fullName.Length / 10);
            data2[1] = (byte)('0' + fullName.Length % 10);
        }
        for (int i = 0; i < fullName.Length; i++)
        {
            data2[i + 2] = (byte)fullName[i];
        }
        for (int i = 0; i < data.Length; i++)
        {
            data2[i + 2 + fullName.Length] = data[i];
        }
        data2[data.Length + 2 + fullName.Length] = (byte)'#';
        try
        {
            Debug.LogFormat("发送数据 msg = {0},{1}", msg.ToString(), SocketConnect.client.LocalEndPoint.ToString());
            SocketConnect.client.Send(data2);
        }
        catch (Exception)
        {

             
        }
        
    }


}
