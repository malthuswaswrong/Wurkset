using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using Wurkset;
using Xunit;

namespace WurksetTests;

public class SmallRepositoryTests
{
    const string baseDataDir = @"D:\src\WurksetSolution\TestData\Small";
    [Fact]
    public void CreateRepository()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "CreateRepository")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
    }
    [Fact]
    public void Create10Worksets()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "Create10Worksets")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
        for (int i = 0; i < 10; i++)
        {
            var t = cut.Create(new TestData() { Id = i, Data = i.ToString() });
            Assert.Equal(i + 1, t.WorksetId);
        }
        Assert.Equal(11, cut.NextWorksetId);
    }
    [Fact]
    public void RetrieveWorkset()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "RetrieveWorkset")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestData() { Id = i, Data = i.ToString() });
        }
        
        Workset<TestData?> verify = cut.GetById<TestData>(10);
        Assert.NotNull(verify);
        Assert.Equal(10, verify.Value?.Id);
        Assert.Equal("10", verify.Value?.Data);
    }
    [Fact]
    public void RetrieveAllWorksets()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "RetrieveAllWorksets")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestData() { Id = i, Data = i.ToString() });
        }

        int chk = 1;
        foreach(var t in cut.GetAll<TestData>())
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
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "SearchForWorkset")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
        for (int i = 1; i <= 10; i++)
        {
            cut.Create(new TestData() { Id = i, Data = i.ToString() });
        }
        
        var t = cut.GetAll<TestData>()
            .Where(x => x.Value?.Id % 3 == 0)
            .Select(x => x.Value)
            .ToList();
        
        Assert.Equal(3, t.Count());
    }
    [Fact]
    public void UpdateAWorkset()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "UpdateAWorkset")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
        Workset<TestData> t1 = cut.Create(new TestData() { Id = 1, Data = "1" });
        Assert.NotNull(t1);
        Assert.Equal(1, t1.WorksetId);
        Assert.NotNull(t1.Value);
        Assert.Equal("1", t1.Value?.Data);
        t1.Value.Data = t1.Value?.Data + " a change";
        t1.Save();
        Workset<TestData?> t2 = cut.GetById<TestData>(1);
        Assert.NotNull(t2);
        Assert.NotNull(t2.Value);
        Assert.Equal("1 a change", t2.Value?.Data);
    }
    [Fact]
    public void BackupCopy()
    {
        WorksetRepositoryOptions options = new WorksetRepositoryOptions()
        {
            BasePath = Path.Combine(baseDataDir, "BackupCopy")
        };
        if (Directory.Exists(options.BasePath))
        {
            Directory.Delete(options.BasePath, true);
        }
        var ioptions = Options.Create(options);
        WorksetRepository cut = new(ioptions);
        Assert.Equal(1, cut.NextWorksetId);
        Assert.True(Directory.Exists(options.BasePath));
        Workset<TestData?> t1 = cut.Create(new TestData() { Id = 1, Data = "Version 1" });
        Assert.NotNull(t1);
        Assert.NotNull(t1.Value);
        Assert.Equal("Version 1", t1.Value?.Data);
        Assert.Empty(t1.PriorVersionDates);
        t1.Value.Data = "Version 2";
        t1.Save(true);
        Assert.Single(t1.PriorVersionDates);
        Assert.NotNull(t1);
        Assert.NotNull(t1.Value);
        Assert.Equal("Version 2", t1.Value?.Data);
        
        Workset<TestData?> t2 = t1.GetPriorVersionAsOfDate(DateTime.Parse("1901-01-01"));
        Assert.NotNull(t2);
        Assert.Equal(t1.WorksetId, t2.WorksetId);
        Assert.Equal("Version 1", t2.Value?.Data);
    }
    //TODO Test UTC dates working properly for prior versions
    //TODO Test archiving
}
