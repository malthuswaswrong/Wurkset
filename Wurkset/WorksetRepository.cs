using Microsoft.Extensions.Options;
using NUlid;
using System.Text.Json;

namespace Wurkset;

public class WorksetRepository
{
    private string pathBulder(Ulid worksetId) => Path.Combine(WorksetRepositoryOptions.Value.BasePath, worksetId.ToString(), "ws");
    public readonly IOptions<WorksetRepositoryOptions> WorksetRepositoryOptions;

    public WorksetRepository(IOptions<WorksetRepositoryOptions> options)
    {
        this.WorksetRepositoryOptions = options;
        if (String.IsNullOrWhiteSpace(options.Value.BasePath))
        {
            throw new ArgumentException("BasePath is not set");
        }
        if (String.Compare(this.WorksetRepositoryOptions.Value.BasePath, @"c:\", true) == 0)
        {
            //I'm not taking on the responsibility of this library bricking someone's C drive
            throw new ArgumentException(@"You're not allowed to use C:\ as the base path.");
        }
        if (!Directory.Exists(this.WorksetRepositoryOptions.Value.BasePath))
        {
            Directory.CreateDirectory(this.WorksetRepositoryOptions.Value.BasePath);
        }
    }
    public WorksetRepository(Action<WorksetRepositoryOptions> configuration)
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions();
        configuration(options);
        this.WorksetRepositoryOptions = Options.Create(options);
    }

    public Workset<T> Create<T>(T data)
    {
        Ulid newId = Ulid.NewUlid();
        string path = pathBulder(newId);
        Directory.CreateDirectory(path);
        string datafile = Path.Combine(path, $"{typeof(T).Name}.json");
        File.WriteAllText(datafile, JsonSerializer.Serialize(data));
        return new Workset<T>(newId, path, data);
    }
    public Workset<T> GetById<T>(Ulid worksetId)
    {
        string path = pathBulder(worksetId);
        string datafile = Path.Combine(path, $"{typeof(T).Name}.json");
        if (!File.Exists(datafile))
        {
            throw new FileNotFoundException($"Workset of type {nameof(T)} with id {worksetId} not found");
        }
        T data = JsonSerializer.Deserialize<T>(File.ReadAllText(datafile)) ?? throw new Exception("Data is null");
        return new Workset<T>(worksetId, path, data);
    }
    public IEnumerable<Workset<T>> GetAll<T>()
    {
        foreach(var wsDir in Directory.GetDirectories(WorksetRepositoryOptions.Value.BasePath))
        {
            Ulid id = Ulid.Parse(Path.GetFileName(wsDir));
            string path = pathBulder(id);
            string datafile = Path.Combine(path, $"{typeof(T).Name}.json");

            if (File.Exists(datafile))
            {
                T data = JsonSerializer.Deserialize<T>(File.ReadAllText(datafile)) ?? throw new Exception("Data is null");
                yield return new Workset<T>(id, path, data);
            }
        }
    }
}
