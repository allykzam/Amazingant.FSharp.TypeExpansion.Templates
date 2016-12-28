#r "../../packages/Amazingant.FSharp.TypeExpansion/lib/net45/Amazingant.FSharp.TypeExpansion.Attributes.dll"
#r "../../packages/Amazingant.FSharp.TypeExpansion/lib/net45/Amazingant.FSharp.TypeExpansion.dll"

[<Literal>]
let SourceFiles = """
../../templates/FromXml/FromXml_Base.fsx
../../templates/FromXml/FromXml_Expander.fsx
source_types.fsx
"""

open Amazingant.FSharp.TypeExpansion

type LoadSourceTypes =
    Expand<
        SourceFiles,
        WorkingDirectory=(__SOURCE_DIRECTORY__),
        References="System,System.Xml,System.Core",
        CompilerFlags="--define:TYPE_EXPANSION",
        OutputPath="actual_expansion.fsx",
        OutputMode=Amazingant.FSharp.TypeExpansion.Provider.OutputMode.CreateSourceFile
        >


let CheckExpansion() =
    let f x =
        System.IO.Path.Combine(__SOURCE_DIRECTORY__, x)
        |> System.IO.File.ReadAllLines
    let actual = f "actual_expansion.fsx"
    let expected = f "expected_expansion.fsx"
    actual = expected
