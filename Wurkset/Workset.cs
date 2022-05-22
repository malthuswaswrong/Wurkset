using System.Text.Json;

namespace Wurkset;

public class Workset<T>
{
    public long WorksetId { get; set; }
    public string WorksetPath { get; set; }
    public T? Value { get; set; }
    public List<DateTime> PriorVersionDates
    {
        get
        {
            List<DateTime> result = new();
            foreach (var backupFile in Directory.GetFiles(WorksetPath, "data.*.json"))
            {
                string fname = Path.GetFileName(backupFile);
                string timestampstring = fname.Split('.')[1];
                result.Add(new DateTime(long.Parse(timestampstring)));
            }
            return result;
        }
    }
    public Workset(long worksetId, string worksetPath, T? value)
    {
        WorksetId = worksetId;
        WorksetPath = worksetPath;
        Value = value;
    }
    public void Save()
    {
        string datafile = Path.Combine(WorksetPath, "data.json");
        if (File.Exists(datafile)) File.Delete(datafile);
        File.WriteAllText(datafile, JsonSerializer.Serialize(Value));
    }
    public void Save(bool keepVersionedCopy)
    {
        if (!keepVersionedCopy)
        {
            Save();
            return;
        }
        long timestamp = DateTime.UtcNow.Ticks;
        string datafile = Path.Combine(WorksetPath, "data.json");
        string backupfile = Path.Combine(WorksetPath, $"data.{timestamp}.json");
        if (File.Exists(datafile)) File.Move(datafile, backupfile);
        File.WriteAllText(datafile, JsonSerializer.Serialize(Value));
    }
    public Workset<T> GetPriorVersionAsOfDate(DateTime utcDateTime)
    {
        DateTime? closestDate = PriorVersionDates.Where(x => x >= utcDateTime)?.Min();
        
        if(closestDate is null) return this;
        
        string backupFilename = Path.Combine(WorksetPath, $"data.{closestDate?.Ticks.ToString()}.json");

        return new Workset<T>(WorksetId, WorksetPath, JsonSerializer.Deserialize<T>(File.ReadAllText(backupFilename)));
    }
    public void Archive()
    {
        string datafile = Path.Combine(WorksetPath, "data.json");
        string archivefile = Path.Combine(WorksetPath, "data.archived");
        if (File.Exists(datafile)) File.Move(datafile, archivefile);
    }
    
    //TODO Add comment to stress that all subdirectories will be deleted.
    public void Archive(bool permanentlyDeleteData)
    {
        if (permanentlyDeleteData)
        {
            foreach (var file in Directory.GetFiles(WorksetPath))
            {
                File.Delete(file);
            }
            foreach (var dir in Directory.GetDirectories(WorksetPath))
            {
                Directory.Delete(dir, true);
            }
        }
        else
        {
            Archive();
        }
    }
}