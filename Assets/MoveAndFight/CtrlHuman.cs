using UnityEngine;

public class CtrlHuman : BaseHuman
{
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            Physics.Raycast(ray, out raycastHit);
            if(raycastHit.collider.tag == "Terrain")
            {
                MoveTo(raycastHit.point);

                //发送协议
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc().ToString() + ",";
                sendStr += raycastHit.point.x + ",";
                sendStr += raycastHit.point.y + ",";
                sendStr += raycastHit.point.z + ",";
                sendStr += raycastHit.transform.eulerAngles.y - transform.eulerAngles.y + ",";

                NetManager.Send(sendStr);
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (isAttacking || isMoving) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            Physics.Raycast(ray, out raycastHit);

            transform.LookAt(raycastHit.point);
            Attack();

            //发送协议
            string sendStr = "Attack|";
            sendStr += NetManager.GetDesc() + ",";
            sendStr += transform.eulerAngles.y + ",";
            NetManager.Send(sendStr);

            Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
            Vector3 lineStart = lineEnd + 20 * transform.forward;
            if(Physics.Linecast(lineStart,lineEnd,out raycastHit))
            {
                GameObject hitgameObject = raycastHit.collider.gameObject;
                if(hitgameObject == gameObject)//如果击中攻击者
                {
                    return;
                }
                SyncHuman h = hitgameObject.GetComponent<SyncHuman>();
                if(h == null)//如果没获取到SyncHuman
                {
                    return;
                }
                sendStr = "Hit|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += h.desc + ",";
                NetManager.Send(sendStr);
            }

        }
    }

}
