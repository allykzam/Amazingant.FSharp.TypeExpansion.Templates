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


License
-------

This project is Copyright © 2016 Anthony Perez a.k.a. amazingant, and is
licensed under the MIT license. See the [LICENSE file][license] for more
details.


[license]: https://github.com/amazingant/Amazingant.FSharp.TypeExpansion.Templates/blob/master/LICENSE
