using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GuestEntry
{
    public string rfid_ID;
    public string guestName;
    public string audioFilename;
}


[CreateAssetMenu(fileName = "GuestDatabase", menuName = "Event/Guest Database")]
public class GuestDatabase : ScriptableObject
{
    public List<GuestEntry> guestList = new List<GuestEntry>();

    public GuestEntry GetGuestByID(string id)
    {
        foreach (var entry in guestList)
        {
            if (entry.rfid_ID == id)
            {
                return entry;
            }
        }
        return null;
    }
}