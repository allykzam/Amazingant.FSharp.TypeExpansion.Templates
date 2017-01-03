#if INTERACTIVE
// Load the TypeExpansion.Attributes library from one of these two relative
// paths in fsi and Visual Studio's script editor
#I "../../packages/Amazingant.FSharp.TypeExpansion/lib/net45"
#I "../../../Amazingant.FSharp.TypeExpansion/lib/net45"
#r "Amazingant.FSharp.TypeExpansion.Attributes.dll"

// Load the attributes file for the XML attributes
#load "FromXml_Base.fsx"
#endif

namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml

open System
open Amazingant.FSharp.TypeExpansion.Attributes


/// Contains type expansion code for the FromXml functions
module Expander =
    /// <summary>
    /// Helper type for processing types of Option<'T> or basic collection types
    /// </summary>
    /// <remarks>
    /// This type considers arrays, the
    /// <see cref="System.Collections.Generic.IEnumerable<'T>" /> (i.e. `'T seq`
    /// in F# code) and F#'s `list` type to be collections. No other collection
    /// types are processed as a collection.
    /// </remarks>
    type TypeLevel =
        /// Indicates that the given type was not a basic collection nor an
        /// Option<'T>
        | NormalType of Type
        /// Indicates that the given type is a basic collection
        | Collection of InnerType : Type * Module : string
        /// Indicates that the given type is an Option<'T>
        | Option of InnerType : Type
        /// Gets the inner type from this value
        member x.InnerType = match x with | NormalType x | Collection (x,_) | Option x -> x
        /// Returns the name of the collection type, if this type level is a
        /// collection.
        member x.CollectionTypeName =
            match x with
            | Collection (_, m) -> m
            | _ -> ""
        /// Generates an instance of this type from the given type information
        static member ForType (t : Type) =
            if t.IsArray then
                let t = t.GetMethod("Get").ReturnType
                Collection (t, "Array")
            elif not t.IsConstructedGenericType then
                NormalType t
            else
                let optType = typeof<int option>.GetGenericTypeDefinition()
                let   seqType = typeof<int  seq>.GetGenericTypeDefinition()
                let  listType = typeof<int list>.GetGenericTypeDefinition()
                let gt = t.GetGenericTypeDefinition()
                let ta = t.GetGenericArguments().[0]
                if gt = seqType then
                    Collection (ta, "Seq")
                elif gt = listType then
                    Collection (ta, "List")
                elif gt = optType then
                    Option ta
                else
                    NormalType t


    /// Gets any XmlAttrAttribute values attached to the given reflection
    /// information
    let inline GetXmlAttrAttribute< ^T when ^T : (member GetCustomAttributes : Type -> bool -> obj [])> (r : ^T) =
        (^T : (member GetCustomAttributes : Type -> bool -> obj []) (r, typeof<XmlAttrAttribute>, false))
        |> Array.map (fun x -> x :?> XmlAttrAttribute)

    /// Gets any XmlNodeAttribute values attached to the given reflection
    /// information
    let inline GetXmlNodeAttribute< ^T when ^T : (member GetCustomAttributes : Type -> bool -> obj [])> (r : ^T) =
        (^T : (member GetCustomAttributes : Type -> bool -> obj []) (r, typeof<XmlNodeAttribute>, false))
        |> Array.map (fun x -> x :?> XmlNodeAttribute)

    let inline GetXPathAttribute< ^T when ^T : (member GetCustomAttributes : Type -> bool -> obj [])> (r : ^T) =
        (^T : (member GetCustomAttributes : Type -> bool -> obj []) (r, typeof<XPathAttribute>, false))
        |> Array.map (fun x -> x :?> XPathAttribute)


    /// This function builds a single line of F# code as a string for a given
    /// property. The code contained in the returned string will fetch the
    /// property value from an XML node, attribute, or specific XPath, and
    /// perform any parsing that must be done.
    let MakeSetter (p : Reflection.PropertyInfo) =
        let pAttr =
            (GetXmlNodeAttribute p)
            |> Seq.map (fun x -> x :> IXmlAttribute)
            |> Seq.append (GetXmlAttrAttribute p |> Seq.map (fun x -> x :> IXmlAttribute))
            |> Seq.append (GetXPathAttribute   p |> Seq.map (fun x -> x :> IXmlAttribute))
            |> Seq.tryHead
        let l1 = TypeLevel.ForType p.PropertyType
        let l2 = TypeLevel.ForType l1.InnerType
        let l3 = TypeLevel.ForType l2.InnerType
        let inline superNestedError () =
            failwithf "Cannot handle types nested more than two layers deep in Option<'T> or any type of collection; property '%s' cannot be processed. Did you mean to create another type to be processed instead?"
                p.Name
        match l3 with NormalType _ -> () | _ -> superNestedError()
        let nAttr =
            GetXmlNodeAttribute l3.InnerType
            |> Seq.map (fun x -> x :> IXmlAttribute)
            |> Seq.append (GetXPathAttribute l3.InnerType |> Seq.map (fun x -> x :> IXmlAttribute))
            |> Seq.tryHead

        let setter =
            let parser =
                match nAttr with
                | Some _ -> sprintf "%s.FromXmlNode" l3.InnerType.FullName
                | None ->
                    if l3.InnerType  = typeof<string>
                    then "getInnerText"
                    else
                        match pAttr with
                        | Some x -> sprintf "(parserForStrings \"%s\" %s.%s)" l3.InnerType.FullName l3.InnerType.FullName x.ParseFunction
                        | None -> sprintf "(parserForStrings \"%s\" %s.TryParse)" l3.InnerType.FullName l3.InnerType.FullName
            let getter =
                match pAttr, nAttr with
                | Some x, _ | None, Some x when (x :? XPathAttribute) -> "fromAnXPath"
                | Some x, _ | None, Some x when (x :? XmlNodeAttribute) -> "fromXmlTags"
                | Some x, None when (x :? XmlAttrAttribute) -> "fromAttributes"
                | _ -> "fromTagsOrAttributes"
            let retType =
                match l1, l2, l3 with
                | NormalType t, _, _ -> sprintf "exactlyOne"
                | Option _, NormalType _, _ -> "maybeOne"
                | Collection _, NormalType _, _ -> sprintf "get%s" l1.CollectionTypeName
                | Option _, Collection _, NormalType _ -> sprintf "getMaybe%s" l2.CollectionTypeName
                // Bad input cases
                | _, Option _, _ ->
                    failwithf "Collections of optional values are not supported; property '%s' cannot be processed."
                        p.Name
                | _, _, Collection _ | _, _, Option _ -> superNestedError()
                | Collection _, Collection _, _ ->
                    failwithf "Collections of collections are not supported; property '%s' cannot be processed."
                        p.Name
            let nodeNameOrXPath =
                match pAttr, nAttr with
                | None, None -> p.Name
                | Some x, _ | None, Some x -> x.Name

            sprintf "\t\t\t\t\t``%s`` = (%s %s xml \"%s\" %s);"
                p.Name
                retType
                getter
                nodeNameOrXPath
                parser

        setter.Replace("\t", "    ").TrimEnd()


    /// Type expansion template for writing FromXml functions for simple types
    [<TypeExpander("FromXml")>]
    let FromXml (t : Type) =
        let isRecordField (p : System.Reflection.PropertyInfo) =
            p.GetCustomAttributes(typeof<CompilationMappingAttribute>, false)
            |> Seq.isEmpty |> not
        let joinLines (x : string seq) = System.String.Join("\n" , x)
        let setters =
            t.GetProperties()
            |> Seq.filter isRecordField
            |> Seq.map MakeSetter
            |> joinLines
        let mainNodeName =
            let nodeAttr = (t.GetCustomAttributes(typeof<XmlNodeAttribute>, false)) |> Array.toList
            let pathAttr = (t.GetCustomAttributes(typeof<XPathAttribute>, false)) |> Array.toList
            match nodeAttr, pathAttr with
            | [], [] -> failwithf "The %s type is flagged for expansion by the FromXml expander, but does not have the XmlNode or XPath attribute." t.FullName
            | [x], [] | [], [x] -> (x :?> IXmlAttribute).Name
            | _::_, _::_ -> failwithf "The %s type has both the XmlNode and XPath attributes; the FromXml expander can only process one or the other." t.FullName
            | [], _::_ -> failwithf "The %s type has multiple XPath attributes." t.FullName
            | _::_, [] -> failwithf "The %s type has multiple XmlNode attributes." t.FullName
        let fromDoc =
            let nodeName = t.GetCustomAttributes(typeof<XmlNodeAttribute>, false)
            let pathName = t.GetCustomAttributes(typeof<XPathAttribute>, false)
            match nodeName, pathName with
            | [||], [||] -> ""
            | _, [||] ->
                let nodeName = (nodeName.[0] :?> XmlNodeAttribute).Name
                // This builds functions for loading an array of the target type
                // out of a single XML document, in case the type is stored as a
                // collection in the XML
                sprintf """
            static member FromXmlDoc doc = thingFromDocElement doc "%s" %s.FromXmlNode
            static member FromXmlDoc xml = thingFromStringElement xml "%s" %s.FromXmlNode
"""
                    mainNodeName
                    t.Name
                    mainNodeName
                    t.Name

            | [||], _ ->
                let pathName = (pathName.[0] :?> XPathAttribute).Path
                // This builds functions for loading an array of the target type
                // out of a single XML document, in case the type is stored as a
                // collection in the XML
                sprintf """
            static member FromXmlDoc doc = thingFromDocXPath doc "%s" %s.FromXmlNode
            static member FromXmlDoc xml = thingFromStringXPath xml "%s" %s.FromXmlNode
"""
                    pathName
                    t.Name
                    pathName
                    t.Name
            // This case should never match, as it should have been caught above
            // when binding the `mainNodeName` value
            | _, _ -> failwithf "The %s type either has both XmlNode and XPath attributes, multiples of one attribute, or some combination thereof." t.FullName

        // Start main result
        sprintf """namespace %s
    [<AutoOpen>]
    module %s_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type %s with
            static member FromXmlNode (xml : System.Xml.XmlNode) : %s =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a '%s' value from it"
                {
%s
                }
%s"""
            t.Namespace
            t.Name
            t.Name
            t.Name
            t.Name
            setters
            fromDoc
