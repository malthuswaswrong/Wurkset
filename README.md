# Wurkset
A file-based repository that trades speed for size and simplicity.

A workset is a unit of work.

Wurkset lets you define a unit of work with a POCO.  You can easily create and save worksets to a filesystem.  You can enumerate over the full collection and filter with traditional Linq.  It also contains a crude version control that lets you access snapshots of your data at a given time.

# Example Code
The unit tests are a great way to explore the capabilities of the library, but here are a few common tasks.
## Instantiate a repository
### Directly make an IOptions object
```csharp
WorksetRepositoryOptions options = new() { BasePath = @"c:\Data" };
var ioptions = Options.Create(options);
WorksetRepository wsr = new(ioptions);
```
### Initialize options with Action
```csharp
WorksetRepository wsr = new WorksetRepository(options =>
        {
            options.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "WeightData");
        });
```
### Add to dependency injection using provided Extension method
```csharp
IHost host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddWurkset(options => {
                options.BasePath = @"c:\Data";
            });
        }).Build();
```
## Create a workset
```csharp
Workset<TestDataA> wsInstance = wsr.Create(new TestDataA() { Id = 1, Data = "Some test data" });
```
## Create a workset, change some data, save the change
```csharp
Workset<TestDataA> wsInstance = wsr.Create(new TestDataA() { Id = 1, Data = "Version 1" });
wsInstance.Value.Data = "Version 2";
wsInstance.Save();
```
## Get a workset by id and access the original object with .Value
```csharp
Workset<TestDataA> wsInstance = wsr.GetById<TestDataA>(worksetId);
Debug.WriteLine(wsInstance.Value.Data);
```
## Get your original object back without the Workset wrapper
```csharp
TestDataA myTestData = wsr.GetById<TestDataA>(worksetId).Value;
```
## Get your object as it appeared last week
```csharp
Workset<TestDataA> wsCurrent = wsr.GetById(worksetId);
Workset<TestDataA> wsLastWeek = wsCurrent.GetPriorVersionAsOfDate(DateTime.Now.AddDays(-7))
```
## Property containing list of all version times for the workset
```csharp
wsInstance.PriorVersionDates;
```
## Eumerate all worksets
```csharp
int chk = 1;
foreach(Workset<TestDataA> wsInstance in wsr.GetAll<TestDataA>())
{
    Assert.Equal(chk, wsInstance.WorksetId);
    Assert.Equal(chk, wsInstance.Value?.Id);
    Assert.Equal(chk.ToString(), wsInstance.Value?.Data);
    chk++;
}
```
## Search your object data and select the original object into a List\<T\> instead of the Workset wrapper object
```csharp
List<TestDataA> myDataOnlyList = wsr.GetAll<TestDataA>()
            .Where(x => x.Value?.Data.Contains("test"))
            .Select(x => x.Value)
            .ToList();
```
## Access the path of a workset
```csharp
wsInstance.WorksetPath;
```
# Additional Notes	
* FAQ:
 	* Why?
 		* Filesystem storage has the advantage of being large and cheap.  This comes at the tradeoff of speed.
 		* Mostly I want to learn how to make a GitHub repo and this is a project I've thought about for a long time.  I started programming in the late 90's at a company that didn't have a database and I spent many years managing large sets of data exclusivly through the filesystem.  It's a good cheap solution for "cold data" that doesn't need fast access or as a stand in for a real repository during development.
	* How can I store extra files?
		* The Workset class provides the path to the workset data directory.  Feel free to get the parent of that directory and go nuts.  Recommend leaving the "ws" sub directory alone.
	* How does Wurkset store the files?
		* Classes are stored on the filesystem in a nested directory structure using the worksetId as the parent directory and a "ws" subdirectory to contain the serialized json.  The workset id is a unique guid like structure generated with a library named NUlid (avalable on github and nuget).
	* How do I get the identity of the workset that was just created?
		* When you create or retrieve a workset it is wrapped in a generic class that contains WorksetId, WorksetPath, and various other properties.
		* See unit tests or examples for more information
	* How performant is Wurkset?
		* For create and retrieving by id it is "fine".  For searching it becoming noticably slow at "a few thousand" objects.
		* You can manage performance by making additional repositories.  You can even make a repository under a repository so you can group similar ideas together.  Increase "depth" to decrease "width".
* Be cautious about:
	* Concurrency.  You have to wait for the hard drive.  Consider implementing as a singleton.
	* Storing different classes in the same base directory.  Wurkset will enumerate faster if everything in the directory is the same class type.  It's simple to store each class in it's own directory.  Ex:
		* C:\Data\ClassA
		* C:\Data\ClassB
	* Cluster size. Filesystems store files in clusters.  A 1 b file could still consume 1 kb on disk due to how the filesystem is configured.
# WeightTracker
A simple WinForms application to demonstrate the repository.

# TODO
* [ ] Create automated build
* [ ] Create NuGet
* [ ] Create IWorkset interface
* [ ] Add "Rename"
	* Ex: Rename class from A to B
* [ ] Add "Move"
* [ ] Add Optional Index
	* Should be able to tag a property in the class with an attribute that will mark it as an index.
	* Then library will index on that attribute for faster searching
	* This is hard, but worth it for long term.
* [ ] Improve version control.
	* Can be more efficient with diffs instead of full copy.
