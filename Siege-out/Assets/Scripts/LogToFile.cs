using UnityEngine;
using System.IO;

public class LogToFile : MonoBehaviour
{
    private string logFilePath = "log.txt"; // Path to the log file
    private StreamWriter logStreamWriter;

    void Start()
    {
        // Open the log file for writing
        logStreamWriter = File.CreateText(logFilePath);

        // Subscribe to log events
        Application.logMessageReceived += LogMessageReceived;
    }

    void OnApplicationQuit()
    {
        // Unsubscribe from log events
        Application.logMessageReceived -= LogMessageReceived;

        // Close the log file
        logStreamWriter.Close();
    }

    void LogMessageReceived(string logString, string stackTrace, LogType logType)
    {
        // Write the log message to the file
        logStreamWriter.WriteLine(logString);
        logStreamWriter.WriteLine(stackTrace);
    }
}