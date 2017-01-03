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

let runTestScript script =
    let dir = Path.GetDirectoryName script
    let template = Path.GetFileName dir
    printfn "Running tests for the %s template" template
    let (ranOkay, output) = Fake.FSIHelper.executeFSI dir script []
    output |> Seq.iter (fun x -> System.Console.WriteLine x.Message)
    let errors = output |> Seq.exists (fun x -> x.IsError)
    if not ranOkay then
        failwithf "FSI returned an error code after running the %s tests." template
    if errors then
        failwithf "FSI generated error messages while running the %s tests." template
    printfn "Successfully ran tests for the %s template" template

let runBadScript script =
    let f1 = Path.GetDirectoryName >> Path.GetFileName
    let f2 = Path.GetDirectoryName >> f1
    let relPath = (f2 script) @@ (f1 script) @@ (Path.GetFileName script)
    let relPath = relPath.Replace("\\", "/")
    printfn "Running the test script '%s', which should result in errors" relPath
    let dir = Path.GetDirectoryName script
    let template = f2 script
    let (ranOkay, output) = Fake.FSIHelper.executeFSI dir script []
    output |> Seq.iter (fun x -> System.Console.WriteLine x.Message)
    let errors = output |> Seq.exists (fun x -> x.IsError)
    if ranOkay && not errors then
        failwithf "FSI successfully ran a %s test that should have failed." template
    printfn "Test script '%s' finished running with errors, as expected.\n" relPath

Target "RunTests" (fun _ ->
    !! "tests/**/test.fsx"
    |> Seq.iter runTestScript

    !! "tests/**/bad_tests/**.fsx"
    |> Seq.iter runBadScript
)

Target "GitTag" (fun _ ->
    let tag = sprintf "%s" releaseNotes.NugetVersion
    Branches.tag "" tag
)

Target "Default" DoNothing
Target "Release" DoNothing

"RunTests" ==> "Package" ==> "Default"
"Package" ==> "GitTag" ==> "Release"

RunTargetOrDefault "Default"
