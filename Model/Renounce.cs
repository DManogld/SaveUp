using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Filename = $"{Path.GetRandomFileName()}.note.txt";
            Date = DateTime.Now;
            Text = string.Empty;
            Preis = string.Empty;
        }

        public void Save()
        {
            string data = $"Artikel:{Text} | Preis: CHF.- {Preis}";
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, Filename), data);
        }

        public void Delete() =>
            File.Delete(Path.Combine(FileSystem.AppDataDirectory, Filename));


        public static Renounce Load(string filname)
        {
            filname = Path.Combine(FileSystem.AppDataDirectory, filname);

            if (!File.Exists(filname))
                throw new FileNotFoundException("Unabel to find the file on locale starage", filname);

            return new Renounce()
            {
                Filename = Path.GetFileName(filname),
                Text = File.ReadAllText(filname),
                Preis = File.ReadAllText(filname),
                Date = File.GetCreationTime(filname)
            };
        }

        public static IEnumerable<Renounce> LoadAll()
        {
            string appDataPath = FileSystem.AppDataDirectory;

            return Directory
                // Select the file name from the directory
                .EnumerateFiles(appDataPath, "*.notes.txt")
                //Each file is used to Load the Note
                .Select(filename => Renounce.Load(Path.GetFileName(filename)))
                // Order by...
                .OrderByDescending(note => note.Date);
        }
    }


}
