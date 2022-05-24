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
            while (Directory.Exists(Path.Combine(options.BasePath, result.ToPath())))
            {
                result++;
            }
            lastWorksetId = result - 1;
            return result;
        }
    }
    private string pathBulder(long id) => Path.Combine(options.BasePath, id.ToPath(), "ws");
    private readonly WorksetRepositoryOptions options;

    public WorksetRepository(IOptions<WorksetRepositoryOptions> ioptions)
    {
        this.options = ioptions.Value;
        if(String.Compare(this.options.BasePath, @"c:\", true ) == 0)
        {
            //I'm not taking on the responsibility of this library bricking someone's C drive
            throw new Exception(@"You're not allowed to use C:\ as the base path.");
        }
        if (!Directory.Exists(this.options.BasePath))
        {
            Directory.CreateDirectory(this.options.BasePath);
        }
        if (!Directory.Exists(Path.Combine(this.options.BasePath, "0")))
        {
            //Reserve directory "0" for internal use.
            Directory.CreateDirectory(Path.Combine(this.options.BasePath, "0"));
        }
        lastWorksetId = QueryLastUsedId();
    }
    private long QueryLastUsedId()
    {
        if (!Directory.Exists(Path.Combine(options.BasePath, "1")))
        {
            return 1;
        }
        
        long maxSearch = 2;        
        while (Directory.Exists(Path.Combine(options.BasePath, maxSearch.ToPath())))
        {
            maxSearch *= 2;
        }
        long minSearch = maxSearch / 2;
        long result = BinarySearch(minSearch, maxSearch);
        return result;
    }

    private long BinarySearch(long min, long max)
    {
        if (max - min <= 1)
        {
            return min;
        }
        long mid = (min + max) / 2;
        if (Directory.Exists(Path.Combine(options.BasePath, mid.ToPath())))
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
        string datafile = Path.Combine(path, "data.json");
        File.WriteAllText(datafile, JsonSerializer.Serialize(data));
        return new Workset<T>(newId, path, data);
    }
    public Workset<T?> GetById<T>(long id)
    {
        string path = pathBulder(id);
        string datafile = Path.Combine(path, "data.json");
        if (!File.Exists(datafile))
        {
            throw new FileNotFoundException($"Workset {id} not found");
        }
        T? data = JsonSerializer.Deserialize<T>(File.ReadAllText(datafile));
        return new Workset<T?>(id, path, data);
    }
    public IEnumerable<Workset<T?>> GetAll<T>(GetAllOptions? getOptions = null)
    {
        long id = getOptions != null && getOptions.StartingId.HasValue ? getOptions.StartingId.Value : 1;
        while (Directory.Exists(Path.Combine(options.BasePath, id.ToPath())))
        {
            string path = pathBulder(id);
            string datafile = Path.Combine(path, "data.json");
            
            if (!File.Exists(datafile))
            {
                if (getOptions != null && getOptions.IncludeArchived.HasValue && getOptions.IncludeArchived.Value)
                {
                    datafile = Path.Combine(path, "data.archived.json");
                    if (!File.Exists(datafile))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
            T? data = JsonSerializer.Deserialize<T>(File.ReadAllText(datafile));
            yield return new Workset<T?>(id, path, data);
            id++;
        }
    }
}
