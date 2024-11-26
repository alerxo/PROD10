using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventLogger : MonoBehaviour
{
    // S�kv�g till loggfilen
    private string logFilePath;

    void Start()
    {
        // H�mtar fils�kv�gen f�r applikationen
        string directoryPath = Path.GetDirectoryName(Application.dataPath);
        directoryPath = Path.Combine(directoryPath, "EventLog");

        // Skapar katalogen om den inte finns
        Directory.CreateDirectory(directoryPath);

        // St�ller in loggfilens s�kv�g
        logFilePath = Path.Combine(directoryPath, "eventlog.txt");

        // Kontrollera om filen existerar
        if (!File.Exists(logFilePath))
        {
            // Skapa en ny loggfil med en rubrik om den inte finns
            File.WriteAllText(logFilePath, "Event log\n");
        }
    }

    // Metod f�r att logga event
    public void LogEvent(string eventDescription)
    {
        // Skapar ett event med datum och tid
        string logEntry = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + eventDescription + "\n";
        // L�gg till eventet i logfilen
        File.AppendAllText(logFilePath, logEntry);
    }
}
