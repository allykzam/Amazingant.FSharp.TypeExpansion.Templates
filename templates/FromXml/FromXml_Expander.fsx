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
        /// Returns the appropriate forward-pipe operation required to change an
        /// array to this collection's type, if this value indicates a basic
        /// collection type other than an array
        member x.ToCollection =
            match x with
            | Collection (_, m) when m = "Array" -> ""
            | Collection (_, m) -> sprintf " |> Array.to%s" m
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


    /// Super-long function that builds two strings for a given property. The
    /// first string is valid F# code that can be used to get the property from
    /// an XML node or attribute and perform any parsing that must be done. The
    /// second string is a single line that sets the property to the value
    // constructed by the code contained in the first string.
    let MakeBuilderAndSetter (p : Reflection.PropertyInfo) =
        let pAttr =
            (GetXmlNodeAttribute p)
            |> Seq.map (fun x -> x :> IXmlAttribute)
            |> Seq.append (GetXmlAttrAttribute p |> Seq.map (fun x -> x :> IXmlAttribute))
            |> Seq.tryHead
        let l1 = TypeLevel.ForType p.PropertyType
        let l2 = TypeLevel.ForType l1.InnerType
        let l3 = TypeLevel.ForType l2.InnerType
        let l4 = TypeLevel.ForType l3.InnerType
        let l5 = TypeLevel.ForType l4.InnerType
        match l5 with NormalType _ -> () | _ -> failwithf "Cannot handle types nested more than four layers deep in Option<'T> or any type of collection. Did you mean to create another type to be processed instead?"
        let nAttr = GetXmlNodeAttribute l5.InnerType |> Seq.map (fun x -> x :> IXmlAttribute) |> Seq.tryHead

        let tempName = p.Name.ToLowerInvariant()
        let setter (builder) =
            if String.IsNullOrWhiteSpace builder then
                ""
            else
                sprintf "                    ``%s`` = ``%s``;"
                    p.Name
                    tempName
        let attrStr (x : IXmlAttribute) = match x with :? XmlAttrAttribute -> "Attr" | _ -> ""

        let builder =
            match pAttr, nAttr, l1, l2, l3, l4 with
            // If the property and its type have neither of the XML attributes,
            // and the type is neither optional nor a collection, and the type
            //  happens to be System.String...
            | None, None, NormalType t, _, _, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` = findEither children xmlAttrs \"%s\""
                    tempName
                    tempName

            // If the property and its type have neither of the XML attributes,
            // and the type is optional but not a collection, and the type
            // happens to be System.String...
            | None, None, Option _, NormalType t, _, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` = tryFindEither children xmlAttrs \"%s\""
                    tempName
                    tempName

            // If the property and its type have neither of the XML attributes,
            // and the type is not an option but is a collection, and the type
            // happens to be System.String...
            | None, None, Collection _, NormalType t, _, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` = findAllEither children xmlAttrs \"%s\" |> Seq.toArray%s"
                    tempName
                    tempName
                    l1.ToCollection

            // If the property and its type have neither of the XML attributes,
            // and the type is an optional collection, and the type happens to
            // be System.String...
            | None, None, Option _, Collection _, NormalType t, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tlet xs = findAllEither children xmlAttrs \"%s\" |> Seq.toArray\n\t\t\t\t\tif xs.Length = 0 then None\n\t\t\t\t\telse xs%s |> Some"
                    tempName
                    tempName
                    l2.ToCollection

            // If the property and its type have neither of the XML attributes,
            // and the type is neither optional nor a collection...
            | None, None, NormalType t, _, _, _ ->
                sprintf "\t\t\t\tlet ``%s`` = findEither children xmlAttrs \"%s\" |> (parse %s.TryParse \"%s\")"
                    tempName
                    tempName
                    t.FullName
                    tempName

            // If the property and its type have neither of the XML attributes,
            // and the type is optional, and the type is not a collection...
            | None, None, Option _, NormalType t, _, _ ->
                sprintf "\t\t\t\tlet ``%s`` = tryFindEither children xmlAttrs \"%s\" |> (tryParse %s.TryParse \"%s\")"
                    tempName
                    tempName
                    t.FullName
                    tempName

            // If the property and its type have neither of the XML attributes,
            // and the type is not optional but is a collection...
            | None, None, Collection _, NormalType t, _, _ ->
                sprintf "\t\t\t\tlet ``%s`` = findAllEither children xmlAttrs \"%s\" |> Seq.map (parse %s.TryParse \"%s\") |> Seq.toArray%s"
                    tempName
                    tempName
                    t.FullName
                    tempName
                    l1.ToCollection

            // If the property and its type have neither of the XML attributes,
            // and the type is an optional collection...
            | None, None, Option _, Collection _, NormalType t, _ ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tlet xs = findAllEither children xmlAttrs \"%s\" |> Seq.toArray\n\t\t\t\t\tif xs.Length = 0 then None\n\t\t\t\t\telse xs |> Array.map (parse %s.TryParse \"%s\")%s |> Some"
                    tempName
                    tempName
                    t.FullName
                    tempName
                    l2.ToCollection

            // If the property and its type have neither of the XML attributes,
            // and none of the above cases matched, then the data cannot be
            // processed.
            | None, None, _, _, _, _ ->
                failwithf "Properties with simple types cannot be parsed at levels beyond an optional collection. You will need to either add an attribute somewhere, or simplify your data model."

            // If the property has an XML attribute and its type does not, and
            // the type is neither optional nor a collection, and the type
            // happens to be System.String...
            | Some x, None, NormalType t, _, _, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` = find%s %s \"%s\""
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name

            // If the property has an XML attribute and its type does not, and
            // the type is optional but not a collection, and the type happens
            // to be System.String...
            | Some x, None, Option _, NormalType t, _, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` = tryFind%s %s \"%s\""
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name

            // If the property has an XML attribute and its type does not, and
            // the type is not an option but is a collection, and the type
            // happens to be System.String...
            | Some x, None, Collection _, NormalType t, _, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` = findAll%s %s \"%s\" |> Seq.toArray%s"
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name
                    l1.ToCollection

            // If the property has an XML attribute and its type does not, and
            // the type is an optional collection, and the type happens to be
            // System.String...
            | Some x, None, Option _, Collection _, NormalType t, _ when t = typeof<string> ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tlet xs = findAll%s %s \"%s\" |> Seq.toArray\n\t\t\t\t\tif xs.Length = 0 then None\n\t\t\t\t\telse xs%s |> Some"
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name
                    l2.ToCollection

            // If the property has an XML attribute and its type does not, and
            // the type is neither optional nor a collection...
            | Some x, None, NormalType t, _, _, _ ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tfind%s %s \"%s\"\n\t\t\t\t\t|> (parse %s.%s \"%s\")"
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name
                    t.FullName
                    x.ParseFunction
                    x.Name

            // If the property has an XML attribute and its type does not, and
            // the type is optional, and the type is not a collection...
            | Some x, None, Option _, NormalType t, _, _ ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\ttryFind%s %s \"%s\"\n\t\t\t\t\t|> (tryParse %s.%s \"%s\")"
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name
                    t.FullName
                    x.ParseFunction
                    x.Name

            // If the property has an XML attribute and its type does not, and
            // the type is not optional but is a collection...
            | Some x, None, Collection _, NormalType t, _, _ ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tfindAll%s %s \"%s\"\n\t\t\t\t\t|> Seq.toArray\n\t\t\t\t\t|> Array.map (parse %s.%s \"%s\")%s"
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name
                    t.FullName
                    x.ParseFunction
                    x.Name
                    l1.ToCollection

            // If the property has an XML attribute and its type does not, and
            // the type is an optional collection...
            | Some x, None, Option _, Collection _, NormalType t, _ ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tlet xs = findAll%s %s \"%s\" |> Seq.toArray\n\t\t\t\t\tif xs.Length = 0 then None\n\t\t\t\t\telse xs |> Array.map (parse %s.%s \"%s\")%s |> Some"
                    tempName
                    (attrStr x)
                    x.SourceCollection
                    x.Name
                    t.FullName
                    x.ParseFunction
                    x.Name
                    l2.ToCollection

            // If the property has an XML attribute and its type does not, but
            // its type is a collection of collections, inform the user that
            // this does not work.
            | Some _, None, Collection _, Collection _, _, _ ->
                failwithf "Cannot create a collection of collections of normal types. Did you mean to use a custom type, or perhaps a simple collection?"

            // If the property does not have an XML attribute but its type does,
            // and the type is neither optional nor a collection...
            // OR
            // If the property AND the type have XML attributes, and the
            // property's XML attribute is the Node attribute, and the type is
            // neither optional nor a collection...
            | None, Some x, NormalType t, _, _, _ | Some x, Some _, NormalType t, _, _, _ when (x :? XmlNodeAttribute) ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tfindNode children \"%s\"\n\t\t\t\t\t|> %s.FromXmlNode"
                    tempName
                    x.Name
                    t.FullName

            // If the property does not have an XML attribute but its type does,
            // and the type is optional, and the type is not a collection...
            // OR
            // If the property AND the type have XML attributes, and the
            // property's XML attribute is the Node attribute, and the type is
            // optional, and the type is not a collection...
            | None, Some x, Option _, NormalType t, _, _ | Some x, Some _, Option _, NormalType t, _, _ when (x :? XmlNodeAttribute) ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\ttryFindNode children \"%s\"\n\t\t\t\t\t|> Option.map %s.FromXmlNode"
                    tempName
                    x.Name
                    t.FullName

            // If the property does not have an XML attribute but its type does,
            // and the type is not optional, but the type is a collection...
            // OR
            // If the property AND the type have XML attributes, and the
            // property's XML attribute is the Node attribute, and the type is
            // not optional, but the type is a collection...
            | None, Some x, Collection _, NormalType t, _, _ | Some x, Some _, Collection _, NormalType t, _, _ when (x :? XmlNodeAttribute) ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tfindAllNodes children \"%s\"\n\t\t\t\t\t|> Seq.toArray\n\t\t\t\t\t|> Array.map %s.FromXmlNode%s"
                    tempName
                    x.Name
                    t.FullName
                    l1.ToCollection

            // If the property does not have an XML attribute but its type does,
            // and the type is an optional collection...
            // OR
            // If the property AND the type have XML attributes and the
            // property's XML attribute is the Node attribute, and the type is
            // an optional collection...
            | None, Some x, Option _, Collection _, NormalType t, _ | Some x, Some _, Option _, Collection _, NormalType t, _ when (x :? XmlNodeAttribute) ->
                sprintf "\t\t\t\tlet ``%s`` =\n\t\t\t\t\tlet xs = findAllNodes children \"%s\"\n\t\t\t\t\t|> Seq.toArray\n\t\t\t\t\tif xs.Length = 0 then None\n\t\t\t\t\telse xs |> Array.map %s.FromXmlNode%s"
                    tempName
                    x.Name
                    t.FullName
                    l2.ToCollection

            // If the property does not have an XML attribute but its type does,
            // and the type is a collection of collections, inform the user that
            // this does not work.
            | None, Some _, Collection _, Collection _, _, _ ->
                failwithf "Cannot create a collection of collections of XML types unless the field has an XmlNode attribute on it. Did you mean to supply one?"

            // If the property AND the type have XML attributes but the
            // property's attribute is an XmlAttr attribute, inform the user
            // that the value cannot be parsed thusly.
            | Some x, Some _, _, _, _, _ when (x :? XmlAttrAttribute) ->
                failwithf "Cannot create a nested XML type from an XML attribute. Did you mean to use the XmlNode attribute instead?"

#if TYPE_EXPANSION
            | _ -> failwithf "No handler for this case?"
#endif


        // Return the builder and setter; setter will be empty if the builder is
        let builder = builder.Replace("\t", "    ")
        builder, (setter builder)


    /// Type expansion template for writing FromXml functions for simple types
    [<TypeExpander("FromXml")>]
    let FromXml (t : Type) =
        let isRecordField (p : System.Reflection.PropertyInfo) =
            p.GetCustomAttributes(typeof<CompilationMappingAttribute>, false)
            |> Seq.isEmpty |> not
        let joinLines (x : string seq) = System.String.Join("\n" , x)
        let (builders, setters) =
            t.GetProperties()
            |> Seq.filter isRecordField
            |> Seq.map MakeBuilderAndSetter
            |> Seq.toArray
            |> Array.unzip
            |> fun (builders, setters) ->
                (builders |> joinLines), (setters |> joinLines)
        let mainNodeName = (t.GetCustomAttributes(typeof<XmlNodeAttribute>, false).[0] :?> XmlNodeAttribute).Name
        let fromDoc =
            let nodeName = t.GetCustomAttributes(typeof<XmlNodeAttribute>, false)
            match nodeName with
            | [||] -> ""
            | _ ->
                let nodeName = (nodeName.[0] :?> XmlNodeAttribute).Name
                // This builds functions for loading an array of the target type out
                // of a single XML document, in case the type is stored as a
                // collection in the XML
                sprintf """
            static member FromXmlDoc (doc : XmlDocument) =
                Enumerable.Cast<XmlNode> (doc.GetElementsByTagName("%s"))
                |> Seq.map %s.FromXmlNode
                |> Seq.toArray
            static member FromXmlDoc (xml : string) =
                let doc = XmlDocument()
                doc.LoadXml xml
                %s.FromXmlDoc doc
"""
                    mainNodeName
                    t.Name
                    t.Name

        // Start main result
        sprintf """namespace %s
    [<AutoOpen>]
    module %s_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml
        open System.Linq
        open System.Xml

        type %s with
            static member FromXmlNode (xml : XmlNode) =
                let children = Enumerable.Cast<XmlNode> xml.ChildNodes
                let xmlAttrs = Enumerable.Cast<XmlAttribute> xml.Attributes
%s
                {
%s
                }
%s"""
            t.Namespace
            t.Name
            t.Name
            builders
            setters
            fromDoc
