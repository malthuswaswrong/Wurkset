using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Wurkset;
using Xunit;

namespace WurksetTests;

public class SmallRepositoryTests
{
    readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "Small");
    WorksetRepository cut;
    public SmallRepositoryTests()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = basePath
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        cut = new(ioptions);
    }
    [Fact]
    public void InvalidBaseDirectory()
    {
        Assert.Throws<ArgumentException>(() => new WorksetRepository(Options.Create(new WorksetRepositoryOptions())));
        Assert.Throws<ArgumentException>(() => new WorksetRepository(Options.Create(new WorksetRepositoryOptions() { BasePath = "" })));
        Assert.Throws<ArgumentException>(() => new WorksetRepository(Options.Create(new WorksetRepositoryOptions() { BasePath = "c:\\" })));
        Assert.Throws<IOException>(() => new WorksetRepository(Options.Create(new WorksetRepositoryOptions() { BasePath = "?invalid*" })));
    }
    [Fact]
    public void CreateRepository()
    {
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(basePath));
    }
    [Fact]
    public void Create10Worksets()
    {
        for (int i = 0; i < 10; i++)
        {
            var t = cut.Create(new TestDataA() { Id = i, Data = i.ToString() });
            Assert.Equal(i + 1, t.WorksetId);
        }
        Assert.Equal(11, cut.NextWorksetId);
    }
    [Fact]
    public void RetrieveWorkset()
    {
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestDataA() { Id = i, Data = i.ToString() });
        }
        
        Workset<TestDataA> verify = cut.GetById<TestDataA>(10);
        Assert.NotNull(verify);
        Assert.Equal(10, verify.Value?.Id);
        Assert.Equal("10", verify.Value?.Data);
    }
    [Fact]
    public void RetrieveAllWorksets()
    {
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestDataA() { Id = i, Data = i.ToString() });
        }

        int chk = 1;
        foreach(var t in cut.GetAll<TestDataA>())
        {
            Assert.Equal(chk, t?.WorksetId);
            Assert.Equal(chk, t?.Value?.Id);
            Assert.Equal(chk.ToString(), t?.Value?.Data);
            chk++;
        }
    }
    [Fact]
    public void SearchForWorkset()
    {
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestDataA() { Id = i, Data = i.ToString() });
        }
        
        var t = cut.GetAll<TestDataA>()
            .Where(x => x.Value?.Id % 3 == 0)
            .Select(x => x.Value)
            .ToList();
        
        Assert.Equal(3, t.Count());
    }
    [Fact]
    public void UpdateAWorkset()
    {
        Workset<TestDataA> t1 = cut.Create(new TestDataA() { Id = 1, Data = "1" });
        Assert.NotNull(t1);
        Assert.Equal(1, t1.WorksetId);
        Assert.NotNull(t1.Value);
        Assert.Equal("1", t1.Value.Data);
        t1.Value.Data = t1.Value?.Data + " a change";
        t1.Save();
        Workset<TestDataA> t2 = cut.GetById<TestDataA>(1);
        Assert.NotNull(t2);
        Assert.NotNull(t2.Value);
        Assert.Equal("1 a change", t2.Value?.Data);
    }
    [Fact]
    public void BackupCopy()
    {
        Workset<TestDataA> t1 = cut.Create(new TestDataA() { Id = 1, Data = "Version 1" });
        Assert.NotNull(t1);
        Assert.NotNull(t1.Value);
        Assert.Equal("Version 1", t1.Value.Data);
        Assert.Empty(t1.PriorVersionDates);
        t1.Value.Data = "Version 2";
        t1.Save(true);
        Assert.Single(t1.PriorVersionDates);
        Assert.NotNull(t1);
        Assert.NotNull(t1.Value);
        Assert.Equal("Version 2", t1.Value?.Data);
        
        Workset<TestDataA> t2 = t1.GetPriorVersionAsOfDate(DateTime.Parse("1901-01-01"));
        Assert.NotNull(t2);
        Assert.Equal(t1.WorksetId, t2.WorksetId);
        Assert.Equal("Version 1", t2.Value?.Data);
    }
    
    [Fact]
    public void TestSeachStartId()
    {
        for (int i = 0; i < 100; i++)
        {
            cut.Create(new TestDataA() { Id = i, Data = i.ToString() });
        }
        Assert.Equal(100, cut.GetAll<TestDataA>().Count());
        Assert.Equal(100, cut.GetAll<TestDataA>(new GetAllOptions { StartId = 1 }).Count());
        Assert.Equal(50, cut.GetAll<TestDataA>(new GetAllOptions { StartId = 51 }).Count());
        Assert.Single(cut.GetAll<TestDataA>(new GetAllOptions { StartId = 100 }));
        Assert.Empty(cut.GetAll<TestDataA>(new GetAllOptions { StartId = 101 }));
    }
    [Fact]
    public void TestTimes()
    {
        DateTime check = DateTime.Now;
        Thread.Sleep(1000);
        Workset<TestDataA> t1 = cut.Create(new TestDataA() { Id = 1, Data = "Version 1" });
        Assert.NotNull(t1);
        Assert.True(t1.CreationTime >= check);
        Assert.True(t1.LastWriteTime >= check);
    }
    [Fact]
    public void MultipleDataTypesInSameRepo()
    {
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestDataA() { Id = i, Data = i.ToString() });
            cut.Create(new TestDataB() { ChosenFruit = TestDataB.FRUITS.Apple });
        }
        Assert.Equal(10, cut.GetAll<TestDataA>().Count());
        Assert.Equal(10, cut.GetAll<TestDataB>().Count());
    }
}
