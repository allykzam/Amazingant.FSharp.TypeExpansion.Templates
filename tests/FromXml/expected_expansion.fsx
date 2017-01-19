
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

        type Node with
            static member FromXmlNode (xml : System.Xml.XmlNode) : Node =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'Node' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocElement doc "Node" Node.FromXmlNode
            static member FromXmlDoc xml = thingFromStringElement xml "Node" Node.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module NodeOpt_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type NodeOpt with
            static member FromXmlNode (xml : System.Xml.XmlNode) : NodeOpt =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'NodeOpt' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocElement doc "Node_Opt" NodeOpt.FromXmlNode
            static member FromXmlDoc xml = thingFromStringElement xml "Node_Opt" NodeOpt.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module NodeColl_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type NodeColl with
            static member FromXmlNode (xml : System.Xml.XmlNode) : NodeColl =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'NodeColl' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocElement doc "Node_Coll" NodeColl.FromXmlNode
            static member FromXmlDoc xml = thingFromStringElement xml "Node_Coll" NodeColl.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module NodeOptColl_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type NodeOptColl with
            static member FromXmlNode (xml : System.Xml.XmlNode) : NodeOptColl =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'NodeOptColl' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocElement doc "Node_Opt_Coll" NodeOptColl.FromXmlNode
            static member FromXmlDoc xml = thingFromStringElement xml "Node_Opt_Coll" NodeOptColl.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module Path_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type Path with
            static member FromXmlNode (xml : System.Xml.XmlNode) : Path =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'Path' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocXPath doc "xpath/path" Path.FromXmlNode
            static member FromXmlDoc xml = thingFromStringXPath xml "xpath/path" Path.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module PathOpt_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type PathOpt with
            static member FromXmlNode (xml : System.Xml.XmlNode) : PathOpt =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'PathOpt' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocXPath doc "xpath/path_opt" PathOpt.FromXmlNode
            static member FromXmlDoc xml = thingFromStringXPath xml "xpath/path_opt" PathOpt.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module PathColl_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type PathColl with
            static member FromXmlNode (xml : System.Xml.XmlNode) : PathColl =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'PathColl' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocXPath doc "xpath/path_coll" PathColl.FromXmlNode
            static member FromXmlDoc xml = thingFromStringXPath xml "xpath/path_coll" PathColl.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module PathOptColl_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type PathOptColl with
            static member FromXmlNode (xml : System.Xml.XmlNode) : PathOptColl =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'PathOptColl' value from it"
                {
                    ``Field`` = (exactlyOne fromTagsOrAttributes xml "Field" getInnerText);
                }

            static member FromXmlDoc doc = thingFromDocXPath doc "xpath/path_opt_coll" PathOptColl.FromXmlNode
            static member FromXmlDoc xml = thingFromStringXPath xml "xpath/path_opt_coll" PathOptColl.FromXmlNode


namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests
    [<AutoOpen>]
    module TestFields_FromXml_Extensions =

        open Amazingant.FSharp.TypeExpansion.Templates.FromXml

        type TestFields with
            static member FromXmlNode (xml : System.Xml.XmlNode) : TestFields =
                if isNull xml then failwithf "Given a null XmlNode and asked to parse a 'TestFields' value from it"
                {
                    ``SimpleString`` = (exactlyOne fromTagsOrAttributes xml "SimpleString" getInnerText);
                    ``MaybeString`` = (maybeOneString fromTagsOrAttributes xml "MaybeString" getInnerText);
                    ``StringList`` = (getList fromTagsOrAttributes xml "StringList" getInnerText);
                    ``StringArray`` = (getArray fromTagsOrAttributes xml "StringArray" getInnerText);
                    ``StringSeq`` = (getSeq fromTagsOrAttributes xml "StringSeq" getInnerText);
                    ``MaybeStringList`` = (getMaybeList fromTagsOrAttributes xml "MaybeStringList" getInnerText);
                    ``MaybeStringArray`` = (getMaybeArray fromTagsOrAttributes xml "MaybeStringArray" getInnerText);
                    ``MaybeStringSeq`` = (getMaybeSeq fromTagsOrAttributes xml "MaybeStringSeq" getInnerText);
                    ``SimpleField`` = (exactlyOne fromTagsOrAttributes xml "SimpleField" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.SimpleField" System.Int32.TryParse));
                    ``MaybeField`` = (maybeOne fromTagsOrAttributes xml "MaybeField" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeField" System.Int32.TryParse));
                    ``FieldList`` = (getList fromTagsOrAttributes xml "FieldList" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.FieldList" System.Int32.TryParse));
                    ``FieldArray`` = (getArray fromTagsOrAttributes xml "FieldArray" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.FieldArray" System.Int32.TryParse));
                    ``FieldSeq`` = (getSeq fromTagsOrAttributes xml "FieldSeq" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.FieldSeq" System.Int32.TryParse));
                    ``MaybeFieldList`` = (getMaybeList fromTagsOrAttributes xml "MaybeFieldList" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeFieldList" System.Int32.TryParse));
                    ``MaybeFieldArray`` = (getMaybeArray fromTagsOrAttributes xml "MaybeFieldArray" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeFieldArray" System.Int32.TryParse));
                    ``MaybeFieldSeq`` = (getMaybeSeq fromTagsOrAttributes xml "MaybeFieldSeq" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeFieldSeq" System.Int32.TryParse));
                    ``SimpleXPathString`` = (exactlyOne fromAnXPath xml "string" getInnerText);
                    ``MaybeXPathString`` = (maybeOneString fromAnXPath xml "string_opt" getInnerText);
                    ``XPathStringList`` = (getList fromAnXPath xml "string_coll" getInnerText);
                    ``XPathStringArray`` = (getArray fromAnXPath xml "string_coll" getInnerText);
                    ``XPathStringSeq`` = (getSeq fromAnXPath xml "string_coll" getInnerText);
                    ``MaybeXPathStringList`` = (getMaybeList fromAnXPath xml "string_opt_coll" getInnerText);
                    ``MaybeXPathStringArray`` = (getMaybeArray fromAnXPath xml "string_opt_coll" getInnerText);
                    ``MaybeXPathStringSeq`` = (getMaybeSeq fromAnXPath xml "string_opt_coll" getInnerText);
                    ``SimpleXPathField`` = (exactlyOne fromAnXPath xml "int" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.SimpleXPathField" System.Int32.TryParse));
                    ``MaybeXPathField`` = (maybeOne fromAnXPath xml "int_opt" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeXPathField" System.Int32.TryParse));
                    ``XPathFieldList`` = (getList fromAnXPath xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.XPathFieldList" System.Int32.TryParse));
                    ``XPathFieldArray`` = (getArray fromAnXPath xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.XPathFieldArray" System.Int32.TryParse));
                    ``XPathFieldSeq`` = (getSeq fromAnXPath xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.XPathFieldSeq" System.Int32.TryParse));
                    ``MaybeXPathFieldList`` = (getMaybeList fromAnXPath xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeXPathFieldList" System.Int32.TryParse));
                    ``MaybeXPathFieldArray`` = (getMaybeArray fromAnXPath xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeXPathFieldArray" System.Int32.TryParse));
                    ``MaybeXPathFieldSeq`` = (getMaybeSeq fromAnXPath xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeXPathFieldSeq" System.Int32.TryParse));
                    ``SimpleNodeString`` = (exactlyOne fromXmlTags xml "string" getInnerText);
                    ``MaybeNodeString`` = (maybeOneString fromXmlTags xml "string_opt" getInnerText);
                    ``NodeStringList`` = (getList fromXmlTags xml "string_coll" getInnerText);
                    ``NodeStringArray`` = (getArray fromXmlTags xml "string_coll" getInnerText);
                    ``NodeStringSeq`` = (getSeq fromXmlTags xml "string_coll" getInnerText);
                    ``MaybeNodeStringList`` = (getMaybeList fromXmlTags xml "string_opt_coll" getInnerText);
                    ``MaybeNodeStringArray`` = (getMaybeArray fromXmlTags xml "string_opt_coll" getInnerText);
                    ``MaybeNodeStringSeq`` = (getMaybeSeq fromXmlTags xml "string_opt_coll" getInnerText);
                    ``SimpleNodeField`` = (exactlyOne fromXmlTags xml "int" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.SimpleNodeField" System.Int32.TryParse));
                    ``MaybeNodeField`` = (maybeOne fromXmlTags xml "int_opt" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeNodeField" System.Int32.TryParse));
                    ``NodeFieldList`` = (getList fromXmlTags xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.NodeFieldList" System.Int32.TryParse));
                    ``NodeFieldArray`` = (getArray fromXmlTags xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.NodeFieldArray" System.Int32.TryParse));
                    ``NodeFieldSeq`` = (getSeq fromXmlTags xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.NodeFieldSeq" System.Int32.TryParse));
                    ``MaybeNodeFieldList`` = (getMaybeList fromXmlTags xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeNodeFieldList" System.Int32.TryParse));
                    ``MaybeNodeFieldArray`` = (getMaybeArray fromXmlTags xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeNodeFieldArray" System.Int32.TryParse));
                    ``MaybeNodeFieldSeq`` = (getMaybeSeq fromXmlTags xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeNodeFieldSeq" System.Int32.TryParse));
                    ``SimpleAttrString`` = (exactlyOne fromAttributes xml "string" getInnerText);
                    ``MaybeAttrString`` = (maybeOneString fromAttributes xml "string_opt" getInnerText);
                    ``AttrStringList`` = (getList fromAttributes xml "string_coll" getInnerText);
                    ``AttrStringArray`` = (getArray fromAttributes xml "string_coll" getInnerText);
                    ``AttrStringSeq`` = (getSeq fromAttributes xml "string_coll" getInnerText);
                    ``MaybeAttrStringList`` = (getMaybeList fromAttributes xml "string_opt_coll" getInnerText);
                    ``MaybeAttrStringArray`` = (getMaybeArray fromAttributes xml "string_opt_coll" getInnerText);
                    ``MaybeAttrStringSeq`` = (getMaybeSeq fromAttributes xml "string_opt_coll" getInnerText);
                    ``SimpleAttrField`` = (exactlyOne fromAttributes xml "int" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.SimpleAttrField" System.Int32.TryParse));
                    ``MaybeAttrField`` = (maybeOne fromAttributes xml "int_opt" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeAttrField" System.Int32.TryParse));
                    ``AttrFieldList`` = (getList fromAttributes xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.AttrFieldList" System.Int32.TryParse));
                    ``AttrFieldArray`` = (getArray fromAttributes xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.AttrFieldArray" System.Int32.TryParse));
                    ``AttrFieldSeq`` = (getSeq fromAttributes xml "int_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.AttrFieldSeq" System.Int32.TryParse));
                    ``MaybeAttrFieldList`` = (getMaybeList fromAttributes xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeAttrFieldList" System.Int32.TryParse));
                    ``MaybeAttrFieldArray`` = (getMaybeArray fromAttributes xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeAttrFieldArray" System.Int32.TryParse));
                    ``MaybeAttrFieldSeq`` = (getMaybeSeq fromAttributes xml "int_opt_coll" (parserForStrings "System.Int32" "Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.TestFields.MaybeAttrFieldSeq" System.Int32.TryParse));
                    ``SimpleNestedField`` = (exactlyOne fromXmlTags xml "Node" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeNestedField`` = (maybeOne fromXmlTags xml "Node_Opt" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeOpt.FromXmlNode);
                    ``NestedFieldList`` = (getList fromXmlTags xml "Node_Coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeColl.FromXmlNode);
                    ``NestedFieldArray`` = (getArray fromXmlTags xml "Node_Coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeColl.FromXmlNode);
                    ``NestedFieldSeq`` = (getSeq fromXmlTags xml "Node_Coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeColl.FromXmlNode);
                    ``MaybeNestedFieldList`` = (getMaybeList fromXmlTags xml "Node_Opt_Coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeOptColl.FromXmlNode);
                    ``MaybeNestedFieldArray`` = (getMaybeArray fromXmlTags xml "Node_Opt_Coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeOptColl.FromXmlNode);
                    ``MaybeNestedFieldSeq`` = (getMaybeSeq fromXmlTags xml "Node_Opt_Coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.NodeOptColl.FromXmlNode);
                    ``SimpleMultiAttrField`` = (exactlyOne fromXmlTags xml "other_node" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeMultiAttrField`` = (maybeOne fromXmlTags xml "other_node_opt" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MultiAttrFieldList`` = (getList fromXmlTags xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MultiAttrFieldArray`` = (getArray fromXmlTags xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MultiAttrFieldSeq`` = (getSeq fromXmlTags xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeMultiAttrFieldList`` = (getMaybeList fromXmlTags xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeMultiAttrFieldArray`` = (getMaybeArray fromXmlTags xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeMultiAttrFieldSeq`` = (getMaybeSeq fromXmlTags xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``SimpleXPathNestedField`` = (exactlyOne fromAnXPath xml "xpath/path" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``MaybeXPathNestedField`` = (maybeOne fromAnXPath xml "xpath/path_opt" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathOpt.FromXmlNode);
                    ``XPathNestedFieldList`` = (getList fromAnXPath xml "xpath/path_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathColl.FromXmlNode);
                    ``XPathNestedFieldArray`` = (getArray fromAnXPath xml "xpath/path_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathColl.FromXmlNode);
                    ``XPathNestedFieldSeq`` = (getSeq fromAnXPath xml "xpath/path_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathColl.FromXmlNode);
                    ``MaybeXPathNestedFieldList`` = (getMaybeList fromAnXPath xml "xpath/path_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathOptColl.FromXmlNode);
                    ``MaybeXPathNestedFieldArray`` = (getMaybeArray fromAnXPath xml "xpath/path_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathOptColl.FromXmlNode);
                    ``MaybeXPathNestedFieldSeq`` = (getMaybeSeq fromAnXPath xml "xpath/path_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.PathOptColl.FromXmlNode);
                    ``SimpleNestedXPathField`` = (exactlyOne fromAnXPath xml "other_node" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeNestedXPathField`` = (maybeOne fromAnXPath xml "other_node_opt" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``NestedXPathFieldList`` = (getList fromAnXPath xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``NestedXPathFieldArray`` = (getArray fromAnXPath xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``NestedXPathFieldSeq`` = (getSeq fromAnXPath xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeNestedXPathFieldList`` = (getMaybeList fromAnXPath xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeNestedXPathFieldArray`` = (getMaybeArray fromAnXPath xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``MaybeNestedXPathFieldSeq`` = (getMaybeSeq fromAnXPath xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Node.FromXmlNode);
                    ``SimpleXPathNestedXPathField`` = (exactlyOne fromAnXPath xml "other_node" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``MaybeXPathNestedXPathField`` = (maybeOne fromAnXPath xml "other_node_opt" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``XPathNestedXPathFieldList`` = (getList fromAnXPath xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``XPathNestedXPathFieldArray`` = (getArray fromAnXPath xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``XPathNestedXPathFieldSeq`` = (getSeq fromAnXPath xml "other_node_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``MaybeXPathNestedXPathFieldList`` = (getMaybeList fromAnXPath xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``MaybeXPathNestedXPathFieldArray`` = (getMaybeArray fromAnXPath xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                    ``MaybeXPathNestedXPathFieldSeq`` = (getMaybeSeq fromAnXPath xml "other_node_opt_coll" Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests.Path.FromXmlNode);
                }

            static member FromXmlDoc doc = thingFromDocElement doc "Test" TestFields.FromXmlNode
            static member FromXmlDoc xml = thingFromStringElement xml "Test" TestFields.FromXmlNode
