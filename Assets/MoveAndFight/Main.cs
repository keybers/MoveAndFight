using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
/// <summary>
/// 一直在场景存在
/// </summary>
public class Main : MonoBehaviour
{
    //人物模型预设
    public GameObject humanPrefab;

    //人物列表
    private BaseHuman ctrlHuman;
    private Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();

    void Start()
    {
        //NetManager.AddListener表示只能从内部调用，不能外部调用
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("Attack", OnAttack);
        NetManager.AddListener("Die", OnDie);
        NetManager.Connect("127.0.0.1", 8888);

        //添加一个角色
        GameObject gameObject = Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        gameObject.transform.position = new Vector3(x, 0, z);
        ctrlHuman = gameObject.AddComponent<CtrlHuman>();
        ctrlHuman.desc = NetManager.GetDesc();

        //发送协议   Enter|127.0.0.1:4564,3,0,5,0
        Vector3 pos = ctrlHuman.transform.position;
        Vector3 eul = ctrlHuman.transform.eulerAngles;

        string sendStr = "Enter|";

        sendStr += NetManager.GetDesc() + ",";

        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y + ",";

        //添加角色，发送Enter协议
        NetManager.Send(sendStr);

        //请求玩家列表
        NetManager.Send("List|");
    }

    private void OnAttack(string msg)
    {
        Debug.Log("OnAttack" + msg);

        //解析参数
        string[] spilt = msg.Split(',');
        string desc = spilt[0];
        float eulY = float.Parse(spilt[1]);

        if (!otherHumans.ContainsKey(desc)) return;

        SyncHuman h = (SyncHuman)otherHumans[desc];
        h.SyncAttack(eulY);

    }

    private void OnList(string msg)
    {
        Debug.Log("OnList" + msg);
        
        //解析参数
        string[] split = msg.Split(',');
        int count = (split.Length - 1) / 6;//6个参数加一个逗号
        for(int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);

            //是自己
            if (desc == NetManager.GetDesc()) continue;

            //添加一个角色
            GameObject gameObject = Instantiate(humanPrefab);
            gameObject.transform.position = new Vector3(x, y, z);
            gameObject.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = gameObject.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }

    private void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg);
        string[] split = msg.Split(',');
        string desc = split[0];

        if (!otherHumans.ContainsKey(desc)) return;
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);
        otherHumans.Remove(desc);
    }

    private void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg);

        //解析函数
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);

        //移动
        if (!otherHumans.ContainsKey(desc)) return;

        BaseHuman h = otherHumans[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        h.transform.eulerAngles = new Vector3(0, eulY, 0);

        h.MoveTo(targetPos);
    }

    private void OnEnter(string msg)
    {
        Debug.Log("OnEnter" + msg);

        //解析参数
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eul = float.Parse(split[4]);

        //是自己
        if(desc == NetManager.GetDesc())
        {
            return;
        }
        GameObject gameObject = Instantiate(humanPrefab);
        gameObject.transform.position = new Vector3(x, y, z);
        gameObject.transform.eulerAngles = new Vector3(0, eul, 0);
        BaseHuman h = gameObject.AddComponent<SyncHuman>();
        h.desc = desc;

        otherHumans.Add(desc, h);
    }

    private void OnDie(string msg)
    {
        Debug.Log("OnDie" + msg);

        //解析函数
        string[] split = msg.Split(',');
        string attDesc = split[0];
        string hitDesc = split[0];

        //自己死了
        if(hitDesc == ctrlHuman.desc)
        {
            Debug.Log("Game Over");
            return;
        }

        //死了
        if (!otherHumans.ContainsKey(hitDesc)) return;
        SyncHuman h = (SyncHuman)otherHumans[hitDesc];
        h.gameObject.SetActive(false);
    }
}
