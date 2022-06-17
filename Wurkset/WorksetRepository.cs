using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Wurkset;

public class WorksetRepository
{
    private long lastWorksetId;
    public long NextWorksetId
    {
        get
        {
            long result = lastWorksetId;
            while (Directory.Exists(Path.Combine(WorksetRepositoryOptions.Value.BasePath, result.ToPath())))
            {
                result++;
            }
            lastWorksetId = result - 1;
            return result;
        }
    }
    private string pathBulder(long id) => Path.Combine(WorksetRepositoryOptions.Value.BasePath, id.ToPath(), "ws");
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
        if (!Directory.Exists(Path.Combine(this.WorksetRepositoryOptions.Value.BasePath, "0")))
        {
            //Reserve directory "0" for internal use.
            Directory.CreateDirectory(Path.Combine(this.WorksetRepositoryOptions.Value.BasePath, "0"));
        }
        FixNextWorksetId();
    }
    public WorksetRepository(Action<WorksetRepositoryOptions> configuration)
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions();
        configuration(options);
        this.WorksetRepositoryOptions = Options.Create(options);
    }
    private void FixNextWorksetId()
    {
        if (!Directory.Exists(Path.Combine(WorksetRepositoryOptions.Value.BasePath, "1")))
        {
            lastWorksetId = 0;
        }

        long maxSearch = 2;
        while (Directory.Exists(Path.Combine(WorksetRepositoryOptions.Value.BasePath, maxSearch.ToPath())))
        {
            maxSearch *= 2;
        }
        long minSearch = maxSearch / 2;
        lastWorksetId = BinarySearch(minSearch, maxSearch);
    }

    private long BinarySearch(long min, long max)
    {
        if (max - min <= 1)
        {
            return min;
        }
        long mid = (min + max) / 2;
        if (Directory.Exists(Path.Combine(WorksetRepositoryOptions.Value.BasePath, mid.ToPath())))
        {
            return BinarySearch(mid, max);
        }
        else
        {
            return BinarySearch(min, mid);
        }
    }

    public Workset<T> Create<T>(T data)
    {
        long newId = NextWorksetId;
        string path = pathBulder(newId);
        Directory.CreateDirectory(path);
        string datafile = Path.Combine(path, $"{typeof(T).Name}.json");
        File.WriteAllText(datafile, JsonSerializer.Serialize(data));
        return new Workset<T>(newId, path, data);
    }
    public Workset<T> GetById<T>(long id)
    {
        string path = pathBulder(id);
        string datafile = Path.Combine(path, $"{typeof(T).Name}.json");
        if (!File.Exists(datafile))
        {
            throw new FileNotFoundException($"Workset of type {nameof(T)} with id {id} not found");
        }
        T data = JsonSerializer.Deserialize<T>(File.ReadAllText(datafile)) ?? throw new Exception("Data is null");
        return new Workset<T>(id, path, data);
    }
    public IEnumerable<Workset<T>> GetAll<T>(GetAllOptions? getOptions = null)
    {
        long id = getOptions != null && getOptions.StartId.HasValue ? getOptions.StartId.Value : 1;
        int step = 1;
        if (getOptions != null && getOptions.Descending.HasValue && getOptions.Descending.Value)
        {
            step = -1;
            if (!getOptions.StartId.HasValue)
            {
                id = NextWorksetId - 1;
            }
        }
        for (; id < NextWorksetId; id += step)
        {
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
