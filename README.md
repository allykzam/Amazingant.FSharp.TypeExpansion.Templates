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




### ImmutableViewModel

This template builds a view model for use with WPF, backed by the user-defined
data model. Generated view models contain a backing state, using the data model
type. There are a few third-party dependencies required for this template; see
the end of this template's documentation for more information.

With this template, fields and properties on the base data model are replicated
as properties on the generated view model. These properties contain both getters
and setters, meaning that, as an example, a string field from the data model can
be bound to a `TextBox` in the UI, allowing users to modify the field's
contents. When a property's setter is called, if modifications are not already
in-progress, the view model's internal state will be replaced with the requested
changes, and the appropriate property changed events will be raised.

Fields which are typed as an F# List will be exposed in the view model as a
`ReadOnlyObservableCollection`, although operations on these lists are currently
reported to WPF as a complete clear and re-build of the collection. Fields which
are typed as an F# Map will be exposed as an `ObservableDictionary` (see the
Third-Party Dependencies section below) containing the appropriate types. This
supports nesting types quite a bit as well, including automatic use of generated
view model types in the nested maps. Note that while the `ObservableDictionary`
type and its contents (including any generated view model types) are mutable,
and can be modified without properly informing the main view model of a state
change, such changes will be overwritten when the associated field(s) change in
the backing state. Likewise, as such changes will not be reported up to the main
view model, commands operating on the main view model will be given a state
value that does not include those changes. This behavior will change at some
point when a `ReadOnlyObservableMap` type can be written.

When a view model type is nested within a collection in another view model, to
better support WPF, consider adding the `ObservableCollectionKey` attribute to a
field or property on the nested view model's base data model type. If the nested
data model contains a single field which can uniquely identify it, operations on
the containing map will take this into account, and update the nested view
model's state when appropriate. Without using this attribute, when the nested
data model changes in any way, the nested view model will be completely
replaced, leading WPF UI elements to lose state. As an example, in my
prototyping phase, I displayed a map in a WPF `TreeView`; when a nested data
model changes, the UI would lose its selection if the attribute is not used, but
would retain its selection with the attribute.

Data transformation functions can be defined in an appropriate module, and will
cause `ICommand` instances to be generated on the view model. To do this, the
module must be marked with the `HelperModule` attribute, e.g.
`[<ImmutableViewModel.HelperModule(typeof<DataModel>)>]`. When this attribute is
found, `ICommand` instances will be made for any function matching one of the
expected type signatures. While one of these commands executes, the view model's
internal state will be flagged as being processed; this status can be checked
with the `InternalState_IsProcessing` property. While an asynchronous command
executes, all commands will be disabled, ensuring that multiple state changes do
not have to be merged together somehow.

When defining a data transformation function, assuming the data model is very
unoriginally named `DataModel`, the functions should be typed as
`DataModel -> DataModel` for a simple synchronous command, or
`DataModel -> 'a -> DataModel` for a synchronous command that takes a parameter.
Replacing the final return type with `Async<DataModel>` will result in an
asynchronous command.

If a command's ability to execute depends on the current state, e.g. if your
data model has an optional "selected value" field and you are defining commands
that operate on the selected value, copy the main function's name, and define a
separate function with "_CanExec" on the end of the name. This function must
take the same parameters as the main function, but return a boolean. When a
function with a matching name is detected, it will be checked for the
appropriate signature, and used to enable and disable the command appropriately.
Be aware that your "_CanExec" function will not override the internal behavior
of disabling all commands while an asynchronous command executes.

Last, for any properties or commands which depend upon the value of fields in
the data model, in the module with the matching `HelperModule` attribute, define
one or more functions with the `PropertyDependencies` attribute. These functions
must take a value of your data model type, and return one of `Expr * Expr`,
`(Expr * Expr) list`, or `Expr * (Expr list)`. See the XML documentation on the
`PropertyDependencies` attribute for more details. Be aware that if you have
defined "_CanExec" functions for enabling and disabling commands based on
current state, you almost certainly need to define dependencies for those
commands, otherwise they will not automatically enable and disable when the
state is updated.



#### Third-party dependencies.

As a warning, this template requires a few extra dependencies. The NLog logging
library is used to log warnings when attempting to change internal state at
invalid times. This is easy enough to remove; locate the `logger` value created
by calling `NLog.LogManager.GetCurrentClassLogger()`, comment it out, and fix
the new compiler errors. Calls to the logger can either be replaced with your
own logging tools, or just with `()` to act as a no-op.

The `ObservableDictionary` type defined above is a third-party class written in
C#, and must be compiled and added to your project seperately. The source code
for this class has been included, or can be found at [DrWPF's website][drwpf],
and has its own license documented at the top of the file. At some point it
would be nice to replace this class with a type defined within the "base" file
for the template, but this existing code saved a fair amount of development time
during the prototyping phase of the template.




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
    [<Validation>]
    member x.ReasonableStartTime() = if x.PromotionStart.Year < 2000 then failwith "Promotion start date is too far in the past."
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

Also note that the F# code above includes a method named `ReasonableStartTime`,
which checks the year of the `PromotionStart` field, throwing an exception if
the start time is determined to be too far in the past. Such validation methods
can be marked with the `Validation` attribute to indicate that they need to be
validated after processing XML; the `FromXmlNode` and `FromXmlDoc` methods
generated by the `FromXml` template will automatically call any of these
validation methods it finds. Validation methods are accepted as either member
methods which take no parameters, or as static methods that take an instance of
their parent type. Likewise, validation methods can either return `unit` (`()`),
or a `ValidationResult`. If returning `unit`, feel free to throw an exception,
as shown in the example above; if returning a `ValidationResult`, please put any
error message(s) into the `ValidationResult.Invalid` case.

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

This project is Copyright © 2016-2017 Anthony Perez a.k.a. amazingant, and is
licensed under the MIT license. See the [LICENSE file][license] for more
details.


[xml-provider]: https://fsharp.github.io/FSharp.Data/library/XmlProvider.html
[license]: https://github.com/amazingant/Amazingant.FSharp.TypeExpansion.Templates/blob/master/LICENSE
[drwpf]: http://drwpf.com/blog/2007/09/16/can-i-bind-my-itemscontrol-to-a-dictionary/