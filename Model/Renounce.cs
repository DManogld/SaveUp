using System.Text.Json;

namespace SaveUp.Model
{
    internal class Renounce
    {
        public string Filename { get; set; }
        public string Text { get; set; }
        public string Preis { get; set; }
        public DateTime Date { get; set; }

        public Renounce()
        {
            Filename = $"{Path.GetRandomFileName()}.renounce.txt";
            Date = DateTime.Now;
            Text = string.Empty;
            Preis = string.Empty;
        }

        /// <summary>
        /// Speichert ein Renounce-Objekt in einer Textdatei und in der JSON-Datei
        /// </summary>
        public void Save()
        {
            string data = $"Artikel:{Text}|Preis:{Preis}";
            string filePath = Path.Combine(FileSystem.AppDataDirectory, Filename);
            File.WriteAllText(filePath, data);
            SaveToJson();
        }

        /// <summary>
        /// Speichert eine Liste von Renounce-Objekten in der JSON-Datei
        /// </summary>
        private void SaveToJson()
        {
            List<Renounce> renounceList = LoadAll().ToList();
            Renounce existingRenounce = renounceList.FirstOrDefault(r => r.Filename == this.Filename);
            if (existingRenounce != null)
            {
                // Aktualisiere den vorhandenen Eintrag
                existingRenounce.Date = this.Date;
                existingRenounce.Preis = this.Preis;
            }
            else
            {
                // Füge den neuen Eintrag hinzu
                renounceList.Add(this);
            }
            string json = JsonSerializer.Serialize(renounceList);
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, "renounceList.json"), json);
        }


        /// <summary>
        /// Löscht die Textdatei und das Renounce-Objekt aus der JSON-Datei
        /// </summary>
        public void Delete()
        {
            File.Delete(Path.Combine(FileSystem.AppDataDirectory, Filename));
            RemoveFromJson();
        }


        /// <summary>
        /// Löscht alle Textdateien und alle Renounce-Objekte aus der JSON-Datei
        /// </summary>
        public void DeleteAll()
        {
            string appDataPath = FileSystem.AppDataDirectory;

            if (!Directory.Exists(appDataPath))
            {
                return;
            }
            foreach (string file in Directory.GetFiles(appDataPath, "*.renounce.txt"))
            {
                File.Delete(file);
            }
            // Lösche die JSON-Datei mit der Liste der renounce-Objekte
            string jsonFile = Path.Combine(appDataPath, "renounceList.json");
            if (File.Exists(jsonFile))
            {
                File.Delete(jsonFile);
            }
        }

        /// <summary>
        /// Entfernt das Renounce-Objekt aus der JSON-Datei
        /// </summary>
        private void RemoveFromJson()
        {
            List<Renounce> renounceList = LoadAll().ToList();
            renounceList.Remove(renounceList.Find(r => r.Filename == Filename));
            string json = JsonSerializer.Serialize(renounceList);
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, "renounceList.json"), json);
        }


        /// <summary>
        /// Lädt ein Renounce-Objekt aus einer Textdatei
        /// </summary>
        /// <param name="filname"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static Renounce Load(string filname)
        {
            filname = Path.Combine(FileSystem.AppDataDirectory, filname);
            if (!File.Exists(filname))
                throw new FileNotFoundException("Unable to find the file on locale starage", filname);

            string[] lines = File.ReadAllLines(filname);
            string[] splitLine = lines[0].Split('|');
            return new Renounce()
            {
                Filename = Path.GetFileName(filname),
                Text = splitLine[0].Substring(8),
                Preis = splitLine[1].Substring(6),
                Date = File.GetCreationTime(filname)
            };
        }

        /// <summary>
        /// Lädt alle Renounce-objekte aus dem lokalen Speicher in absteigender Reihenfolge ihres Erstellungsdatums.
        /// </summary>
        /// <returns>An IEnumerable collection of Renounce objects.</returns>
        public static IEnumerable<Renounce> LoadAll()
        {
            string appDataPath = FileSystem.AppDataDirectory;
            if (!File.Exists(Path.Combine(appDataPath, "renounceList.json")))
            {
                File.Create(Path.Combine(appDataPath, "renounceList.json")).Close();
            }
            string json = File.ReadAllText(Path.Combine(appDataPath, "renounceList.json"));
            if (json == string.Empty)
            {
                return new List<Renounce>();
            }
            List<Renounce> renounceList = JsonSerializer.Deserialize<List<Renounce>>(json);
            return renounceList.OrderByDescending(r => r.Date);
        }
    }
}
