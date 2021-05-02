using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[CheckBuildProjectConfigurations]
[GitHubActions(
    "Build",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] {"development", "master"},
    OnPullRequestBranches = new[] { "development", "master" })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Package);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    private AbsolutePath StagingDirectory = RootDirectory / "Staging";
    private AbsolutePath ArtifactsDirectory = RootDirectory / "Artifacts";
    private Project ModuleProject => Solution.GetProject("DotNetNuke.FAQs");
    private string Manifest => GlobFiles(RootDirectory / "Installation", "*.dnn").FirstOrDefault();
    private IReadOnlyCollection<string> LocalizationFiles => GlobFiles(RootDirectory / "App_LocalResources", "*.resx");
    private IReadOnlyCollection<string> Images => GlobFiles(RootDirectory / "Images", "*");
    private IReadOnlyCollection<string> Scripts => GlobFiles(RootDirectory / "Scripts", "*");
    private IReadOnlyCollection<string> OtherResources => GlobFiles(RootDirectory, "*.png", "*.ascx", "*.css");
    private IReadOnlyCollection<string> CleanupFiles => GlobFiles(RootDirectory / "Installation" / "Cleanup", "*");
    private IReadOnlyCollection<string> SqlDataProviderFiles => GlobFiles(RootDirectory / "Installation", "*.SqlDataProvider");
    private string ModuleAssembly => GlobFiles(RootDirectory / "bin", "DotNetNuke.Modules.FAQs.dll").FirstOrDefault();
    private string Version
    {
        get
        {
            var version = XmlTasks.XmlPeekSingle(Manifest, "dotnetnuke/packages/package/@version");
            int major = int.Parse(version.Substring(0, 2));
            int minor = int.Parse(version.Substring(3, 2));
            int patch = int.Parse(version.Substring(6, 2));
            return $"{major}.{minor}.{patch}";
        }
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(StagingDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGet($"restore {ModuleProject} -PackagesDirectory packages");
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetProjectFile(ModuleProject)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(Version)
                .SetFileVersion(Version));
                
        });

    Target Package => _ => _
        .DependsOn(Clean)
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            LocalizationFiles.ForEach(f => CopyFileToDirectory(f, StagingDirectory / "App_LocalResources"));
            Images.ForEach(f => CopyFileToDirectory(f, StagingDirectory / "Images"));
            Scripts.ForEach(f => CopyFileToDirectory(f, StagingDirectory / "Scripts"));
            OtherResources.ForEach(f => CopyFileToDirectory(f, StagingDirectory));
            CompressionTasks.Compress(StagingDirectory, StagingDirectory / "Resources.zip");
            var resourceFiles = GlobFiles(StagingDirectory, "*");
            resourceFiles.ForEach(f => {
                if (!f.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
                {
                    DeleteFile(f);
                }
            });
            var resourcesDirectories = GlobDirectories(StagingDirectory, "*");
            resourcesDirectories.ForEach(d => DeleteDirectory(d));

            CopyFileToDirectory(ModuleAssembly, StagingDirectory / "bin");
            CleanupFiles.ForEach(f => CopyFileToDirectory(f, StagingDirectory / "Installation" / "CleanUp"));
            SqlDataProviderFiles.ForEach(f => CopyFileToDirectory(f, StagingDirectory / "Installation"));
            CopyFileToDirectory(Manifest, StagingDirectory);
            CopyFileToDirectory(RootDirectory / "License.txt", StagingDirectory);
            CopyFileToDirectory(RootDirectory / "ReleaseNotes.txt", StagingDirectory);
            CompressionTasks.Compress(StagingDirectory, ArtifactsDirectory / $"DNN_FAQs_{Version}_Install.zip");
            DeleteDirectory(StagingDirectory);
        });

    Target Deploy => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var parentDirectory = new DirectoryInfo(RootDirectory.Parent);
            if (parentDirectory.Name == "DesktopModules")
            {
                var deployDirectory = RootDirectory.Parent / "FAQs";
                var binDirectory = RootDirectory.Parent.Parent / "bin";

                // Only copy the dll if it changed to prevent a DNN restart.
                bool allBytesEqual = false;
                using (var md5 = MD5.Create())
                {
                    byte[] oldMd5;
                    byte[] newMd5;
                    using (var oldDll = File.OpenRead(binDirectory / "DotNetNuke.Modules.FAQs.dll"))
                    {
                        oldMd5 = md5.ComputeHash(oldDll);
                    }
                    using (var newDll = File.OpenRead(ModuleAssembly))
                    {
                        newMd5 = md5.ComputeHash(newDll);
                    }

                    if (newMd5.Length == oldMd5.Length)
                    {
                        int i = 0;
                        while ((i < newMd5.Length) && (newMd5[i] == oldMd5[i]))
                        {
                            i++;
                        }
                        if (i == newMd5.Length)
                        {
                            allBytesEqual = true;
                        }
                    }
                }
                if (allBytesEqual)
                {
                    Logger.Info("Library was not deployed as it was unchanged.");
                }
                else
                {
                    CopyFileToDirectory(ModuleAssembly, binDirectory, FileExistsPolicy.Overwrite);
                    Logger.Info("Deployed library");
                }

                // Copy resource files
                EnsureCleanDirectory(deployDirectory);
                LocalizationFiles.ForEach(f => CopyFileToDirectory(f, deployDirectory / "App_LocalResources"));
                Images.ForEach(f => CopyFileToDirectory(f, deployDirectory / "Images"));
                Scripts.ForEach(f => CopyFileToDirectory(f, deployDirectory / "Scripts"));
                OtherResources.ForEach(f => CopyFileToDirectory(f, deployDirectory));
            }
        });
}
