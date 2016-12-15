Amazingant.FSharp.TypeExpansion.Templates
=========================================

This package contains templates for use with the
`Amazingant.FSharp.TypeExpansion` type provider.


Templates
---------

### Lenses

This template provides two types for lensing, `StaticLens<'a,'b>` and
`Lens<'a,'b>`, to use for creating lenses for nested record types.

The type expansion template provides a static member of type `StaticLens<'a,'b>`
for each field (of type `'b`) in the processed record of type `'a`, and a
non-static member of type `Lens<'a,'b>` for the same. The template additionally
composes the `StaticLens<'a,'b>` members for nested types, creating a static
member of type `StaticLens<'a,'c>` for each field of type `'c` in the nested
record of type `'b` which is, in turn, a field in the record type `'a`.

Example:

```FSharp
[<Lens; ExpandableType([| "Lenses" |])>]
type Coordinate = { X : int; Y : int; Z : int; }

[<Lens; ExpandableType([| "Lenses" |])>]
type Player =
    {
        Name : string;
        Position : Coordinate;
    }
```

The expansion of these two types causes individual lenses to be created for each
axis of a `Player`'s `Position`, such that a `Player` can be moved like so:

```FSharp
let MoveRight (p : Player) (distance : int) =
    p.Position_X_Lens.Map ((+) distance)
```

This `MoveRight` function now takes a `Player` and moves them to the right by
the specified distance, and returns the new `Player` value that reflects this.


While this lensing template is not as feature-complete as some lensing tools
are, and the lens names can be very long depending on the nesting depth and
field names, the benefit that this template provides is that lenses are created
automatically via the type expansion system. These lenses do not require
manually creating lenses as some tools do, and the alternative in vanilla F# is
much longer:

```FSharp
let MoveRight (p : Player) (distance : int) =
    { p with Position = { p.Position with X = (p.Position.X + distance) } }
```

And of course with deeper nesting, the length of vanilla F# record updates grows
much faster than the lenses provided by this template. In the event that a more
feature-rich lensing library is needed, feel free to use this template as a
reference to build a template for that lensing library, if doing so will improve
its usefulness and/or usability.


### FromXml

This template provides a `FromXmlNode` extension method for any type provided to
it. Types which have the `XmlNode` attribute will additionally receive two
`FromXmlDoc` extension methods, the first of which takes a
`System.Xml.XmlDocument` object, and the second of which takes a string and
loads it as an `XmlDocument` object before calling the first function.

These extension methods serve to load a record type and any nested types from an
XML document. These methods are intended to build F# record types only, and are
not likely to work with custom classes, and the XML processing done is very
simplistic. If more advanced processing is needed, the [XML type
provider][xml-provider] from from F# Data should be considered instead.

A basic example:

```FSharp
[<XmlNode("Item_Data"); ExpandableType([| "FromXml" |])>]
type ItemData =
    {
        ItemId : string;
        PromotionStart : DateTimeOffset;
        [<XmlAttr("free_shipping")>]
        HasFreeShipping : bool;
    }
```

Note that the above example makes use of both the `XmlNode` and `XmlAttr`
attributes; the `XmlNode` attribute can be used on both the record type, and
individual fields, whereas the `XmlAttr` attribute can only be used on the
record's fields. Either node indicates to this template what part of the XML to
process. The result is that the following XML can be passed to
`ItemData.FromXmlDoc`, and a valid `ItemData` value will be produced:

```XML
<ITEM_DATA FREE_SHIPPING="true">
    <ITEM_ID>123abc</ITEM_ID>
    <PROMOTION_START>2016-01-01 12:34:56 +00:00</PROMOTION_START>
</ITEM_DATA>
```

In addition to the basic information provided below, more advanced processing
can be done. Individual fields in the processed record type can be optional, or
one of the three main collection types used in F# code (arrays, F#'s `list`, and
`seq`). These can be combined in a handful of ways, and the template will
provide an error if it cannot process the combination provided.

If a record field is of a type that also has an `XmlNode` attribute on it, the
`FromXmlNode` method for that type will be used to process it. Nested types like
this improve the levels of `Option<'T>` and the collection types which can be
used. As an example, the above `ItemData` type could be modified to contain a
`Promotions` field:

```FSharp
[<XmlNode("Sales_Promotion"); ExpandableType([| "FromXml" |])>]
type Promotion =
    {
        ...
    }

[<XmlNode("Item_Data"); ExpandableType([| "FromXml" |])>]
type ItemData =
    {
        ...
        [<XmlNode("Promotions")>]
        Promotions : (Promotion list) option;
    }
```

If the `Promotions` node was empty or not present during processing, the
`Promotions` field would be set to `None`. But if the `Promotions` node was
present and contained one or more `Sales_Promotion` nodes, each
`Sales_Promotion` node would be processed into a `Promotion` value and stored in
the resulting list.

The rules around nesting levels of `list` and `option` are a bit flexible, so
feel free to play around with them; however, be sure to test the result with
sample XML documents to ensure that the result matches the expectations.

An additional point of note, when specifying `XmlNode("Item_Data")` or
`XmlAttr("free_shipping")`, the name specified is case-insensitive. The
specified name is free to be all lowercase while your data source provides
uppercased XML nodes, or visa versa. However, when neither `XmlNode` nor
`XmlAttr` are used for a field, the processing code will additionally strip out
underscores in the XML node and attribute names while processing. This means
that in the initial example, the `ItemId` field in the record type will
successfully match XML nodes or attributes named e.g. `ITEM_ID`, `itemid`, or
even `I_t_E_m_I_d`. Of course, one should not take this as a suggestion to go
crazy with mixed upper and lower case letters or underscores.


For cases where XML nodes are nested in containers such as the following
example, the `XPath` attribute can be used with an XPath specifier.

```XML
<ITEM_DATA FREE_SHIPPING="true">
    <ITEM_ID>123abc</ITEM_ID>
    <PROMOTIONS>
        <PROMOTION>Free Shipping</PROMOTION>
        <PROMOTION>10% Off</PROMOTION>
    </PROMOTIONS>
</ITEM_DATA>
```

In such a case, creating a `Promotions` type just to access the `PROMOTION`
nodes is needlessly required. An XPath specifier can be used to avoid this:

```FSharp
[<XmlNode("Item_Data"); ExpandableType([| "FromXml" |])>]
type ItemData =
    {
        ...
        [<XPath("PROMOTIONS/PROMOTION")>]
        Promotions : string list;
    }
```

Points of note with the `XPath` attribute:

* Fields that are tagged with the `XPath` attribute currently cannot be of
  another type with an `XmlNode` attribute
* Any valid XPath specifier can be used, but those which contain double-quotes
  (`"`) will cause a compiler error after expansion is complete. Prefer
  single-quotes (`'`) within the XPath string to avoid this.
* Escaped characters will need to be prefixed with two extra backslashes to
  account for the fact that the string is going to be dumped into an F# source
  file and compiled again (fun, right?)



License
-------

This project is Copyright © 2016 Anthony Perez a.k.a. amazingant, and is
licensed under the MIT license. See the [LICENSE file][license] for more
details.


[xml-provider]: https://fsharp.github.io/FSharp.Data/library/XmlProvider.html
[license]: https://github.com/amazingant/Amazingant.FSharp.TypeExpansion.Templates/blob/master/LICENSE
