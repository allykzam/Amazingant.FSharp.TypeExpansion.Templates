#if INTERACTIVE
#r "System.Xml"
#endif

namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml

open System

/// Represents an XML attribute that contains basic information needed for
/// handling XML processing for a type
type IXmlAttribute =
    /// For XmlNode and XmlAttr attributes, this is the XML node or attribute
    /// name to gather data from. For XPath attributes, this is the XPath to
    /// follow.
    abstract member Name : string
    /// The name of the function to use when processing the gathered data
    abstract member ParseFunction : string
    /// For XmlNode and XmlAttr attributes, indicates which collection of
    /// elements to search through; either `children` or `xmlAttrs`. For XPath
    /// attributes, this value is unused and therefore empty.
    abstract member SourceCollection : string


[<AttributeUsage(
    AttributeTargets.Class    |||
    AttributeTargets.Struct   |||
    AttributeTargets.Field    |||
    AttributeTargets.Property
    )>]
/// <summary>
/// Indicates a type or value that can be set using the contents of an XML node
/// </summary>
/// <param name="name">
/// The name of the XML node to process data from
/// </param>
/// <param name="parseFunc">
/// Then name of a parser function on the target type to use when processing the
/// XML node's InnerText property. The named function must take a string and
/// return a boolean and the target type, where the boolean indicates whether or
/// not the input string was valid.
/// </param>
/// <remarks>
/// The provided name is used in a case-insensitive comparison against XML node
/// names
/// </remarks>
type XmlNodeAttribute (name : string, parseFunc : string) =
    inherit Attribute()

    /// <summary>
    /// Creates a new instance of this attribute.
    /// </summary>
    /// <param name="name">
    /// The name of the XML node to process data from
    /// </param>
    /// <remarks>
    /// The provided name is used in a case-insensitive comparison against XML
    /// node names
    ///
    /// This constructor defaults to using the "TryParse" function on the target
    /// type to process the XML node contents
    /// </remarks>
    new (name) = XmlNodeAttribute(name, "TryParse")

    /// The name of the XML node that will be used
    member __.Name = name
    /// THe name of the function that will be used to process the XML node
    /// contents
    member __.ParseFunction = parseFunc
    interface IXmlAttribute with
        member __.Name = name
        member __.ParseFunction = parseFunc
        member __.SourceCollection = "children"



[<AttributeUsage(
    AttributeTargets.Field    |||
    AttributeTargets.Property
    )>]
/// <summary>
/// Indicates a value that can be set using the contents of an XML attribute
/// </summary>
/// <param name="name">
/// The name of the XML attribute to process data from
/// </param>
/// <param name="parseFunc">
/// The name of a parser function on the target type to use when processing the
/// XML node's InnerText property. The named function must take a string and
/// return a boolean and the target type, where the boolean indicates whether or
/// not the input string was valid.
/// </param>
/// <remarks>
/// The provided name is used in a case-insensitive comparison against XML
/// attribute names
/// </remarks>
type XmlAttrAttribute (name : string, parseFunc : string) =
    inherit Attribute()

    /// <summary>
    /// Creates a new instance of this attribute.
    /// </summary>
    /// <param name="name">
    /// The name of the XML attribute to process data from
    /// </param>
    /// <remarks>
    /// The provided name is used in a case-insensitive comparison against XML
    /// attribute names
    ///
    /// This constructor defaults to using the "TryParse" function on the target
    /// type to process the XML attribute contents
    /// </remarks>
    new (name) = XmlAttrAttribute(name, "TryParse")

    /// The name of the XML attribute that will be used
    member __.Name = name
    /// The name of the function that will be used to process the XML attribute
    /// contents
    member __.ParseFunction = parseFunc
    interface IXmlAttribute with
        member __.Name = name
        member __.ParseFunction = parseFunc
        member __.SourceCollection = "xmlAttrs"



[<AttributeUsage(
    AttributeTargets.Class    |||
    AttributeTargets.Struct   |||
    AttributeTargets.Field    |||
    AttributeTargets.Property
    )>]
/// <summary>
/// Indicates a value that can be set using the contents of a value to be
/// gathered via an XPath
/// </summary>
/// <param name="path">
/// The XPath to gather data from for processing
/// </param>
/// <param name="parseFunc">
/// The name of a parser function on the target type to use when processing the
/// XML node's InnerText property. The named function must take a string and
/// return a boolean and the target type, where the boolean indicates whether or
/// not the input string was valid.
/// </param>
type XPathAttribute (path : string, parseFunc : string) =
    inherit Attribute()

    /// <summary>
    /// Creates a new instance of this attribute.
    /// </summary>
    /// <param name="path">
    /// The XPath to gather data from for processing
    /// </param>
    /// <remarks>
    /// This constructor defaults to using the "TryParse" function on the target
    /// type to process the XML attribute contents
    /// </remarks>
    new (path) = XPathAttribute(path, "TryParse")

    /// The XPath to use when gathering the target data
    member __.Path = path
    /// The name of the function that will be used to process the target data
    member __.ParseFunction = parseFunc
    interface IXmlAttribute with
        member __.Name = path
        member __.ParseFunction = parseFunc
        member __.SourceCollection = ""



[<AttributeUsage(AttributeTargets.Method)>]
/// <summary>
/// Indicates that a function is to be used as validation after processing it
/// from XML.
/// </summary>
type ValidationAttribute() =
    inherit Attribute()


[<RequireQualifiedAccess>]
type ValidationResult =
    | Valid
    | Invalid of Message : string



open System.Xml

[<AutoOpen>]
/// Helpers used for the expanded FromXml code to simplify the processing of
/// XML data
module Helpers =
    /// Uses the given function to process elements whose tag name match the
    /// specified name, and returns the results as an array.
    let inline thingFromDocElement (doc : XmlDocument) (tagName : string) (fromNode : XmlNode -> 'T) : 'T array =
        doc.ChildNodes
        |> System.Linq.Enumerable.Cast
        |> Seq.filter (fun (x : XmlNode) -> x.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase))
        |> Seq.map fromNode
        |> Seq.toArray
    /// Uses the given XML string to create an XML document, then uses the given
    /// function to process elements whose tag name match the specified name,
    /// and returns the results as an array
    let inline thingFromStringElement (xml : string) (tagName : string) (fromNode : XmlNode -> 'T) : 'T array =
        let doc = XmlDocument()
        doc.LoadXml xml
        thingFromDocElement doc tagName fromNode
    /// Uses the given function to process elements found using the given XPath,
    /// and returns the results as an array.
    let inline thingFromDocXPath (doc : XmlDocument) (xpath : string) (fromNode : XmlNode -> 'T) : 'T array =
        doc.SelectNodes xpath
        |> System.Linq.Enumerable.Cast
        |> Seq.map fromNode
        |> Seq.toArray
    /// Uses the given XML string to create an XML document, then uses the given
    /// function to process elements found using the given XPath, and returns
    /// the results as an array.
    let inline thingFromStringXPath (xml : string) (xpath : string) (fromNode : XmlNode -> 'T) : 'T array =
        let doc = XmlDocument()
        doc.LoadXml xml
        thingFromDocXPath doc xpath fromNode

    /// Returns a tuple whose second value is a function which gathers all XML
    /// tags whose names match the given name and uses the given parser to
    /// transform their inner text. The first tuple value is a stringy
    /// definition of what the function fetches values from, for use in error
    /// messages when values cannot be found.
    let fromXmlTags : string * (XmlNode -> string -> (XmlNode -> 'a) -> 'a seq) =
        "tags",
        fun xml tagName parser ->
            xml.ChildNodes
            |> System.Linq.Enumerable.Cast
            |> Seq.filter (fun (x : XmlNode) -> x.Name.Replace("_", "").Equals(tagName.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
            |> Seq.map parser

    /// Returns a tuple whose second value is a function which gathers all XML
    /// attributes whose names match the given name and uses the given parser to
    /// transform their inner text. The first tuple value is a stringy
    /// definition of what the function fetches values from, for use in error
    /// messages when values cannot be found.
    let fromAttributes : string * (XmlNode -> string -> (XmlNode -> 'a) -> 'a seq) =
        "attributes",
        fun xml attrName parser ->
            xml.Attributes
            |> System.Linq.Enumerable.Cast
            |> Seq.filter (fun (x : XmlNode) -> x.Name.Replace("_", "").Equals(attrName.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
            |> Seq.map parser

    /// Returns a tuple whose second value is a function which gathers all XML
    /// tags and attributes whose names match the given name, and uses the given
    /// parser to transform their inner text. The first tuple value is a stringy
    /// definition of what the function fetches values from, for use in error
    /// messages when values cannot be found.
    let fromTagsOrAttributes : string * (XmlNode -> string -> (XmlNode -> 'a) -> 'a seq) =
        "tags or attributes",
        fun xml tagOrAttr parser ->
            let xs = (snd fromXmlTags   ) xml tagOrAttr parser
            let ys = (snd fromAttributes) xml tagOrAttr parser
            Seq.append xs ys

    /// Returns a tuple whose second value is a function which gathers all nodes
    /// captured by the given XPath specifier, and uses the given parser to
    /// transform their inner text. The first tuple value is a stringy
    /// definition of what the function fetches values from, for use in error
    /// messages when values cannot be found.
    let fromAnXPath : string * (XmlNode -> string -> (XmlNode -> 'a) -> 'a seq) =
        "XPath",
        fun xml xpath parser ->
            xml.SelectNodes xpath
            |> System.Linq.Enumerable.Cast
            |> Seq.map parser

    /// Converts a normal TryParse-style function into one which takes an XML
    /// node and returns a result, throwing an exception if the inner text from
    /// the node cannot be parsed.
    let parserForStrings targetType targetField (p : string -> bool * 'a) : XmlNode -> 'a =
        fun x ->
            match x with
            | null -> failwithf "Trying to process a null XmlNode into a '%s' for the field '%s'" targetType targetField
            | _ ->
                match p x.InnerText with
                | (true , x) -> x
                | (false, _) -> failwithf "Invalid value '%s' could not be parsed as a '%s' for the field '%s'" x.InnerText targetType targetField

    /// Acts as a "parser" for cases where a node's inner text is desired with
    /// no further processing.
    let getInnerText (x : XmlNode) = x.InnerText

    /// Processes the given values and returns the first matching result, if any
    /// matching results exist. If the first matching result is an empty string,
    /// no result is returned.
    let maybeOne (_, getter) xml field parser =
        let data : System.Xml.XmlNode option = getter xml field id |> Seq.tryHead
        match data with
        | None -> None
        | Some x when System.String.IsNullOrWhiteSpace x.InnerText -> None
        | Some x -> Some <| parser x

    /// Processes the given values and returns the first matching result. If no
    /// matching results are found, throws an exception.
    let exactlyOne (sourceType, getter) a b c =
        let ret = getter a b c |> Seq.tryHead
        match ret with
        | None -> failwithf "No %s could be found matching '%s'" sourceType b
        | Some x -> x

    /// Processes the given values and returns all matching results as an array.
    let getArray (_, getter) a b c : 'a array = getter a b c |> Seq.toArray
    /// Processes the given values and returns all matching results as a list.
    let getList  (_, getter) a b c : 'a list  = getter a b c |> Seq.toList
    /// Processes the given values and returns all matching results in a cached
    /// sequence, backed by an array.
    let getSeq   (_, getter) a b c : 'a seq   = getter a b c |> Seq.toArray |> Seq.ofArray
    /// Processes the given values and returns the results as an array. Returns
    /// None if no matching results are found.
    let getMaybeArray (_, getter) a b c : 'a array option = let rs = Seq.toArray (getter a b c) in if rs.Length = 0 then None else Some rs
    /// Processes the given values and returns the results as a list. Returns
    /// None if no matching results are found.
    let getMaybeList  (_, getter) a b c : 'a list  option = let rs = Seq.toList  (getter a b c) in if rs.Length = 0 then None else Some rs
    /// Processes the given values and returns the results in a cached sequence,
    /// backed by an array. Returns None if no matching results are found.
    let getMaybeSeq       getter  a b c : 'a seq   option = let rs = getMaybeArray getter a b c in Option.map Seq.ofArray rs

    /// Runs the given validation methods against the given value
    let validateAll staticUnion staticUnit memberUnion memberUnit x =
        let typeName = x.GetType().FullName
        let inline check x =
            match x with
            | ValidationResult.Valid -> ()
            | ValidationResult.Invalid msg -> failwithf "Validation failure processing a '%s' value: %s" typeName msg
        staticUnion |> Seq.iter (fun f -> check (f x))
        staticUnit  |> Seq.iter (fun f -> f x)
        memberUnion |> Seq.iter (fun f -> check (f x))
        memberUnit  |> Seq.iter (fun f -> f x)
        x
