//#r "../../packages/Amazingant.FSharp.TypeExpansion/lib/net45/Amazingant.FSharp.TypeExpansion.Attributes.dll"
#r @"M:\Users\amazingant\GitHub\Amazingant.FSharp.TypeExpansion\Source\Amazingant.FSharp.TypeExpansion\bin\Debug\Amazingant.FSharp.TypeExpansion.Attributes.dll"
#load "../../templates/FromXml/FromXml_Base.fsx"

namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests

open Amazingant.FSharp.TypeExpansion.Templates.FromXml
open Amazingant.FSharp.TypeExpansion.Attributes

// These are here for test fields that need a "nested" type. The base "Node"
// type can be used in any situation where a field will have an XmlNode
// attribute on it; the other types are needed for fields that are
// optional/collections/etc.
[<XmlNode("node"         ); ExpandableType([| "FromXml" |])>] type Node        = { Field : string; }
[<XmlNode("node_opt"     ); ExpandableType([| "FromXml" |])>] type NodeOpt     = { Field : string; }
[<XmlNode("node_coll"    ); ExpandableType([| "FromXml" |])>] type NodeColl    = { Field : string; }
[<XmlNode("node_opt_coll"); ExpandableType([| "FromXml" |])>] type NodeOptColl = { Field : string; }


[<XmlNode("Test"); ExpandableType([| "FromXml" |])>]
type TestFields =
    {
        // **** Simple fields **** //
        // Strings are treated differently than other types
        SimpleString      :  string              ;
    }
