namespace Amazingant.FSharp.TypeExpansion.Templates.Lenses

/// <summary>
/// Provides a lens for the <typeparamref name="'a" /> type, where each
/// function requires a value of type <typeparamref name="'a" /> in order to
/// function.
/// </summary>
type StaticLens<'a,'b> =
    {
        /// A getter for the target field
        Get : 'a -> 'b;
        /// A setter that updates the target field and returns the new container
        Set : 'b -> 'a -> 'a;
    }
    /// Applies the given function to the target field in the given container,
    /// and returns the new container
    member self.Map func x =
        let value = x |> (self.Get >> func)
        self.Set value x


/// <summary>
/// Provides a lens for the <typeparamref name="'a" /> type which operates on a
/// known value of type <typeparamref name="'a" />, so that a
/// <typeparamref name="'a" /> value does not need to be provided for each
/// function to operate.
/// </summary>
type Lens<'a,'b> =
    {
        /// A getter for the target field
        Get : unit -> 'b;
        /// A setter that updates the target field and returns the new container
        Set : 'b -> 'a;
        /// <summary>
        /// The value of <typeparamref name="'a" /> which this lens operates on
        /// </summary>
        Nil : 'a;
    }
    /// <summary>
    /// Applies the given function to the target field in the current value of
    /// <typeparamref name="'a" />, and returns the updated
    /// <typeparamref name="'a" /> value
    /// </summary>
    member self.Map func = self.Get() |> func |> self.Set
    /// <summary>
    /// Creates a <see cref="Lens" /> for the given value of
    /// <typeparamref name="'a" /> which uses the contents of the given
    /// <see cref="StaticLens" />.
    /// </summary>
    static member FromStatic (x : 'a) (l : StaticLens<'a,'b>) : Lens<'a,'b> =
        {
            Get = (fun () -> l.Get x);
            Set = (fun v -> l.Set v x);
            Nil = x;
        }


/// Attribute for flagging a type as needing lenses
type LensAttribute() =
    inherit System.Attribute()

[<AutoOpen>]
module LensHelpers =
    /// Helper for creating a static lens from the given getter and setter
    let MakeStaticLens g s : StaticLens<'a,'b> = { Get = g; Set = s; }
    /// Composes two static lenses
    let Compose (one : StaticLens<'a,'b>) (two : StaticLens<'b,'c>) : StaticLens<'a,'c> =
        MakeStaticLens
            (one.Get >> two.Get)
            (fun c a -> one.Set (two.Set c (one.Get a)) a)
    /// Helper for creating a lens for the given container using the given
    /// getter and setter
    let MakeLens x g s : Lens<'a,'b> =
        {
            Get = (fun () -> g x);
            Set = (fun v -> s v x);
            Nil = x;
        }
