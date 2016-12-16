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



open System.Xml

[<AutoOpen>]
/// Helpers used for the expanded FromXml code to simplify the processing of
/// XML data
module Helpers =
    /// Calls the given parser function and passes it the specified value; if
    /// the parser function indicates that the given value was not valid, throws
    /// an exception including the specified name
    let inline parse (g : string -> bool * 'a) name value =
        match g value with
        | (false, _) -> failwithf "Value for '%s' was not valid" name
        | (true , x) -> x
    /// <summary>
    /// If the value does not exist, or
    /// <see cref="System.String.IsNullOrWhiteSpace" /> returns true for the
    /// given value, then this function returns None. Otherwise, this function
    /// calls <see cref="parse" />.
    /// </summary>
    /// <remarks>
    /// If the given value is not empty but is not a valid value, an exception
    /// will be thrown by <see cref="parse" />.
    /// </remarks>
    let inline tryParse (g : string -> bool * 'a) name value =
        match value with
        | None -> None
        | Some x ->
            if String.IsNullOrWhiteSpace x then
                None
            else
                Some (parse g name x)


    /// Gets the inner text from all XML attributes provided whose names match
    /// the specified name
    let findAllAttr (xs : XmlAttribute seq) x =
        xs
        |> Seq.filter (fun y -> y.Name.Equals(x, StringComparison.OrdinalIgnoreCase))
        |> Seq.map (fun x -> x.InnerText)
    /// Returns the inner text from the first matching XML attribute if any
    /// attributes names match the specified name, returns None otherwise
    let tryFindAttr xs x = findAllAttr xs x |> Seq.tryHead
    /// Gets the inner text from the first matching XML attribute if any
    /// attributes names match the specified name, throws an exception otherwise
    let findAttr xs x =
        tryFindAttr xs x
        |> function
           | None -> failwithf "The '%s' attribute could not be found" x
           | Some x -> x


    /// Gets all XML nodes from the provided collection whose names match the
    /// specified name
    let findAllNodes (xs : XmlNode seq) x =
        xs |> Seq.filter (fun y -> y.Name.Equals(x, StringComparison.OrdinalIgnoreCase))
    /// Gets the first XML node whose name matches the specified name; returns
    /// None if no XML nodes match
    let tryFindNode xs x = findAllNodes xs x |> Seq.tryHead
    /// Gets the first XML node whose name matches the specified name; throws an
    /// exception if no XML nodes match
    let findNode xs x =
        tryFindNode xs x
        |> function
           | None -> failwithf "The '%s' node could not be found" x
           | Some x -> x


    /// Gets the inner text from all XML nodes whose names match the specified
    /// name
    let findAll xs x =
        findAllNodes xs x
        |> Seq.map (fun x -> x.InnerText)
    /// Gets the inner text of the first XML node whose name matches the
    /// specified name; returns None if no XML nodes match
    let tryFind xs x = findAll xs x |> Seq.tryHead
    /// Gets the inner text of the first XML node whose name matches the
    /// specified name; throws an exception if no XML nodes match
    let find xs x =
        tryFind xs x
        |> function
           | None -> failwithf "The '%s' node could not be found" x
           | Some x -> x


    /// Gets the inner text of all XML nodes and XML attributes whose names
    /// match the specified name
    let findAllEither (ys : XmlNode seq) (zs : XmlAttribute seq) (x : string) =
        let needle = x.Replace("_", "")
        let nodes =
            ys
            |> Seq.filter (fun y -> y.Name.Replace("_", "").Equals(needle, StringComparison.OrdinalIgnoreCase))
            |> Seq.map (fun x -> x.InnerText)
        let attrs =
            zs
            |> Seq.filter (fun z -> z.Name.Replace("_", "").Equals(needle, StringComparison.OrdinalIgnoreCase))
            |> Seq.map (fun x -> x.InnerText)
        Seq.append nodes attrs
    /// Gets the inner text of the first XML node or XML attribute whose name
    /// matches the specified name; returns None if no XML nodes or XML
    /// attributes match
    let tryFindEither ys zs x = findAllEither ys zs x |> Seq.tryHead
    /// Gets the inner text of the first XML node or XML attribute whose name
    /// matches the specified name; throws an exception if no XML nodes or XML
    /// attributes match
    let findEither ys zs x =
        tryFindEither ys zs x
        |> function
           | None -> failwithf "No nodes or attributes could be found matching the name '%s'" x
           | Some x -> x


    /// Gets the inner text of every node in the given collection
    let getInnerTexts (xs : XmlNodeList) : string seq =
        System.Linq.Enumerable.Cast<XmlNode> xs
        |> Seq.map (fun x -> x.InnerText)
    /// Tries to get the inner text of a node, and returns None if the node or
    /// its contents are null
    let tryInnerText (x : XmlNode) : string option =
        match Option.ofObj x with
        | None -> None
        | Some x -> Option.ofObj x.InnerText
