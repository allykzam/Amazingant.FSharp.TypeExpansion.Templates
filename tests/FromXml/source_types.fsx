#r "../../packages/Amazingant.FSharp.TypeExpansion/lib/net47/Amazingant.FSharp.TypeExpansion.Attributes.dll"
#load "../../templates/FromXml/FromXml_Base.fsx"

namespace Amazingant.FSharp.TypeExpansion.Templates.FromXml.Tests

open Amazingant.FSharp.TypeExpansion.Templates.FromXml
open Amazingant.FSharp.TypeExpansion.Attributes

// These are here for test fields that need a "nested" type. The base "Node"
// type can be used in any situation where a field will have an XmlNode
// attribute on it; the other types are needed for fields that are
// optional/collections/etc.
[<XmlNode("Node"         ); ExpandableType([| "FromXml" |])>] type Node        = { Field : string; }
[<XmlNode("Node_Opt"     ); ExpandableType([| "FromXml" |])>] type NodeOpt     = { Field : string; }
[<XmlNode("Node_Coll"    ); ExpandableType([| "FromXml" |])>] type NodeColl    = { Field : string; }
[<XmlNode("Node_Opt_Coll"); ExpandableType([| "FromXml" |])>] type NodeOptColl = { Field : string; }

// These are here for test fields that need a "nested" type with the XPath
// attribute specifically. Otherwise, they should match the Node types above.
[<XPath("xpath/path"         ); ExpandableType([| "FromXml" |])>] type Path        = { Field : string; }
[<XPath("xpath/path_opt"     ); ExpandableType([| "FromXml" |])>] type PathOpt     = { Field : string; }
[<XPath("xpath/path_coll"    ); ExpandableType([| "FromXml" |])>] type PathColl    = { Field : string; }
[<XPath("xpath/path_opt_coll"); ExpandableType([| "FromXml" |])>] type PathOptColl = { Field : string; }


[<XmlNode("Test"); ExpandableType([| "FromXml" |])>]
type TestFields =
    {
        // **** Simple fields **** //
        // Strings are treated differently than other types
        SimpleString      :  string              ;
         MaybeString      :  string        option;
              StringList  :  string list         ;
              StringArray :  string array        ;
              StringSeq   :  string seq          ;
         MaybeStringList  : (string list ) option;
         MaybeStringArray : (string array) option;
         MaybeStringSeq   : (string seq  ) option;
        // Now the simple fields; these use `int`, but any other type should be
        // fine so long as the type itself does not have one of the FromXml
        // attributes on it
        SimpleField       :  int                 ;
         MaybeField       :  int           option;
              FieldList   :  int    list         ;
              FieldArray  :  int    array        ;
              FieldSeq    :  int    seq          ;
         MaybeFieldList   : (int    list ) option;
         MaybeFieldArray  : (int    array) option;
         MaybeFieldSeq    : (int    seq  ) option;

        // **** XPath fields **** //
        // Strings with an XPath attribute on the field
        [<XPath("string"         )>] SimpleXPathString      :  string              ;
        [<XPath("string_opt"     )>]  MaybeXPathString      :  string        option;
        [<XPath("string_coll"    )>]       XPathStringList  :  string list         ;
        [<XPath("string_coll"    )>]       XPathStringArray :  string array        ;
        [<XPath("string_coll"    )>]       XPathStringSeq   :  string seq          ;
        [<XPath("string_opt_coll")>]  MaybeXPathStringList  : (string list ) option;
        [<XPath("string_opt_coll")>]  MaybeXPathStringArray : (string array) option;
        [<XPath("string_opt_coll")>]  MaybeXPathStringSeq   : (string seq  ) option;
        // Non-string fields with an XPath attribute on the field
        [<XPath("int"            )>] SimpleXPathField       :  int                 ;
        [<XPath("int_opt"        )>]  MaybeXPathField       :  int           option;
        [<XPath("int_coll"       )>]       XPathFieldList   :  int    list         ;
        [<XPath("int_coll"       )>]       XPathFieldArray  :  int    array        ;
        [<XPath("int_coll"       )>]       XPathFieldSeq    :  int    seq          ;
        [<XPath("int_opt_coll"   )>]  MaybeXPathFieldList   : (int    list ) option;
        [<XPath("int_opt_coll"   )>]  MaybeXPathFieldArray  : (int    array) option;
        [<XPath("int_opt_coll"   )>]  MaybeXPathFieldSeq    : (int    seq  ) option;

        // **** XmlNode fields **** //
        // Strings with an XmlNode attribute on the field
        [<XmlNode("string"         )>] SimpleNodeString      :  string              ;
        [<XmlNode("string_opt"     )>]  MaybeNodeString      :  string        option;
        [<XmlNode("string_coll"    )>]       NodeStringList  :  string list         ;
        [<XmlNode("string_coll"    )>]       NodeStringArray :  string array        ;
        [<XmlNode("string_coll"    )>]       NodeStringSeq   :  string seq          ;
        [<XmlNode("string_opt_coll")>]  MaybeNodeStringList  : (string list ) option;
        [<XmlNode("string_opt_coll")>]  MaybeNodeStringArray : (string array) option;
        [<XmlNode("string_opt_coll")>]  MaybeNodeStringSeq   : (string seq  ) option;
        // Non-string fields with an XmlNode attribute on the field
        [<XmlNode("int"            )>] SimpleNodeField       :  int                 ;
        [<XmlNode("int_opt"        )>]  MaybeNodeField       :  int           option;
        [<XmlNode("int_coll"       )>]       NodeFieldList   :  int    list         ;
        [<XmlNode("int_coll"       )>]       NodeFieldArray  :  int    array        ;
        [<XmlNode("int_coll"       )>]       NodeFieldSeq    :  int    seq          ;
        [<XmlNode("int_opt_coll"   )>]  MaybeNodeFieldList   : (int    list ) option;
        [<XmlNode("int_opt_coll"   )>]  MaybeNodeFieldArray  : (int    array) option;
        [<XmlNode("int_opt_coll"   )>]  MaybeNodeFieldSeq    : (int    seq  ) option;

        // **** XmlAttr fields **** //
        // Strings with an XmlAttr attribute on the field
        [<XmlAttr("string"         )>] SimpleAttrString      :  string              ;
        [<XmlAttr("string_opt"     )>]  MaybeAttrString      :  string        option;
        [<XmlAttr("string_coll"    )>]       AttrStringList  :  string list         ;
        [<XmlAttr("string_coll"    )>]       AttrStringArray :  string array        ;
        [<XmlAttr("string_coll"    )>]       AttrStringSeq   :  string seq          ;
        [<XmlAttr("string_opt_coll")>]  MaybeAttrStringList  : (string list ) option;
        [<XmlAttr("string_opt_coll")>]  MaybeAttrStringArray : (string array) option;
        [<XmlAttr("string_opt_coll")>]  MaybeAttrStringSeq   : (string seq  ) option;
        // Non-string fields with an XmlAttr attribute on the field
        [<XmlAttr("int"            )>] SimpleAttrField       :  int                 ;
        [<XmlAttr("int_opt"        )>]  MaybeAttrField       :  int           option;
        [<XmlAttr("int_coll"       )>]       AttrFieldList   :  int    list         ;
        [<XmlAttr("int_coll"       )>]       AttrFieldArray  :  int    array        ;
        [<XmlAttr("int_coll"       )>]       AttrFieldSeq    :  int    seq          ;
        [<XmlAttr("int_opt_coll"   )>]  MaybeAttrFieldList   : (int    list ) option;
        [<XmlAttr("int_opt_coll"   )>]  MaybeAttrFieldArray  : (int    array) option;
        [<XmlAttr("int_opt_coll"   )>]  MaybeAttrFieldSeq    : (int    seq  ) option;

        // **** Fields with an XmlNode attribute on the type **** //
        SimpleNestedField      :  Node                     ;
         MaybeNestedField      :  NodeOpt            option;
              NestedFieldList  :  NodeColl    list         ;
              NestedFieldArray :  NodeColl    array        ;
              NestedFieldSeq   :  NodeColl    seq          ;
         MaybeNestedFieldList  : (NodeOptColl list ) option;
         MaybeNestedFieldArray : (NodeOptColl array) option;
         MaybeNestedFieldSeq   : (NodeOptColl seq  ) option;

        // **** Fields with an XmlNode attribute on the type and an XmlNode attribute on the field **** //
        [<XmlNode("other_node"         )>] SimpleMultiAttrField      :  Node              ;
        [<XmlNode("other_node_opt"     )>]  MaybeMultiAttrField      :  Node        option;
        [<XmlNode("other_node_coll"    )>]       MultiAttrFieldList  :  Node list         ;
        [<XmlNode("other_node_coll"    )>]       MultiAttrFieldArray :  Node array        ;
        [<XmlNode("other_node_coll"    )>]       MultiAttrFieldSeq   :  Node seq          ;
        [<XmlNode("other_node_opt_coll")>]  MaybeMultiAttrFieldList  : (Node list ) option;
        [<XmlNode("other_node_opt_coll")>]  MaybeMultiAttrFieldArray : (Node array) option;
        [<XmlNode("other_node_opt_coll")>]  MaybeMultiAttrFieldSeq   : (Node seq  ) option;

        // **** Fields with an XPath attribute on the type **** //
        SimpleXPathNestedField      :  Path                     ;
         MaybeXPathNestedField      :  PathOpt            option;
              XPathNestedFieldList  :  PathColl    list         ;
              XPathNestedFieldArray :  PathColl    array        ;
              XPathNestedFieldSeq   :  PathColl    seq          ;
         MaybeXPathNestedFieldList  : (PathOptColl list ) option;
         MaybeXPathNestedFieldArray : (PathOptColl array) option;
         MaybeXPathNestedFieldSeq   : (PathOptColl seq  ) option;

        // **** Fields with an XPath attribute on the field and an XmlNode attribute on the type **** //
        [<XPath("other_node"         )>] SimpleNestedXPathField      :  Node              ;
        [<XPath("other_node_opt"     )>]  MaybeNestedXPathField      :  Node        option;
        [<XPath("other_node_coll"    )>]       NestedXPathFieldList  :  Node list         ;
        [<XPath("other_node_coll"    )>]       NestedXPathFieldArray :  Node array        ;
        [<XPath("other_node_coll"    )>]       NestedXPathFieldSeq   :  Node seq          ;
        [<XPath("other_node_opt_coll")>]  MaybeNestedXPathFieldList  : (Node list ) option;
        [<XPath("other_node_opt_coll")>]  MaybeNestedXPathFieldArray : (Node array) option;
        [<XPath("other_node_opt_coll")>]  MaybeNestedXPathFieldSeq   : (Node seq  ) option;

        // **** Fields with an XPath attribute on the field and an XPath attribute on the type **** //
        // These tests should hit the same match cases as the above fields
        [<XPath("other_node"         )>] SimpleXPathNestedXPathField      :  Path              ;
        [<XPath("other_node_opt"     )>]  MaybeXPathNestedXPathField      :  Path        option;
        [<XPath("other_node_coll"    )>]       XPathNestedXPathFieldList  :  Path list         ;
        [<XPath("other_node_coll"    )>]       XPathNestedXPathFieldArray :  Path array        ;
        [<XPath("other_node_coll"    )>]       XPathNestedXPathFieldSeq   :  Path seq          ;
        [<XPath("other_node_opt_coll")>]  MaybeXPathNestedXPathFieldList  : (Path list ) option;
        [<XPath("other_node_opt_coll")>]  MaybeXPathNestedXPathFieldArray : (Path array) option;
        [<XPath("other_node_opt_coll")>]  MaybeXPathNestedXPathFieldSeq   : (Path seq  ) option;

        // **** Fields needed to test issues **** //
        // Issue #17
        [<XmlNode("underscored")>] IgnoreUnderscoreInNode : int;
        [<XmlAttr("underscored")>] IgnoreUnderscoreInAttr : int;
                                         Underscored      : int;
    }
    [<Validation>] static member    ValidationOne   (_ : TestFields) = ValidationResult.Valid
    [<Validation>] static member    ValidationTwo   (_ : TestFields) = ()
    [<Validation>]        member __.ValidationThree (              ) = ValidationResult.Valid
    [<Validation>]        member __.ValidationFour  (              ) = ()
