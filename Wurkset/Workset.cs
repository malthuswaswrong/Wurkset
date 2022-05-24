using System.Text.Json;

namespace Wurkset;

public class Workset<T>
{
    public long WorksetId { get; }
    public string WorksetPath { get; }
    public T? Value { get; set; }
    public DateTime CreateTime => new FileInfo(Path.Combine(WorksetPath, "data.json")).CreationTime;
    public DateTime LastModifiedTime => new FileInfo(Path.Combine(WorksetPath, "data.json")).LastWriteTime;
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
    public Workset<T> GetPriorVersionAsOfDate(DateTime dateTime)
    {
        //TODO Test this
        DateTime? closestDate = PriorVersionDates.Where(x => x >= dateTime)?.Min();
        
        if(closestDate is null) return this;
        
        string backupFilename = Path.Combine(WorksetPath, $"data.{closestDate?.Ticks.ToString()}.json");

        return new Workset<T>(WorksetId, WorksetPath, JsonSerializer.Deserialize<T>(File.ReadAllText(backupFilename)));
    }
    public void Archive()
    {
        string datafile = Path.Combine(WorksetPath, "data.json");
        string archivefile = Path.Combine(WorksetPath, "data.archived.json");
        if (File.Exists(datafile)) File.Move(datafile, archivefile);
    }
}