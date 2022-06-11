# Wurkset
A simple, slow, file-based workset library with crude version control.

A workset is a unit of work.

Wurkset lets you define a unit of work with a POCO.  You can easily create and save worksets to a filesystem.  You can enumerate over the full collection and filter with traditional Linq.

Filesystem storage has the advantage of being large and cheap.  This comes at the tradeoff of speed.

* Be cautious about:
	* Concurrency.  You have to wait for the hard drive.  Consider injecting as a singleton.
	* Storing different classes in the same base directory.  Workset will enumerate faster if everything in the directory is the same class type.  It's simple to store each class in it's own directory.  Ex:
		* C:\Data\ClassA
		* C:\Data\ClassB
	* Cluster size. Filesystems store files in clusters.  A 1 b file could still consume 1 kb on disk due to how the filesystem is configured.
	
* FAQ:
 	* Why?
 		* Mostly I want to learn how to make a GitHub repo and this is a project I've thought about for a long time.  I started programming in the late 90's at a company that didn't have a database and I spent many years managing large sets of data exclusivly through the filesystem.  It's a good cheap solution for "cold data" that doesn't need fast access or as a stand in for a real repository during development.
	* How do I store extra files?
		* The Workset class provides the path to the workset.  Go nuts.  Just leave the nameof(T).json file alone.
# Example Code
## Create a repository
### Directly make an options object
```
WorksetRepositoryOptions options = new WorksetRepositoryOptions() { BasePath = @"c:\Data" };
var ioptions = Options.Create(options);
WorksetRepository wsr = new(ioptions);
```
### Initialize options with Action
```
WorksetRepository wsr = new WorksetRepository(options =>
        {
            options.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "WeightData");
        });
```
### Add to dependency injection using provided Extension method
```
IHost host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddWurkset(options => {
                options.BasePath = @"c:\Data";
            });
        }).Build();
```
## Create a workset
```
Workset<TestDataA> wsNew = cut.Create(new TestDataA() { Id = 1, Data = "Some test data" });
```
## Save a workset
```
wsInstance.Save();
```
## Get a workset by id
```
Workset<TestDataA> verify = cut.GetById<TestDataA>(10);
```
## Enumerate all worksets
```
int chk = 1;
foreach(Workset<TestDataA> t in cut.GetAll<TestDataA>())
{
    Assert.Equal(chk, t.WorksetId);
    Assert.Equal(chk, t.Value?.Id);
    Assert.Equal(chk.ToString(), t.Value?.Data);
    chk++;
}
```
## Search for a workset(s)
```
List<TestDataA> wsList = cut.GetAll<TestDataA>()
            .Where(x => x.Value?.Data.Contains("test"))
            .Select(x => x.Value)
            .ToList();
```
## Important notes
* The repository BasePath can be either a full path or a relative path.  If relative it will be the executing assembly's working directory.

# TODO
* [ ] Create IWorkset interface
* [ ] Add "Rename"
	* Ex: Rename class from A to B
* [ ] Add Optional Index
	* Should be able to tag a property in the class with an attribute that will mark it as an index.
	* Then app will index on that attribute for faster searching
		* This is hard, but worth it for long term.
