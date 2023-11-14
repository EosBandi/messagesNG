using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace MpMessagesControl
{

    public enum FieldType
    {
        Text,
        Number,
        Boolean,
        List,
        Range
    }

    public class SettingsItem
    {
        [JsonPropertyAttribute("Settingname")]
        public string name;

        [JsonPropertyAttribute("DisplayName")]
        public string displayName;

        [JsonPropertyAttribute("Description")]
        public string description;

        [JsonPropertyAttribute("FieldType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldType fieldType;

        [JsonPropertyAttribute("DefaultValue")]
        public string defaultValue;

        [JsonPropertyAttribute("FixValues")]
        public string[] fixValues;

        [JsonPropertyAttribute("RangeMin")]
        public float rangeMin;

        [JsonPropertyAttribute("RangeMax")]
        public float rangeMax;

        [JsonPropertyAttribute("Persistent")]
        public bool persistent;

        [JsonIgnoreAttribute]
        public Control control;

        [JsonIgnoreAttribute]
        public string value;
    }

    public class PluginSettings
    {
        internal List<SettingsItem> SettingsTable = new List<SettingsItem>();

        //get settingstable
        public List<SettingsItem> getSettingsTable()
        {
            return SettingsTable;
        }

        public PluginSettings(string settingsFileName)
        {


            //Initialise settings table
            SettingsTable.Clear();
            //Check if file settingsFileName exists
            if (System.IO.File.Exists(settingsFileName))
            {
                //Read the file
                string json = System.IO.File.ReadAllText(settingsFileName);
                //Deserialize the file
                SettingsTable = JsonConvert.DeserializeObject<List<SettingsItem>>(json);
                Console.WriteLine("Settings file loaded");
            }

            // Update value fields from default values
            foreach (SettingsItem i in SettingsTable)
            {
                i.value = i.defaultValue;
            }

        }

        public void setSetting(string name, string value)
        {
            foreach (SettingsItem i in SettingsTable)
            {
                if (i.name == name)
                {
                    i.value = value;
                    return;
                }
            }
        }

        public string getSetting(string name)
        {
            //Get value of the setting 
            foreach (SettingsItem i in SettingsTable)
            {
                if (i.name == name)
                {
                    return (i.value);
                }
            }
            return "";
        }

        // Test functions
        public void writeOutSettingsDef(string settingsFileName)
        {
            //Serialize the settings table
            string json = JsonConvert.SerializeObject(SettingsTable, Formatting.Indented);
            //Write the file
            System.IO.File.WriteAllText(settingsFileName, json);
        }

        public void generateTestData()
        {

            string[] fixValues = { "All", "Some", "None" };
            SettingsTable.Add(new SettingsItem()
            { defaultValue = "All", description = "Show all messages", displayName = "Show all messages", fieldType = FieldType.List, fixValues = fixValues, name = "ShowAllMessages" });
            SettingsTable.Add(new SettingsItem()
            { defaultValue = "True", description = "Show all messages", displayName = "Show all messages", fieldType = FieldType.Boolean, fixValues = null, name = "ShowAllMessages" });
            SettingsTable.Add(new SettingsItem()
            { defaultValue = "Miafaszom", description = "Show all messages", displayName = "Show all messages", fieldType = FieldType.Text, fixValues = null, name = "ShowAllMessages" });
            SettingsTable.Add(new SettingsItem()
            { defaultValue = "0", description = "Show all messages", displayName = "Show all messages", fieldType = FieldType.Number, fixValues = null, name = "ShowAllMessages" });

        }

    }
}
