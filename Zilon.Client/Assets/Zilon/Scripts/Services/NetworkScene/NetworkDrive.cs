using UnityEngine;
using UnityEngine.Networking;

public class NetworkDrive : NetworkBehaviour
{
    float periodSvrRpc = 0.02f; //как часто сервер шлёт обновление картинки клиентам, с.
    float timeSvrRpcLast = 0; //когда последний раз сервер слал обновление картинки

    string srvCommand;
    string myCommand;


    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myCommand = "Моя команда";
                CmdDrive(myCommand);
                myCommand = null;
            }
        }

        if (isServer)
            //Код исполняется только у духа
        {
            //Обработать мои команды
            //this.transform.Translate(0, 0, veloSvrCurr * Time.deltaTime, Space.Self);
            if (timeSvrRpcLast + periodSvrRpc < Time.time)
                //Если пора, то выслать координаты всем моим аватарам
            {
                //RpcUpdateUnitPosition(this.transform.position);
                //RpcUpdateUnitOrientation(this.transform.rotation);
                if (srvCommand != null)
                {
                    RpcSendCommand(srvCommand + " Через Сеть");
                    srvCommand = null;
                }

                timeSvrRpcLast = Time.time;
            }
        }
    }

    [Command(channel = 0)]
    void CmdDrive(string data)
    {
        if (isServer)
            //Мой дух принимает и проверяет команду.
        {
            srvCommand = data;
            ////Проверяем моё требование на валидность.
            //veloSvrNew = Mathf.Clamp(veloSvrNew, -veloSvrMax, veloSvrMax);
            ////Устанавливаем текущее значение требуемой мною скорости для духа.
            //veloSvrCurr = veloSvrNew;
            ////Исполнять будет дух позже.
        }
    }

    [ClientRpc(channel = 0)]
    private void RpcSendCommand(string data)
    {
        if (isClient)
            //Мои аватары копируют состояние моего духа.
        {
            Debug.Log(data);
        }
    }
}