// ------------------------------------------------------------------------------
// Baby FAKE build script!
// ------------------------------------------------------------------------------

// FAKE
#r "packages/FAKE/tools/FakeLib.dll"

open System
open System.IO
open Fake
open Fake.Git
open Fake.ReleaseNotesHelper

let releaseNotes = LoadReleaseNotes "RELEASE_NOTES.md"

Target "Package" (fun _ ->
    // Delete any existing .nupkg files
    if Directory.Exists("bin") then
        Directory.EnumerateFiles("bin", "*.nupkg", SearchOption.TopDirectoryOnly)
        |> Seq.iter File.Delete

    // Package up the build script
    Paket.Pack (fun p ->
        { p with
              OutputPath = "bin";
                 Version = releaseNotes.NugetVersion;
            ReleaseNotes = releaseNotes.Notes |> toLines;
        })
)

Target "GitTag" (fun _ ->
    let tag = sprintf "%s" releaseNotes.NugetVersion
    Branches.tag "" tag
)

Target "Default" DoNothing
Target "Release" DoNothing

"Package" ==> "Default"
"Package" ==> "GitTag" ==> "Release"

RunTargetOrDefault "Default"
