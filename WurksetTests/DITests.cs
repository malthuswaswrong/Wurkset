﻿using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Wurkset;
using System.IO;

namespace WurksetTests;

public class DITests
{
    WorksetRepository? wurkset;
    private readonly string BaseDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "DependencyInjection");
    public DITests()
    {
        IHost host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddWurkset(options => {
                options.BasePath = BaseDir;
            });
        }).Build();
        
        wurkset = host.Services.GetService<WorksetRepository>();        
    }
    [Fact]
    public void ConfirmBasePath()
    {
        Assert.Equal(BaseDir, wurkset?.Options.Value.BasePath);
        Assert.True(Directory.Exists(BaseDir));
    }
    
}