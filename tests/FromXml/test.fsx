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
    if actual = expected
    then printfn "Expanded code matches expected code"
    else failwithf "Expanded code does not match expected code"

CheckExpansion()

#load "actual_expansion.fsx"

open Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests


let ValidateSample (xmlPath : string) (fromXmlDoc : string -> 'T array) (exp : Map<string, obj>) : unit =
    let data =
        let file = System.IO.Path.Combine(__SOURCE_DIRECTORY__, xmlPath)
        let doc = System.IO.File.ReadAllText file
        fromXmlDoc doc
    if data.Length <> 1
    then failwithf "Could not parse any data from the file '%s'" xmlPath

    let x = data.[0]

    exp |> Map.iter
        (fun property expected ->
            if isNull property then failwithf "Null property name?"
            let prop = typeof<'T>.GetProperty(property)
            if isNull prop then failwithf "Property '%s' does not exist?" property
            let getter = prop.GetGetMethod()
            let actual = getter.Invoke(x, [||])
            match expected, actual with
            | null, null -> ()
            | _, _ ->
                let eT = expected.GetType()
                let aT = actual.GetType()
                if eT <> aT then
                    failwithf "Property '%s' should be of type '%s' but is '%s'"
                        property eT.FullName aT.FullName
                let expected = sprintf "%A" expected
                let actual = sprintf "%A" actual
                if expected <> actual then
                    failwithf "Property '%s' is not valid\nExpected: %s\nGot: %s"
                        property expected actual
        )
    printfn "Successfully validated XML file '%s'" xmlPath


let FullSampleData =
    [
        ("SimpleString"     , box ("simple string"        ));
        ( "MaybeString"     , box ( "maybe string" |> Some));
        (      "StringList" , box ([| "first list element"          ; "second list element"           |] |> Array.toList        ));
        (      "StringArray", box ([| "first array element"         ; "second array element"          |]                        ));
        (      "StringSeq"  , box ([| "first seq element"           ; "second seq element"            |] |> Array.toSeq         ));
        ( "MaybeStringList" , box ([| "first optional list element" ; "second optional list element"  |] |> Array.toList |> Some));
        ( "MaybeStringArray", box ([| "first optional array element"; "second optional array element" |] |>                 Some));
        ( "MaybeStringSeq"  , box ([| "first optional seq element"  ; "second optional seq element"   |] |> Array.toSeq  |> Some));

        ( "SimpleField"     , box (   1       ));
        (  "MaybeField"     , box (   2 |> Some));
        (       "FieldList" , box ([| 3 |] |> Array.toList        ));
        (       "FieldArray", box ([| 4 |]                        ));
        (       "FieldSeq"  , box ([| 5 |] |> Array.toSeq         ));
        (  "MaybeFieldList" , box ([| 6 |] |> Array.toList |> Some));
        (  "MaybeFieldArray", box ([| 7 |]                 |> Some));
        (  "MaybeFieldSeq"  , box ([| 8 |] |> Array.toSeq  |> Some));

        ("SimpleXPathString"     , box         "string"     );
        ( "MaybeXPathString"     , box (Some   "opt string"));
        (      "XPathStringList" , box ([|     "string coll 1";     "string coll 2" |] |> Array.toList        ));
        (      "XPathStringArray", box ([|     "string coll 1";     "string coll 2" |]                        ));
        (      "XPathStringSeq"  , box ([|     "string coll 1";     "string coll 2" |] |> Array.toSeq         ));
        ( "MaybeXPathStringList" , box ([| "opt string coll 1"; "opt string coll 2" |] |> Array.toList |> Some));
        ( "MaybeXPathStringArray", box ([| "opt string coll 1"; "opt string coll 2" |]                 |> Some));
        ( "MaybeXPathStringSeq"  , box ([| "opt string coll 1"; "opt string coll 2" |] |> Array.toSeq  |> Some));

        ("SimpleXPathField"     , box ( 9        ));
        ( "MaybeXPathField"     , box (10 |> Some));
        (      "XPathFieldList" , box ([| 11; 12 |] |> Array.toList        ));
        (      "XPathFieldArray", box ([| 11; 12 |]                        ));
        (      "XPathFieldSeq"  , box ([| 11; 12 |] |> Array.toSeq         ));
        ( "MaybeXPathFieldList" , box ([| 13; 14 |] |> Array.toList |> Some));
        ( "MaybeXPathFieldArray", box ([| 13; 14 |]                 |> Some));
        ( "MaybeXPathFieldSeq"  , box ([| 13; 14 |] |> Array.toSeq  |> Some));

        ("SimpleNodeString"     , box       "string"     );
        ( "MaybeNodeString"     , box (Some "opt string"));
        (      "NodeStringList" , box ([|     "string coll 1";     "string coll 2" |] |> Array.toList        ));
        (      "NodeStringArray", box ([|     "string coll 1";     "string coll 2" |]                        ));
        (      "NodeStringSeq"  , box ([|     "string coll 1";     "string coll 2" |] |> Array.toSeq         ));
        ( "MaybeNodeStringList" , box ([| "opt string coll 1"; "opt string coll 2" |] |> Array.toList |> Some));
        ( "MaybeNodeStringArray", box ([| "opt string coll 1"; "opt string coll 2" |]                 |> Some));
        ( "MaybeNodeStringSeq"  , box ([| "opt string coll 1"; "opt string coll 2" |] |> Array.toSeq  |> Some));

        ("SimpleNodeField"     , box (    9        ));
        ( "MaybeNodeField"     , box (   10 |> Some));
        (      "NodeFieldList" , box ([| 11; 12 |] |> Array.toList        ));
        (      "NodeFieldArray", box ([| 11; 12 |]                        ));
        (      "NodeFieldSeq"  , box ([| 11; 12 |] |> Array.toSeq         ));
        ( "MaybeNodeFieldList" , box ([| 13; 14 |] |> Array.toList |> Some));
        ( "MaybeNodeFieldArray", box ([| 13; 14 |]                 |> Some));
        ( "MaybeNodeFieldSeq"  , box ([| 13; 14 |] |> Array.toSeq  |> Some));

        ("SimpleAttrString"     , box       "string"     );
        ( "MaybeAttrString"     , box (Some "opt string"));
        (      "AttrStringList" , box ([|     "string coll 1" |] |> Array.toList        ));
        (      "AttrStringArray", box ([|     "string coll 1" |]                        ));
        (      "AttrStringSeq"  , box ([|     "string coll 1" |] |> Array.toSeq         ));
        ( "MaybeAttrStringList" , box ([| "opt string coll 1" |] |> Array.toList |> Some));
        ( "MaybeAttrStringArray", box ([| "opt string coll 1" |]                 |> Some));
        ( "MaybeAttrStringSeq"  , box ([| "opt string coll 1" |] |> Array.toSeq  |> Some));

        ("SimpleAttrField"     , box (   15        ));
        ( "MaybeAttrField"     , box (   16 |> Some));
        (      "AttrFieldList" , box ([| 17 |] |> Array.toList        ));
        (      "AttrFieldArray", box ([| 17 |]                        ));
        (      "AttrFieldSeq"  , box ([| 17 |] |> Array.toSeq         ));
        ( "MaybeAttrFieldList" , box ([| 18 |] |> Array.toList |> Some));
        ( "MaybeAttrFieldArray", box ([| 18 |]                 |> Some));
        ( "MaybeAttrFieldSeq"  , box ([| 18 |] |> Array.toSeq  |> Some));

        ("SimpleNestedField"     , box (   { Node       .Field = "node"            }        ));
        ( "MaybeNestedField"     , box (   { NodeOpt    .Field = "opt node"        } |> Some));
        (      "NestedFieldList" , box ([| { NodeColl   .Field = "node coll 1"     }; { NodeColl.Field = "node coll 2"        } |] |> Array.toList        ));
        (      "NestedFieldArray", box ([| { NodeColl   .Field = "node coll 1"     }; { NodeColl.Field = "node coll 2"        } |]                        ));
        (      "NestedFieldSeq"  , box ([| { NodeColl   .Field = "node coll 1"     }; { NodeColl.Field = "node coll 2"        } |] |> Array.toSeq         ));
        ( "MaybeNestedFieldList" , box ([| { NodeOptColl.Field = "opt node coll 1" }; { NodeOptColl.Field = "opt node coll 2" } |] |> Array.toList |> Some));
        ( "MaybeNestedFieldArray", box ([| { NodeOptColl.Field = "opt node coll 1" }; { NodeOptColl.Field = "opt node coll 2" } |]                 |> Some));
        ( "MaybeNestedFieldSeq"  , box ([| { NodeOptColl.Field = "opt node coll 1" }; { NodeOptColl.Field = "opt node coll 2" } |] |> Array.toSeq  |> Some));

        ("SimpleMultiAttrField"     , box (   { Node.Field =     "onode"        }        ));
        ( "MaybeMultiAttrField"     , box (   { Node.Field = "opt onode"        } |> Some));
        (      "MultiAttrFieldList" , box ([| { Node.Field =     "onode coll 1" }; { Node.Field = "onode coll 2"     } |] |> Array.toList        ));
        (      "MultiAttrFieldArray", box ([| { Node.Field =     "onode coll 1" }; { Node.Field = "onode coll 2"     } |]                        ));
        (      "MultiAttrFieldSeq"  , box ([| { Node.Field =     "onode coll 1" }; { Node.Field = "onode coll 2"     } |] |> Array.toSeq         ));
        ( "MaybeMultiAttrFieldList" , box ([| { Node.Field = "opt onode coll 1" }; { Node.Field = "opt onode coll 2" } |] |> Array.toList |> Some));
        ( "MaybeMultiAttrFieldArray", box ([| { Node.Field = "opt onode coll 1" }; { Node.Field = "opt onode coll 2" } |]                 |> Some));
        ( "MaybeMultiAttrFieldSeq"  , box ([| { Node.Field = "opt onode coll 1" }; { Node.Field = "opt onode coll 2" } |] |> Array.toSeq  |> Some));

        ("SimpleNestedXPathField"     , box (   { Node.Field =     "onode"        }        ));
        ( "MaybeNestedXPathField"     , box (   { Node.Field = "opt onode"        } |> Some));
        (      "NestedXPathFieldList" , box ([| { Node.Field =     "onode coll 1" }; { Node.Field = "onode coll 2"     } |] |> Array.toList        ));
        (      "NestedXPathFieldArray", box ([| { Node.Field =     "onode coll 1" }; { Node.Field = "onode coll 2"     } |]                        ));
        (      "NestedXPathFieldSeq"  , box ([| { Node.Field =     "onode coll 1" }; { Node.Field = "onode coll 2"     } |] |> Array.toSeq         ));
        ( "MaybeNestedXPathFieldList" , box ([| { Node.Field = "opt onode coll 1" }; { Node.Field = "opt onode coll 2" } |] |> Array.toList |> Some));
        ( "MaybeNestedXPathFieldArray", box ([| { Node.Field = "opt onode coll 1" }; { Node.Field = "opt onode coll 2" } |]                 |> Some));
        ( "MaybeNestedXPathFieldSeq"  , box ([| { Node.Field = "opt onode coll 1" }; { Node.Field = "opt onode coll 2" } |] |> Array.toSeq  |> Some));

        ("SimpleXPathNestedXPathField"     , box (   { Path.Field =     "onode"        }        ));
        ( "MaybeXPathNestedXPathField"     , box (   { Path.Field = "opt onode"        } |> Some));
        (      "XPathNestedXPathFieldList" , box ([| { Path.Field =     "onode coll 1" }; { Path.Field = "onode coll 2"     } |] |> Array.toList        ));
        (      "XPathNestedXPathFieldArray", box ([| { Path.Field =     "onode coll 1" }; { Path.Field = "onode coll 2"     } |]                        ));
        (      "XPathNestedXPathFieldSeq"  , box ([| { Path.Field =     "onode coll 1" }; { Path.Field = "onode coll 2"     } |] |> Array.toSeq         ));
        ( "MaybeXPathNestedXPathFieldList" , box ([| { Path.Field = "opt onode coll 1" }; { Path.Field = "opt onode coll 2" } |] |> Array.toList |> Some));
        ( "MaybeXPathNestedXPathFieldArray", box ([| { Path.Field = "opt onode coll 1" }; { Path.Field = "opt onode coll 2" } |]                 |> Some));
        ( "MaybeXPathNestedXPathFieldSeq"  , box ([| { Path.Field = "opt onode coll 1" }; { Path.Field = "opt onode coll 2" } |] |> Array.toSeq  |> Some));
    ]
    |> Map.ofList


let ValidateFullSample() = ValidateSample "sample_data/full_sample.xml" TestFields.FromXmlDoc FullSampleData

ValidateFullSample()


let PartialSampleData =
    FullSampleData
    |> Map.add "MaybeString"                      (box (  None : (string            option)                 ))
    |> Map.add       "StringList"                 (box (([| |] : (string      array       )) |> Array.toList))
    |> Map.add       "StringArray"                (box (([| |] : (string      array       ))                ))
    |> Map.add       "StringSeq"                  (box (([| |] : (string      array       )) |> Array.toSeq ))
    |> Map.add  "MaybeStringList"                 (box (  None : (string      list  option)                 ))
    |> Map.add  "MaybeStringArray"                (box (  None : (string      array option)                 ))
    |> Map.add  "MaybeStringSeq"                  (box (  None : (string      seq   option)                 ))

    |> Map.add   "MaybeField"                     (box (  None : (int               option)                 ))
    |> Map.add        "FieldList"                 (box (([| |] : (int         array       )) |> Array.toList))
    |> Map.add        "FieldArray"                (box (([| |] : (int         array       ))                ))
    |> Map.add        "FieldSeq"                  (box (([| |] : (int         array       )) |> Array.toSeq ))
    |> Map.add   "MaybeFieldList"                 (box (  None : (int         list  option)                 ))
    |> Map.add   "MaybeFieldArray"                (box (  None : (int         array option)                 ))
    |> Map.add   "MaybeFieldSeq"                  (box (  None : (int         seq   option)                 ))

    |> Map.add  "MaybeXPathString"                (box (  None : (string            option)                 ))
    |> Map.add       "XPathStringList"            (box (([| |] : (string      array       )) |> Array.toList))
    |> Map.add       "XPathStringArray"           (box (([| |] : (string      array       ))                ))
    |> Map.add       "XPathStringSeq"             (box (([| |] : (string      array       )) |> Array.toSeq ))
    |> Map.add  "MaybeXPathStringList"            (box (  None : (string      list  option)                 ))
    |> Map.add  "MaybeXPathStringArray"           (box (  None : (string      array option)                 ))
    |> Map.add  "MaybeXPathStringSeq"             (box (  None : (string      seq   option)                 ))

    |> Map.add  "MaybeXPathField"                 (box (  None : (int               option)                 ))
    |> Map.add       "XPathFieldList"             (box (([| |] : (int         array       )) |> Array.toList))
    |> Map.add       "XPathFieldArray"            (box (([| |] : (int         array       ))                ))
    |> Map.add       "XPathFieldSeq"              (box (([| |] : (int         array       )) |> Array.toSeq ))
    |> Map.add  "MaybeXPathFieldList"             (box (  None : (int         list  option)                 ))
    |> Map.add  "MaybeXPathFieldArray"            (box (  None : (int         array option)                 ))
    |> Map.add  "MaybeXPathFieldSeq"              (box (  None : (int         seq   option)                 ))

    |> Map.add  "MaybeNodeString"                 (box (  None : (string            option)                 ))
    |> Map.add       "NodeStringList"             (box (([| |] : (string      array       )) |> Array.toList))
    |> Map.add       "NodeStringArray"            (box (([| |] : (string      array       ))                ))
    |> Map.add       "NodeStringSeq"              (box (([| |] : (string      array       )) |> Array.toSeq ))
    |> Map.add  "MaybeNodeStringList"             (box (  None : (string      list  option)                 ))
    |> Map.add  "MaybeNodeStringArray"            (box (  None : (string      array option)                 ))
    |> Map.add  "MaybeNodeStringSeq"              (box (  None : (string      seq   option)                 ))

    |> Map.add  "MaybeNodeField"                  (box (  None : (int               option)                 ))
    |> Map.add       "NodeFieldList"              (box (([| |] : (int         array       )) |> Array.toList))
    |> Map.add       "NodeFieldArray"             (box (([| |] : (int         array       ))                ))
    |> Map.add       "NodeFieldSeq"               (box (([| |] : (int         array       )) |> Array.toSeq ))
    |> Map.add  "MaybeNodeFieldList"              (box (  None : (int         list  option)                 ))
    |> Map.add  "MaybeNodeFieldArray"             (box (  None : (int         array option)                 ))
    |> Map.add  "MaybeNodeFieldSeq"               (box (  None : (int         seq   option)                 ))

    |> Map.add  "MaybeAttrString"                 (box (  None : (string            option)                 ))
    |> Map.add       "AttrStringList"             (box (([| |] : (string      array       )) |> Array.toList))
    |> Map.add       "AttrStringArray"            (box (([| |] : (string      array       ))                ))
    |> Map.add       "AttrStringSeq"              (box (([| |] : (string      array       )) |> Array.toSeq ))
    |> Map.add  "MaybeAttrStringList"             (box (  None : (string      list  option)                 ))
    |> Map.add  "MaybeAttrStringArray"            (box (  None : (string      array option)                 ))
    |> Map.add  "MaybeAttrStringSeq"              (box (  None : (string      seq   option)                 ))

    |> Map.add  "MaybeAttrField"                  (box (  None : (int               option)                 ))
    |> Map.add       "AttrFieldList"              (box (([| |] : (int         array       )) |> Array.toList))
    |> Map.add       "AttrFieldArray"             (box (([| |] : (int         array       ))                ))
    |> Map.add       "AttrFieldSeq"               (box (([| |] : (int         array       )) |> Array.toSeq ))
    |> Map.add  "MaybeAttrFieldList"              (box (  None : (int         list  option)                 ))
    |> Map.add  "MaybeAttrFieldArray"             (box (  None : (int         array option)                 ))
    |> Map.add  "MaybeAttrFieldSeq"               (box (  None : (int         seq   option)                 ))

    |> Map.add  "MaybeNestedField"                (box (  None : (NodeOpt           option)                 ))
    |> Map.add       "NestedFieldList"            (box (([| |] : (NodeColl    array       )) |> Array.toList))
    |> Map.add       "NestedFieldArray"           (box (([| |] : (NodeColl    array       ))                ))
    |> Map.add       "NestedFieldSeq"             (box (([| |] : (NodeColl    array       )) |> Array.toSeq ))
    |> Map.add  "MaybeNestedFieldList"            (box (  None : (NodeOptColl list  option)                 ))
    |> Map.add  "MaybeNestedFieldArray"           (box (  None : (NodeOptColl array option)                 ))
    |> Map.add  "MaybeNestedFieldSeq"             (box (  None : (NodeOptColl seq   option)                 ))

    |> Map.add  "MaybeMultiAttrField"             (box (  None : (Node              option)                 ))
    |> Map.add       "MultiAttrFieldList"         (box (([| |] : (Node        array       )) |> Array.toList))
    |> Map.add       "MultiAttrFieldArray"        (box (([| |] : (Node        array       ))                ))
    |> Map.add       "MultiAttrFieldSeq"          (box (([| |] : (Node        array       )) |> Array.toSeq ))
    |> Map.add  "MaybeMultiAttrFieldList"         (box (  None : (Node        list  option)                 ))
    |> Map.add  "MaybeMultiAttrFieldArray"        (box (  None : (Node        array option)                 ))
    |> Map.add  "MaybeMultiAttrFieldSeq"          (box (  None : (Node        seq   option)                 ))

    |> Map.add  "MaybeXPathNestedField"           (box (  None : (PathOpt           option)                 ))
    |> Map.add       "XPathNestedFieldList"       (box (([| |] : (PathColl    array       )) |> Array.toList))
    |> Map.add       "XPathNestedFieldArray"      (box (([| |] : (PathColl    array       ))                ))
    |> Map.add       "XPathNestedFieldSeq"        (box (([| |] : (PathColl    array       )) |> Array.toSeq ))
    |> Map.add  "MaybeXPathNestedFieldList"       (box (  None : (PathOptColl list  option)                 ))
    |> Map.add  "MaybeXPathNestedFieldArray"      (box (  None : (PathOptColl array option)                 ))
    |> Map.add  "MaybeXPathNestedFieldSeq"        (box (  None : (PathOptColl seq   option)                 ))

    |> Map.add  "MaybeNestedXPathField"           (box (  None : (Node              option)                 ))
    |> Map.add       "NestedXPathFieldList"       (box (([| |] : (Node        array       )) |> Array.toList))
    |> Map.add       "NestedXPathFieldArray"      (box (([| |] : (Node        array       ))                ))
    |> Map.add       "NestedXPathFieldSeq"        (box (([| |] : (Node        array       )) |> Array.toSeq ))
    |> Map.add  "MaybeNestedXPathFieldList"       (box (  None : (Node        list  option)                 ))
    |> Map.add  "MaybeNestedXPathFieldArray"      (box (  None : (Node        array option)                 ))
    |> Map.add  "MaybeNestedXPathFieldSeq"        (box (  None : (Node        seq   option)                 ))

    |> Map.add  "MaybeXPathNestedXPathField"      (box (  None : (Path              option)                 ))
    |> Map.add       "XPathNestedXPathFieldList"  (box (([| |] : (Path        array       )) |> Array.toList))
    |> Map.add       "XPathNestedXPathFieldArray" (box (([| |] : (Path        array       ))                ))
    |> Map.add       "XPathNestedXPathFieldSeq"   (box (([| |] : (Path        array       )) |> Array.toSeq ))
    |> Map.add  "MaybeXPathNestedXPathFieldList"  (box (  None : (Path        list  option)                 ))
    |> Map.add  "MaybeXPathNestedXPathFieldArray" (box (  None : (Path        array option)                 ))
    |> Map.add  "MaybeXPathNestedXPathFieldSeq"   (box (  None : (Path        seq   option)                 ))

let ValidatePartialSample() =
    PartialSampleData
    |> ValidateSample "sample_data/partial_sample.xml" TestFields.FromXmlDoc

ValidatePartialSample()


let ValidateEmptyNodesSample() =
    PartialSampleData
    |> Map.add "MaybeString" (box "    ")
    |> ValidateSample "sample_data/empty_nodes_sample.xml" TestFields.FromXmlDoc

ValidateEmptyNodesSample()
