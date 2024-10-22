using System.IO;
using UnityEngine;

public static class CSVManager
{
    private static string directoryPath = Application.dataPath + "/CSVFiles";
    private static string filePath = directoryPath + "/TensionData7.csv";

    // Append to the CSV file
    public static void AppendToCSV(string content)
    {
        if (!File.Exists(filePath))
        {
            // Create the file and write headers if the file doesn't exist
            File.WriteAllText(filePath, "Episode,Tactual,Tension,EnemyCount\n");
        }

        // Append content to the file
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine(content);
        }
    }
}