using System.Text.Json;

namespace Wurkset;

public class Workset<T>
{
    public long WorksetId { get; }
    public string WorksetPath { get; }
    public string WorksetDataFile => (!Archived) ? Path.Combine(WorksetPath, "data.json") : Path.Combine(WorksetPath, "data.archived.json");
    public T? Value { get; set; }
    public bool Archived => (File.Exists(Path.Combine(WorksetPath, "data.archived.json")));
    public DateTime CreationTime => new DirectoryInfo(WorksetPath).CreationTime;
    public DateTime LastWriteTime => new FileInfo(WorksetDataFile).LastWriteTime;
    public List<DateTime> PriorVersionDates
    {
        get
        {
            List<DateTime> result = new();
            foreach (var backupFile in Directory.GetFiles(WorksetPath, "data.*.json"))
            {
                string fname = Path.GetFileName(backupFile);
                if (long.TryParse(fname.Split('.')[1], out long timestamp))
                {
                    try
                    {
                        DateTime t = new DateTime(timestamp);
                        result.Add(t);
                    }
                    catch (Exception) { }
                }
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
    public void Save(bool keepVersionedCopy = false)
    {
        if (keepVersionedCopy)
        {
            long timestamp = DateTime.Now.Ticks;
            string backupfile = Path.Combine(WorksetPath, $"data.{timestamp}.json");
            if (File.Exists(WorksetDataFile)) File.Move(WorksetDataFile, backupfile);
        }
        File.WriteAllText(WorksetDataFile, JsonSerializer.Serialize(Value));
    }
    public Workset<T> GetPriorVersionAsOfDate(DateTime dateTime)
    {
        //TODO Test this
        DateTime? closestDate = PriorVersionDates.Where(x => x >= dateTime)?.Min();

        if (closestDate is null) return this;

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