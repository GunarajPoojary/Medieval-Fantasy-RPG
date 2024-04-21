using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GunarajCode
{
    public class ExceptionLogger : MonoBehaviour
    {
        StreamWriter StreamWriter;
        [SerializeField] string LogFileName = "log.txt";


        // Start is called before the first frame update
        void Start()
        {
            StreamWriter = new StreamWriter(Application.persistentDataPath + "/" + LogFileName);

            Debug.Log(Application.persistentDataPath + "/" + LogFileName);
        }

        [System.Obsolete]
        void OnEnable() => Application.RegisterLogCallback(HandleLog);

        [System.Obsolete]
        void OnDisable() => Application.RegisterLogCallback(null);

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                StreamWriter.WriteLine("Logged at: " + System.DateTime.Now.ToString() + " - Log Desc: " + logString + " - Trace: " + stackTrace + " - Type: " + type.ToString());
            }
        }

        void OnDestroy() => StreamWriter.Close();
    }
}
