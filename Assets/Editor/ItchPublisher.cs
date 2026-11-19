using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Builds the game for macOS, Windows, and WebGL, then pushes each build to itch.io with butler.
/// Menu: Build > Build and Upload to itch.io
///       Build > Upload All to itch.io (existing builds) pushes whatever is already in Builds/itch.
///       Build > Mac / Windows / Web does the same for a single platform.
/// </summary>
public static class ItchPublisher
{
    const string ItchTarget = "imnotjahan/jress-revitalized";
    const string OutputRoot = "Builds/itch";
    const string ButlerPathPref = "ItchPublisher.ButlerPath";

    struct Platform
    {
        public string Channel;
        public BuildTarget Target;
        public BuildTargetGroup Group;
        public string ExecutableName; // relative to the channel folder; empty for WebGL
    }

    static Platform[] Platforms => new[]
    {
        new Platform { Channel = "mac", Target = BuildTarget.StandaloneOSX, Group = BuildTargetGroup.Standalone,
            ExecutableName = PlayerSettings.productName + ".app" },
        new Platform { Channel = "windows", Target = BuildTarget.StandaloneWindows64, Group = BuildTargetGroup.Standalone,
            ExecutableName = PlayerSettings.productName + ".exe" },
        new Platform { Channel = "html5", Target = BuildTarget.WebGL, Group = BuildTargetGroup.WebGL,
            ExecutableName = "" },
    };

    // ---- All platforms ----

    [MenuItem("Build/Build and Upload to itch.io", priority = 0)]
    static void BuildAndUpload() => Run(upload: true);

    [MenuItem("Build/Build All (no upload)", priority = 1)]
    static void BuildOnly() => Run(upload: false);

    [MenuItem("Build/Upload All to itch.io (existing builds)", priority = 2)]
    static void UploadAll() => Upload();

    // ---- Single platform ----

    [MenuItem("Build/Mac/Build and Upload", priority = 20)]
    static void MacBuildAndUpload() => Run(upload: true, "mac");

    [MenuItem("Build/Mac/Build (no upload)", priority = 21)]
    static void MacBuild() => Run(upload: false, "mac");

    [MenuItem("Build/Mac/Upload Existing Build", priority = 22)]
    static void MacUpload() => Upload("mac");

    [MenuItem("Build/Windows/Build and Upload", priority = 23)]
    static void WindowsBuildAndUpload() => Run(upload: true, "windows");

    [MenuItem("Build/Windows/Build (no upload)", priority = 24)]
    static void WindowsBuild() => Run(upload: false, "windows");

    [MenuItem("Build/Windows/Upload Existing Build", priority = 25)]
    static void WindowsUpload() => Upload("windows");

    [MenuItem("Build/Web/Build and Upload", priority = 26)]
    static void WebBuildAndUpload() => Run(upload: true, "html5");

    [MenuItem("Build/Web/Build (no upload)", priority = 27)]
    static void WebBuild() => Run(upload: false, "html5");

    [MenuItem("Build/Web/Upload Existing Build", priority = 28)]
    static void WebUpload() => Upload("html5");

    /// <summary>The platforms with the given channel names, or all of them if none are given.</summary>
    static List<Platform> PlatformsFor(string[] channels) =>
        Platforms.Where(p => channels.Length == 0 || channels.Contains(p.Channel)).ToList();

    static string ChannelDir(Platform platform) => Path.GetFullPath(Path.Combine(OutputRoot, platform.Channel));

    static string FindButlerOrWarn()
    {
        string butler = FindButler();
        if (butler == null)
            EditorUtility.DisplayDialog("butler not found",
                "Couldn't find the butler executable. Use Build > Set butler Path... to locate it.", "OK");
        return butler;
    }

    /// <summary>Pushes builds already in Builds/itch without rebuilding.</summary>
    static void Upload(params string[] channels)
    {
        string butler = FindButlerOrWarn();
        if (butler == null)
            return;

        var builds = PlatformsFor(channels).Select(p => (platform: p, dir: ChannelDir(p))).ToList();

        var missing = builds
            .Where(b => !Directory.Exists(b.dir) || !Directory.EnumerateFileSystemEntries(b.dir).Any())
            .Select(b => b.platform.Channel)
            .ToList();
        if (missing.Count > 0)
        {
            EditorUtility.DisplayDialog("Missing builds",
                $"No build found for: {string.Join(", ", missing)}.\nBuild it first. Nothing was uploaded.", "OK");
            return;
        }

        StartPush(butler, builds);
    }

    [MenuItem("Build/Set butler Path...", priority = 40)]
    static void SetButlerPath()
    {
        string path = EditorUtility.OpenFilePanel("Locate butler executable", "", "");
        if (!string.IsNullOrEmpty(path))
        {
            EditorPrefs.SetString(ButlerPathPref, path);
            Debug.Log($"[itch] butler path set to {path}");
        }
    }

    /// <summary>Builds the given channels (all if none are given), then optionally pushes them.</summary>
    static void Run(bool upload, params string[] channels)
    {
        string butler = null;
        if (upload)
        {
            butler = FindButlerOrWarn();
            if (butler == null)
                return;
        }

        if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("No scenes", "No enabled scenes in Build Settings.", "OK");
            return;
        }

        // Build the currently active target last so we don't leave the editor switched to another platform.
        BuildTarget originalTarget = EditorUserBuildSettings.activeBuildTarget;
        var ordered = PlatformsFor(channels).OrderBy(p => p.Target == originalTarget ? 1 : 0).ToList();

        // itch.io doesn't reliably serve Brotli/Gzip with Content-Encoding headers,
        // so let the WebGL loader decompress in JavaScript.
        bool originalFallback = PlayerSettings.WebGL.decompressionFallback;

        var built = new List<(Platform platform, string dir)>();
        try
        {
            PlayerSettings.WebGL.decompressionFallback = true;

            foreach (var platform in ordered)
            {
                string dir = ChannelDir(platform);
                if (Directory.Exists(dir))
                    Directory.Delete(dir, recursive: true);
                Directory.CreateDirectory(dir);

                var options = new BuildPlayerOptions
                {
                    scenes = scenes,
                    target = platform.Target,
                    targetGroup = platform.Group,
                    locationPathName = string.IsNullOrEmpty(platform.ExecutableName)
                        ? dir
                        : Path.Combine(dir, platform.ExecutableName),
                    options = BuildOptions.None,
                };

                Debug.Log($"[itch] Building {platform.Channel}...");
                BuildReport report = BuildPipeline.BuildPlayer(options);
                if (report.summary.result != BuildResult.Succeeded)
                {
                    EditorUtility.DisplayDialog("Build failed",
                        $"{platform.Channel} build {report.summary.result}. Nothing was uploaded.\nSee the console for details.", "OK");
                    return;
                }

                Debug.Log($"[itch] {platform.Channel} built in {report.summary.totalTime:mm\\:ss} ({report.summary.totalSize / (1024f * 1024f):0.0} MB)");
                built.Add((platform, dir));
            }
        }
        finally
        {
            PlayerSettings.WebGL.decompressionFallback = originalFallback;
        }

        if (!upload)
        {
            EditorUtility.RevealInFinder(Path.GetFullPath(OutputRoot));
            return;
        }

        Debug.Log($"[itch] Built {string.Join(", ", built.Select(b => b.platform.Channel))}.");
        StartPush(butler, built);
    }

    static void StartPush(string butler, List<(Platform platform, string dir)> builds)
    {
        string version = PlayerSettings.bundleVersion;
        Debug.Log($"[itch] Pushing version {version} to {ItchTarget} in the background...");
        Task.Run(() => PushAll(butler, builds, version));
    }

    static void PushAll(string butler, List<(Platform platform, string dir)> builds, string version)
    {
        int failures = 0;
        foreach (var (platform, dir) in builds)
        {
            string target = $"{ItchTarget}:{platform.Channel}";
            var (exitCode, output) = RunProcess(butler,
                $"push \"{dir}\" {target} --userversion \"{version}\"");

            if (exitCode == 0)
                Debug.Log($"[itch] Pushed {target}\n{output}");
            else
            {
                failures++;
                Debug.LogError($"[itch] butler push to {target} failed (exit {exitCode})\n{output}");
            }
        }

        if (failures == 0)
            Debug.Log($"[itch] Upload complete: https://{ItchTarget.Split('/')[0]}.itch.io/{ItchTarget.Split('/')[1]}");
    }

    static (int exitCode, string output) RunProcess(string file, string args)
    {
        var psi = new ProcessStartInfo(file, args)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        try
        {
            using var process = Process.Start(psi);
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            return (process.ExitCode, (stdout.Result + stderr.Result).Trim());
        }
        catch (Exception e)
        {
            return (-1, e.ToString());
        }
    }

    static string FindButler()
    {
        string pref = EditorPrefs.GetString(ButlerPathPref, "");
        if (File.Exists(pref))
            return pref;

        string exe = Application.platform == RuntimePlatform.WindowsEditor ? "butler.exe" : "butler";
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var candidates = new List<string>
        {
            Path.Combine(home, "bin", exe),
            "/usr/local/bin/butler",
            "/opt/homebrew/bin/butler",
        };

        // Unity launched from the Hub doesn't get the shell PATH, but check it anyway.
        string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
        candidates.AddRange(pathEnv.Split(Path.PathSeparator)
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(p => Path.Combine(p, exe)));

        // butler bundled with the itch.io desktop app.
        string brothRoot = Application.platform == RuntimePlatform.OSXEditor
            ? Path.Combine(home, "Library/Application Support/itch/broth/butler/versions")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itch/broth/butler/versions");
        if (Directory.Exists(brothRoot))
            candidates.AddRange(Directory.GetDirectories(brothRoot)
                .OrderByDescending(d => d)
                .Select(d => Path.Combine(d, exe)));

        return candidates.FirstOrDefault(File.Exists);
    }
}
