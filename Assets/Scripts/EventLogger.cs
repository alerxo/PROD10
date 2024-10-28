using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventLogger : MonoBehaviour
{
    // Sökväg till loggfilen
    private string logFilePath;

    void Start()
    {
        // Hämtar filsökvägen för applikationen
        string directoryPath = Path.GetDirectoryName(Application.dataPath);
        directoryPath = Path.Combine(directoryPath, "EventLog");

        // Skapar katalogen om den inte finns
        Directory.CreateDirectory(directoryPath);

        // Ställer in loggfilens sökväg
        logFilePath = Path.Combine(directoryPath, "eventlog.txt");

        // Kontrollera om filen existerar
        if (!File.Exists(logFilePath))
        {
            // Skapa en ny loggfil med en rubrik om den inte finns
            File.WriteAllText(logFilePath, "Event log\n");
        }
    }

    // Metod för att logga event
    public void LogEvent(string eventDescription)
    {
        // Skapar ett event med datum och tid
        string logEntry = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + eventDescription + "\n";
        // Lägg till eventet i logfilen
        File.AppendAllText(logFilePath, logEntry);
    }
}
