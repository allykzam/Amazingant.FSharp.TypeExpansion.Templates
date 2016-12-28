
#if INTERACTIVE
#load @"../../templates/FromXml/FromXml_Base.fsx"
#load @"../../templates/FromXml/FromXml_Expander.fsx"
#load @"source_types.fsx"
#endif

(*
 * This file was auto-generated by the TypeExpansion type provider
 *
 * The contents of this file are likely to be overwritten at any time while
 * Visual Studio or F# Interactive is open, or while a build is in progress.
 *
 * Please do not make important changes to this file.
 *)

namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module Node_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml
        open System.Linq
        open System.Xml

        type Node with
            static member FromXmlNode (xml : XmlNode) =
                let children = Enumerable.Cast<XmlNode> xml.ChildNodes
                let xmlAttrs = Enumerable.Cast<XmlAttribute> xml.Attributes
                let ``field`` = findEither children xmlAttrs "field"
                {
                    ``Field`` = ``field``;
                }

            static member FromXmlDoc (doc : XmlDocument) =
                Enumerable.Cast<XmlNode> (doc.GetElementsByTagName("node"))
                |> Seq.map Node.FromXmlNode
                |> Seq.toArray
            static member FromXmlDoc (xml : string) =
                let doc = XmlDocument()
                doc.LoadXml xml
                Node.FromXmlDoc doc


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module NodeOpt_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml
        open System.Linq
        open System.Xml

        type NodeOpt with
            static member FromXmlNode (xml : XmlNode) =
                let children = Enumerable.Cast<XmlNode> xml.ChildNodes
                let xmlAttrs = Enumerable.Cast<XmlAttribute> xml.Attributes
                let ``field`` = findEither children xmlAttrs "field"
                {
                    ``Field`` = ``field``;
                }

            static member FromXmlDoc (doc : XmlDocument) =
                Enumerable.Cast<XmlNode> (doc.GetElementsByTagName("node_opt"))
                |> Seq.map NodeOpt.FromXmlNode
                |> Seq.toArray
            static member FromXmlDoc (xml : string) =
                let doc = XmlDocument()
                doc.LoadXml xml
                NodeOpt.FromXmlDoc doc


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module NodeColl_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml
        open System.Linq
        open System.Xml

        type NodeColl with
            static member FromXmlNode (xml : XmlNode) =
                let children = Enumerable.Cast<XmlNode> xml.ChildNodes
                let xmlAttrs = Enumerable.Cast<XmlAttribute> xml.Attributes
                let ``field`` = findEither children xmlAttrs "field"
                {
                    ``Field`` = ``field``;
                }

            static member FromXmlDoc (doc : XmlDocument) =
                Enumerable.Cast<XmlNode> (doc.GetElementsByTagName("node_coll"))
                |> Seq.map NodeColl.FromXmlNode
                |> Seq.toArray
            static member FromXmlDoc (xml : string) =
                let doc = XmlDocument()
                doc.LoadXml xml
                NodeColl.FromXmlDoc doc


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module NodeOptColl_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml
        open System.Linq
        open System.Xml

        type NodeOptColl with
            static member FromXmlNode (xml : XmlNode) =
                let children = Enumerable.Cast<XmlNode> xml.ChildNodes
                let xmlAttrs = Enumerable.Cast<XmlAttribute> xml.Attributes
                let ``field`` = findEither children xmlAttrs "field"
                {
                    ``Field`` = ``field``;
                }

            static member FromXmlDoc (doc : XmlDocument) =
                Enumerable.Cast<XmlNode> (doc.GetElementsByTagName("node_opt_coll"))
                |> Seq.map NodeOptColl.FromXmlNode
                |> Seq.toArray
            static member FromXmlDoc (xml : string) =
                let doc = XmlDocument()
                doc.LoadXml xml
                NodeOptColl.FromXmlDoc doc


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module TestFields_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml
        open System.Linq
        open System.Xml

        type TestFields with
            static member FromXmlNode (xml : XmlNode) =
                let children = Enumerable.Cast<XmlNode> xml.ChildNodes
                let xmlAttrs = Enumerable.Cast<XmlAttribute> xml.Attributes
                let ``simplestring`` = findEither children xmlAttrs "simplestring"
                let ``maybestring`` = tryFindEither children xmlAttrs "maybestring"
                let ``stringlist`` = findAllEither children xmlAttrs "stringlist" |> Seq.toArray |> Array.toList
                let ``stringarray`` = findAllEither children xmlAttrs "stringarray" |> Seq.toArray
                let ``stringseq`` = findAllEither children xmlAttrs "stringseq" |> Seq.toArray |> Array.toSeq
                let ``maybestringlist`` =
                    let xs = findAllEither children xmlAttrs "maybestringlist" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.toList |> Some
                let ``maybestringarray`` =
                    let xs = findAllEither children xmlAttrs "maybestringarray" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Some
                let ``maybestringseq`` =
                    let xs = findAllEither children xmlAttrs "maybestringseq" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.toSeq |> Some
                let ``simplefield`` = findEither children xmlAttrs "simplefield" |> (parse System.Int32.TryParse "simplefield")
                let ``maybefield`` = tryFindEither children xmlAttrs "maybefield" |> (tryParse System.Int32.TryParse "maybefield")
                let ``fieldlist`` = findAllEither children xmlAttrs "fieldlist" |> Seq.map (parse System.Int32.TryParse "fieldlist") |> Seq.toArray |> Array.toList
                let ``fieldarray`` = findAllEither children xmlAttrs "fieldarray" |> Seq.map (parse System.Int32.TryParse "fieldarray") |> Seq.toArray
                let ``fieldseq`` = findAllEither children xmlAttrs "fieldseq" |> Seq.map (parse System.Int32.TryParse "fieldseq") |> Seq.toArray |> Array.toSeq
                let ``maybefieldlist`` =
                    let xs = findAllEither children xmlAttrs "maybefieldlist" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.map (parse System.Int32.TryParse "maybefieldlist") |> Array.toList |> Some
                let ``maybefieldarray`` =
                    let xs = findAllEither children xmlAttrs "maybefieldarray" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.map (parse System.Int32.TryParse "maybefieldarray") |> Some
                let ``maybefieldseq`` =
                    let xs = findAllEither children xmlAttrs "maybefieldseq" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.map (parse System.Int32.TryParse "maybefieldseq") |> Array.toSeq |> Some
                let ``simplexpathstring`` = xml.SelectSingleNode("string").InnerText
                let ``maybexpathstring`` = xml.SelectSingleNode("string_opt").InnerText |> Option.ofObj
                let ``xpathstringlist`` = xml.SelectNodes("string_coll") |> getInnerTexts |> Seq.toArray |> Array.toList
                let ``xpathstringarray`` = xml.SelectNodes("string_coll") |> getInnerTexts |> Seq.toArray
                let ``xpathstringseq`` = xml.SelectNodes("string_coll") |> getInnerTexts |> Seq.toArray |> Array.toSeq
                let ``maybexpathstringlist`` =
                    let xs = xml.SelectNodes("string_opt_coll") |> getInnerTexts |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.toList |> Some
                let ``maybexpathstringarray`` =
                    let xs = xml.SelectNodes("string_opt_coll") |> getInnerTexts |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Some
                let ``maybexpathstringseq`` =
                    let xs = xml.SelectNodes("string_opt_coll") |> getInnerTexts |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.toSeq |> Some
                let ``simplexpathfield`` =
                    xml.SelectSingleNode("int").InnerText
                    |> (parse System.Int32.TryParse "simplexpathfield")
                let ``maybexpathfield`` =
                    xml.SelectSingleNode("int_opt")
                    |> tryInnerText
                    |> (tryParse System.Int32.TryParse "maybexpathfield")
                let ``xpathfieldlist`` =
                    xml.SelectNodes("int_coll")
                    |> getInnerTexts
                    |> Seq.map (parse System.Int32.TryParse "xpathfieldlist")
                    |> Seq.toArray |> Array.toList
                let ``xpathfieldarray`` =
                    xml.SelectNodes("int_coll")
                    |> getInnerTexts
                    |> Seq.map (parse System.Int32.TryParse "xpathfieldarray")
                    |> Seq.toArray
                let ``xpathfieldseq`` =
                    xml.SelectNodes("int_coll")
                    |> getInnerTexts
                    |> Seq.map (parse System.Int32.TryParse "xpathfieldseq")
                    |> Seq.toArray |> Array.toSeq
                let ``maybexpathfieldlist`` =
                    let xs = xml.SelectNodes("int_opt_coll") |> getInnerTexts |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.map (parse System.Int32.TryParse "maybexpathfieldlist") |> Array.toList |> Some
                let ``maybexpathfieldarray`` =
                    let xs = xml.SelectNodes("int_opt_coll") |> getInnerTexts |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.map (parse System.Int32.TryParse "maybexpathfieldarray") |> Some
                let ``maybexpathfieldseq`` =
                    let xs = xml.SelectNodes("int_opt_coll") |> getInnerTexts |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.map (parse System.Int32.TryParse "maybexpathfieldseq") |> Array.toSeq |> Some
                let ``simplenodestring`` = find children "string"
                let ``maybenodestring`` = tryFind children "string_opt"
                let ``nodestringlist`` = findAll children "string_coll" |> Seq.toArray |> Array.toList
                let ``nodestringarray`` = findAll children "string_coll" |> Seq.toArray
                let ``nodestringseq`` = findAll children "string_coll" |> Seq.toArray |> Array.toSeq
                let ``maybenodestringlist`` =
                    let xs = findAll children "string_opt_coll" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.toList |> Some
                let ``maybenodestringarray`` =
                    let xs = findAll children "string_opt_coll" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Some
                let ``maybenodestringseq`` =
                    let xs = findAll children "string_opt_coll" |> Seq.toArray
                    if xs.Length = 0 then None
                    else xs |> Array.toSeq |> Some
                {
                    ``SimpleString`` = ``simplestring``;
                    ``MaybeString`` = ``maybestring``;
                    ``StringList`` = ``stringlist``;
                    ``StringArray`` = ``stringarray``;
                    ``StringSeq`` = ``stringseq``;
                    ``MaybeStringList`` = ``maybestringlist``;
                    ``MaybeStringArray`` = ``maybestringarray``;
                    ``MaybeStringSeq`` = ``maybestringseq``;
                    ``SimpleField`` = ``simplefield``;
                    ``MaybeField`` = ``maybefield``;
                    ``FieldList`` = ``fieldlist``;
                    ``FieldArray`` = ``fieldarray``;
                    ``FieldSeq`` = ``fieldseq``;
                    ``MaybeFieldList`` = ``maybefieldlist``;
                    ``MaybeFieldArray`` = ``maybefieldarray``;
                    ``MaybeFieldSeq`` = ``maybefieldseq``;
                    ``SimpleXPathString`` = ``simplexpathstring``;
                    ``MaybeXPathString`` = ``maybexpathstring``;
                    ``XPathStringList`` = ``xpathstringlist``;
                    ``XPathStringArray`` = ``xpathstringarray``;
                    ``XPathStringSeq`` = ``xpathstringseq``;
                    ``MaybeXPathStringList`` = ``maybexpathstringlist``;
                    ``MaybeXPathStringArray`` = ``maybexpathstringarray``;
                    ``MaybeXPathStringSeq`` = ``maybexpathstringseq``;
                    ``SimpleXPathField`` = ``simplexpathfield``;
                    ``MaybeXPathField`` = ``maybexpathfield``;
                    ``XPathFieldList`` = ``xpathfieldlist``;
                    ``XPathFieldArray`` = ``xpathfieldarray``;
                    ``XPathFieldSeq`` = ``xpathfieldseq``;
                    ``MaybeXPathFieldList`` = ``maybexpathfieldlist``;
                    ``MaybeXPathFieldArray`` = ``maybexpathfieldarray``;
                    ``MaybeXPathFieldSeq`` = ``maybexpathfieldseq``;
                    ``SimpleNodeString`` = ``simplenodestring``;
                    ``MaybeNodeString`` = ``maybenodestring``;
                    ``NodeStringList`` = ``nodestringlist``;
                    ``NodeStringArray`` = ``nodestringarray``;
                    ``NodeStringSeq`` = ``nodestringseq``;
                    ``MaybeNodeStringList`` = ``maybenodestringlist``;
                    ``MaybeNodeStringArray`` = ``maybenodestringarray``;
                    ``MaybeNodeStringSeq`` = ``maybenodestringseq``;
                }

            static member FromXmlDoc (doc : XmlDocument) =
                Enumerable.Cast<XmlNode> (doc.GetElementsByTagName("Test"))
                |> Seq.map TestFields.FromXmlNode
                |> Seq.toArray
            static member FromXmlDoc (xml : string) =
                let doc = XmlDocument()
                doc.LoadXml xml
                TestFields.FromXmlDoc doc
