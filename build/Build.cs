using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities.Collections;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release; // IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [Parameter("Version to be injected in the Build")]
    public string Version { get; set; } = $"2.0.0";

    [Parameter("The Buildnumber provided by the CI")]
    public int BuildNo = 30;

    [Parameter("Is RC Version")]
    public bool IsRc = false;

    [Parameter("Run UnitTests on build")]
    public bool RunTests = true;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath TestsDirectory => RootDirectory / "src" / "tests";

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    AbsolutePath DeployPath => (AbsolutePath)"C:" / "Projects" / "NuGet Store";

    [Parameter("Full name of the Project. This is defined in parameters.json")]
    readonly string ProjectName;

    [Parameter("URL of the SonarQube Server")]
    public string SonarServer = "https://sonarcloud.io";

    [Parameter("Login Token of the SonarQube Server")]
    public string SonarToken = "";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(d => d.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(d => d.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean, Restore)
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetVersion($"{Version}.{BuildNo}")
                .SetAssemblyVersion($"{Version}.{BuildNo}")
                .SetFileVersion(Version)
                .SetInformationalVersion($"{Version}.{BuildNo}")
                .AddProperty("PackageVersion", PackageVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            if (!RunTests)
            {
                return;
            }

            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(true)
                .SetExcludeByFile("MeasureMap.IntegrationTest")
                .EnableNoRestore());
        });

    Target Release => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            // copy to artifacts folder
            foreach (var file in Directory.GetFiles(RootDirectory, $"*.{PackageVersion}.nupkg", SearchOption.AllDirectories))
            {
                ((AbsolutePath)file).CopyToDirectory(ArtifactsDirectory, ExistsPolicy.FileOverwrite);
                Serilog.Log.Write(Serilog.Events.LogEventLevel.Information, "Deployed {0} to {1}", file, RootDirectory);
            }

            foreach (var file in Directory.GetFiles(RootDirectory, $"*.{PackageVersion}.snupkg", SearchOption.AllDirectories))
            {
                ((AbsolutePath)file).CopyToDirectory(ArtifactsDirectory, ExistsPolicy.FileOverwrite);
                Serilog.Log.Write(Serilog.Events.LogEventLevel.Information, "Deployed {0} to {1}", file, RootDirectory);
            }
        });

    Target Deploy => _ => _
        .DependsOn(Release)
        .Executes(() =>
        {
            // copy to local store
            foreach (var file in Directory.GetFiles(RootDirectory, $"*.{PackageVersion}.nupkg", SearchOption.AllDirectories))
            {
                ((AbsolutePath)file).CopyToDirectory(DeployPath, ExistsPolicy.FileOverwrite);
                Serilog.Log.Write(Serilog.Events.LogEventLevel.Information, "Deployed {0} to {1}", file, DeployPath);
            }

            foreach (var file in Directory.GetFiles(RootDirectory, $"*.{PackageVersion}.snupkg", SearchOption.AllDirectories))
            {
                ((AbsolutePath)file).CopyToDirectory(DeployPath, ExistsPolicy.FileOverwrite);
                Serilog.Log.Write(Serilog.Events.LogEventLevel.Information, "Deployed {0} to {1}", file, DeployPath);
            }
        }
        );

    string PackageVersion
        => IsRc ? BuildNo < 10 ? $"{Version}-RC0{BuildNo}" : $"{Version}-RC{BuildNo}" : Version;

}
