using System.Text.Json;

namespace Wurkset;

public class Workset<T>
{
    public long WorksetId { get; }
    public string WorksetPath { get; }
    public string WorksetDataFile => Path.Combine(WorksetPath, $"{typeof(T).Name}.json");
    public T Value { get; set; }
    public DateTime CreationTime => new DirectoryInfo(WorksetPath).CreationTime;
    public DateTime LastWriteTime => new FileInfo(WorksetDataFile).LastWriteTime;
    public List<DateTime> PriorVersionDates
    {
        get
        {
            List<DateTime> result = new();
            foreach (var backupFile in Directory.GetFiles(WorksetPath, $"{typeof(T).Name}.*.json"))
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
    public Workset(long worksetId, string worksetPath, T value)
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
            string backupfile = Path.Combine(WorksetPath, $"{typeof(T).Name}.{timestamp}.json");
            if (File.Exists(WorksetDataFile)) File.Move(WorksetDataFile, backupfile);
        }
        File.WriteAllText(WorksetDataFile, JsonSerializer.Serialize(Value));
    }
    public Workset<T> GetPriorVersionAsOfDate(DateTime dateTime)
    {
        //TODO Test this
        DateTime? closestDate = PriorVersionDates.Where(x => x >= dateTime)?.Min();

        if (closestDate is null) return this;

        string backupFilename = Path.Combine(WorksetPath, $"{typeof(T).Name}.{closestDate?.Ticks.ToString()}.json");

        return new Workset<T>(WorksetId, WorksetPath, JsonSerializer.Deserialize<T>(File.ReadAllText(backupFilename)) ?? throw new Exception("Could not deserialize backup file"));
    }
    public void Delete()
    {
        if (File.Exists(WorksetDataFile)) File.Delete(WorksetDataFile);
    }
}