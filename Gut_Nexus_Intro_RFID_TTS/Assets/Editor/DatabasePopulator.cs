using UnityEngine;
using UnityEditor;
using System.IO;

public class DatabasePopulator : Editor
{
    private const int RFID_EXPECTED_LENGTH = 10;

    [MenuItem("Event/Populate Guest Database from CSV")]
    public static void PopulateDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:GuestDatabase");
        if (guids.Length == 0)
        {
            Debug.LogError("Could not find a GuestDatabase asset. Please create one.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        GuestDatabase database = AssetDatabase.LoadAssetAtPath<GuestDatabase>(path);

        database.guestList.Clear();
        string csvPath = Path.Combine(Application.streamingAssetsPath, "guest_data.csv");

        if (!File.Exists(csvPath))
        {
            Debug.LogError("Could not find guest_data.csv at: " + csvPath);
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(',');

            if (values.Length < 3)
            {
                Debug.LogWarning("Skipping line (not enough data): " + line);
                continue;
            }

            GuestEntry entry = new GuestEntry();

            string rawRfid = values[0].Trim();

            entry.rfid_ID = rawRfid.PadLeft(RFID_EXPECTED_LENGTH, '0');

            entry.guestName = values[1].Trim();
            entry.audioFilename = values[2].Trim();

            database.guestList.Add(entry);

            if (rawRfid != entry.rfid_ID)
            {
                Debug.Log("Import: Corrected RFID " + rawRfid + " to " + entry.rfid_ID);
            }
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log("Successfully populated Guest Database with " + database.guestList.Count + " entries.");
    }
}