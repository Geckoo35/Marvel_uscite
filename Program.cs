using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC_Uscite
{
    internal class Program
    {
        static async Task Main()
        {
            string url = "https://www.panini.it/shp_ita_it/fumetti-libri-riviste/calendario-delle-uscite/le-uscite-di-questa-settimana.html?pnn_editorial_line=Marvel";

            try
            {
                List<Fumetto> fumetti = await EstraiNomi.ExtractFumetti(url);

                if (fumetti.Count > 0)
                {
                    fumetti = fumetti.OrderBy(f => decimal.Parse(f.Prezzo, System.Globalization.CultureInfo.GetCultureInfo("it-IT"))).ToList();

                    var groupedFumetti = fumetti.GroupBy(f => f.Nome).ToList();
                    foreach (var group in groupedFumetti)
                    {
                        if (group.Count() > 1)
                        {
                            int count = 1;
                            foreach (Fumetto fumetto in group)
                            {
                                fumetto.Nome += count == 1 ? "" : $" Variant";
                                count++;
                            }
                        }
                    }

                    // Compila il template con nomi e prezzi
                    StringBuilder output = new StringBuilder();
                    output.AppendLine("➖🗯#Fumetti🗯➖\n");
                    output.AppendLine("➖➖➖➖➖🗯➖➖➖➖➖");
                    output.AppendLine("<b>ECCO I FUMETTI MARVEL IN USCITA QUESTA SETTIMANA!</b>");
                    output.AppendLine("➖➖➖➖➖🗯➖➖➖➖➖\n");

                    foreach (Fumetto fumetto in fumetti)
                    {
                        string line = $"🔘<i><b>\"{fumetto.Nome}\"</b> - \"{fumetto.Prezzo}\" €</i>\n";
                        line = line.Replace("\"", "");
                        output.AppendLine(line);
                    }

                    output.AppendLine("➖➖➖➖➖➖➖➖➖➖➖");
                    output.AppendLine("✍🏻Scritta da @Geckoexe");
                    output.AppendLine("➖➖➖➖➖➖➖➖➖➖➖\n");
                    output.AppendLine("🌐nerdalquadrato.it");
                    output.AppendLine("📢@NewsMarvelItaly");
                    output.AppendLine("👥@MarvelGroupItaly");

                    // Salva il template in un file di testo sul desktop
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = Path.Combine(desktopPath, "uscite_settimanali.txt");

                    File.WriteAllText(filePath, output.ToString());

                    Console.WriteLine("Il file 'uscite_settimanali.txt' è stato creato con successo sul desktop.");

                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Nessun fumetto trovato.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la richiesta: {ex.Message}");
            }
        }
    }
}
