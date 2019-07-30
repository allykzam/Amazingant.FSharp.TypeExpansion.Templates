#if INTERACTIVE
#r "WindowsBase"
#nowarn "0211"
#I "../../packages/NLog/lib/net45"
#I "../../../NLog/lib/net45"
#r "NLog.dll"
#endif

namespace Amazingant.FSharp.TypeExpansion.Templates.ImmutableViewModel


[<System.AttributeUsage(System.AttributeTargets.Class ||| System.AttributeTargets.Module)>]
/// Indicates that a module contains commands for the specific type, which is
/// going to be processed by the ImmutableViewModel type expansion template.
type HelperModuleAttribute(targetType : System.Type) =
    inherit System.Attribute()
    member __.TargetType with get () = targetType


[<System.AttributeUsage(System.AttributeTargets.Method)>]
/// <summary>
/// Indicates that a function can be used to get a collection of property
/// dependencies. Functions with this attribute should take a value of the
/// target data model type, and return one of three different results:
///
/// -  Expr * Expr
///
/// -  (Expr * Expr) list
///
/// -  Expr * (Expr list)
///
/// In each return type, the second `Expr` in the type signature defines the
/// property or properties that the first `Expr` depends upon, e.g. in the
/// following example, the "LoadDisplayProgress" property depends on the
/// "LoadProgress" value:
///
/// [&lt;PropertyDependencies&gt;] let LoadProgress (x : State) = (&lt;@@ x.LoadDisplayProgress @@&gt;, &lt;@@ x.LoadProgress @@&gt;)
///
/// Of the three possible return types, the first is used as in the above
/// sample, to indicate a single property depends upon a single value; the
/// second is used to indicate multiple dependencies at once; and the third is
/// used to indicate that a single property depends on multiple values. Note
/// that the effects of multiple dependency definitions is cumulative. If an
/// additional function were defined which indicated that the
/// "LoadDisplayProgress" property depended upon a "TotalCount" value, these two
/// dependency definitions would be combined, and the "LoadDisplayProgress"
/// property would be flagged as changed when either the "LoadProgress" or
/// "TotalCount" values changed.
/// </summary>
type PropertyDependenciesAttribute() =
    inherit System.Attribute()


[<System.AttributeUsageAttribute(System.AttributeTargets.Field ||| System.AttributeTargets.Property)>]
/// Indicates that the specified field or property is to be used as an
/// identifier for comparison when a view model is nested within a map in
/// another view model. Assume types 'A and 'B will have immutable view models
/// generated for them, and type 'A contains a field of type `Map<int, 'B>`.
/// Without this attribute, updates to the map field will cause WPF to be told
/// that a 'B view model was removed, and a new one was inserted in its place.
/// However, if this attribute is applied to a field or property on the 'B type,
/// when the flagged field or property matches on old and new values, the value
/// will be properly "updated" as far as WPF is concerned. This will prevent
/// e.g. a ListView or TreeView from clearing its selection when modifying
/// values on the selected value.
type ObservableCollectionKeyAttribute() =
    inherit System.Attribute()


[<System.AttributeUsageAttribute(System.AttributeTargets.Field)>]
type ReadOnlyAttribute() =
    inherit System.Attribute()


/// Helper for holding an internal state and whether or not processing is
/// currently being done on it.
type State<'T> =
    | Available of 'T
    | Processing of 'T



/// A view model which contains an immutable data type, and provides support for
/// acting as though that data type is mutable, primarily for use with WPF
/// applications.
type IImmutableViewModelBase<'T> =
    /// <summary>
    /// Applies the given function to the current immutable state value, and
    /// replaces the state with the result. Any modified fields are reported via
    /// <see cref="System.ComponentModel.INotifyPropertyChanged" />.
    /// </summary>
    abstract member UpdateField : updater:('T -> 'T) -> unit
    /// <summary>
    /// Applies the given asynchronous function to the current immutable state
    /// value, and replaces the state with the result. Any modified fields are
    /// reported via
    /// <see cref="System.ComponentModel.INotifyPropertyChanged" />.
    /// </summary>
    /// <remarks>
    /// While the <paramref name="updater" /> function is running, the internal
    /// immutable state will be marked as being processed, and
    /// <see cref="IsProcessing" /> will become true. During this time, it is
    /// not possible to make any changes to any fields.
    /// </remarks>
    abstract member AsyncUpdate : updater:('T -> Async<'T>) -> unit
    /// Returns the current immutable state value.
    abstract member CurrentValue : 'T
    /// Indicates whether or not an asynchronous update is in progress.
    abstract member IsProcessing : bool
    /// Builds a read-only observable collection for the specified field.
    abstract member MakeObservableList<'a> : fieldName:string * startData:list<'a> -> System.Collections.ObjectModel.ReadOnlyObservableCollection<'a>
    /// Builds a read-only observable collection for the specified field.
    abstract member MakeObservableList<'a, 'b when 'a : equality and 'b : equality> : fieldName:string * startData:list<'a> * getKey:('a -> 'b) -> System.Collections.ObjectModel.ReadOnlyObservableCollection<'a>
    abstract member MakeObservableSet<'a when 'a : comparison> : fieldName:string * startData:Set<'a> -> System.Collections.ObjectModel.ReadOnlyObservableCollection<'a>
    /// When the specified field is modified, calls the given update function
    /// to keep an observable dictionary up-to-date with the backing map.
    abstract member TrackObservableDictionary : fieldName:string * updateValues:(obj -> unit) -> unit
    /// <summary>
    /// Builds a command for WPF binding, which takes no additional parameters,
    /// and is always able to execute, so long as the
    /// <see cref="IsProcessing" /> property is false.
    /// </summary>
    abstract member MakeCommand      : name:string * behavior:(                'T       ->       'T )                                 -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes an additional parameter,
    /// and is always able to execute, so long as the
    /// <see cref="IsProcessing" /> property is false.
    /// </summary>
    abstract member MakeCommand      : name:string * behavior:(                'T -> 'a ->       'T )                                 -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes no additional parameters,
    /// but may not always be able to execute. Execution is automatically
    /// disabled whenever the <see cref="IsProcessing" /> property is true.
    /// </summary>
    abstract member MakeCommand      : name:string * behavior:(                'T       ->       'T ) * canExecute:('T       -> bool) -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes an additional parameter,
    /// and may not always be able to execute. Execution is automatically
    /// disabled whenever the <see cref="IsProcessing" /> property is true.
    /// </summary>
    abstract member MakeCommand      : name:string * behavior:(                'T -> 'a ->       'T ) * canExecute:('T -> 'a -> bool) -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes no additional parameters,
    /// and performs its behavior asynchronously. Execution of other commands is
    /// disabled while this command runs, and execution of this command is
    /// automatically disabled whenever the <see cref="IsProcessing" /> property
    /// is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(                'T       -> Async<'T>)                                 -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes an additional parameter,
    /// and performs its behavior asynchronously. Execution of other commands is
    /// disabled while this command runs, and execution of this command is
    /// automatically disabled whenever the <see cref="IsProcessing" /> property
    /// is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(                'T -> 'a -> Async<'T>)                                 -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes no additional parameters,
    /// and performs its behavior asynchronously, but may not always be able to
    /// execute. Execution of other commands is disabled while this command
    /// runs, and execution of this command is automatically disabled whenever
    /// the <see cref="IsProcessing" /> property is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(                'T       -> Async<'T>) * canExecute:('T       -> bool) -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes an additional parameter,
    /// and performs its behavior asynchronously, but may not always be able to
    /// execute. Execution of other commands is disabled while this command
    /// runs, and execution of this command is automatically disabled whenever
    /// the <see cref="IsProcessing" /> property is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(                'T -> 'a -> Async<'T>) * canExecute:('T -> 'a -> bool) -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes no additional parameters,
    /// performs its behavior asynchronously, and may provide status updates.
    /// Execution of other commands is disabled while this command runs, and
    /// execution of this command is automatically disabled whenever the
    /// <see cref="IsProcessing" /> property is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(('T -> unit) -> 'T       -> Async<'T>)                                 -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes an additional parameter,
    /// performs its behavior asynchronously, and may provide status updates.
    /// Execution of other commands is disabled while this command runs, and
    /// execution of this command is automatically disabled whenever the
    /// <see cref="IsProcessing" /> property is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(('T -> unit) -> 'T -> 'a -> Async<'T>)                                 -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes no additional parameters,
    /// performs its behavior asynchronously, and may provide status updates,
    /// but may not always be able to execute. Execution of other commands is
    /// disabled while this command runs, and execution of this command is
    /// automatically disabled whenever the <see cref="IsProcessing" /> property
    /// is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(('T -> unit) -> 'T       -> Async<'T>) * canExecute:('T       -> bool) -> System.Windows.Input.ICommand
    /// <summary>
    /// Builds a command for WPF binding, which takes an additional parameter,
    /// performs its behavior asynchronously, and may provide status updates,
    /// but may not always be able to execute. Execution of other commands is
    /// disabled while this command runs, and execution of this command is
    /// automatically disabled whenever the <see cref="IsProcessing" /> property
    /// is true.
    /// </summary>
    abstract member MakeAsyncCommand : name:string * behavior:(('T -> unit) -> 'T -> 'a -> Async<'T>) * canExecute:('T -> 'a -> bool) -> System.Windows.Input.ICommand
    /// <summary>
    /// Tracks a new dependency, such that whenever
    /// <paramref name="dependsOn" /> updates, <paramref name="name" /> will
    /// also be flagged as having changed.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to be used for commands whose ability to
    /// execute depends on the values of certain properties.
    /// </remarks>
    abstract member AddDependency : name:string * dependsOn:string -> unit
    /// <summary>
    /// Tracks a new dependency, such that whenever
    /// <paramref name="dependsOn" /> updates, <paramref name="name" /> will
    /// also be flagged as having changed.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to be used for commands whose ability to
    /// execute depends on the values of certain properties.
    /// </remarks>
    abstract member AddDependency : name:Microsoft.FSharp.Quotations.Expr * dependsOn:Microsoft.FSharp.Quotations.Expr -> unit
    /// <summary>
    /// Tracks a new dependency, such that whenever
    /// <paramref name="dependsOn" /> updates, <paramref name="name" /> will
    /// also be flagged as having changed.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to be used for commands whose ability to
    /// execute depends on the values of certain properties.
    /// </remarks>
    abstract member AddDependency : name:Microsoft.FSharp.Quotations.Expr<_> * dependsOn:Microsoft.FSharp.Quotations.Expr<_> -> unit
    /// <summary>
    /// Tracks new dependencies, such that whenever any of the values specified
    /// in <paramref name="dependsOn" /> updates, <paramref name="name" /> will
    /// also be flagged as having changed.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to be used for commands whose ability to
    /// execute depends on the values of certain properties.
    /// </remarks>
    abstract member AddDependencies : name:string * dependsOn:string list -> unit
    /// <summary>
    /// Tracks new dependencies, such that whenever any of the values specified
    /// in <paramref name="dependsOn" /> updates, <paramref name="name" /> will
    /// also be flagged as having changed.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to be used for commands whose ability to
    /// execute depends on the values of certain properties.
    /// </remarks>
    abstract member AddDependencies : name:Microsoft.FSharp.Quotations.Expr * dependsOn:Microsoft.FSharp.Quotations.Expr list -> unit
    /// <summary>
    /// Tracks new dependencies, such that whenever any of the values specified
    /// in <paramref name="dependsOn" /> updates, <paramref name="name" /> will
    /// also be flagged as having changed.
    /// </summary>
    /// <remarks>
    /// This is primarily intended to be used for commands whose ability to
    /// execute depends on the values of certain properties.
    /// </remarks>
    abstract member AddDependencies : name:Microsoft.FSharp.Quotations.Expr<_> * dependsOn:Microsoft.FSharp.Quotations.Expr<_> list -> unit



/// <summary>
/// Helper for indicating that the result of
/// <see cref="System.Windows.Input.ICommand.CanExecute" /> has changed.
/// </summary>
type ICanExecuteChange =
    abstract member RaiseCanExecuteChanged : unit -> unit



/// <summary>
/// A command that allows operations on an immutable data type by replacing a
/// value rather than modifying it.
/// </summary>
/// <param name="action">
/// A function which takes a value of type <typeparamref name="T" />, and an
/// additional parameter, and returns an updated <typeparamref name="T" />
/// value.
/// </param>
/// <param name="canExecute">
/// A function which indicates whether the command can be executed with the
/// current <typeparamref name="T" /> and parameter values.
/// </param>
/// <param name="getCurrent">
/// A function which returns the current <typeparamref name="T" /> state.
/// </param>
/// <param name="finish">
/// A function which "finishes" processing the resulting
/// <typeparamref name="T" /> value after this command has executed. This should
/// be a call to <see cref="IImmutableViewModel.UpdateField" /> in the parent
/// view model.
/// </param>
/// <remarks>
/// The internals of this type assumes that the specified parameter value can be
/// passed as null. If your functionality expects a non-nullable type, such as a
/// record or union type, be sure to check for nulls with e.g.
/// <c>isNull (box x)</c>.
/// </remarks>
type ImmutableCommand<'T, 'a>(action : 'T -> 'a -> 'T, canExecute : 'T -> 'a -> bool, getCurrent : unit -> State<'T>, finish : 'T -> unit) =
    let canExecuteChanged = new Event<_,_>()

    interface ICanExecuteChange with
        member this.RaiseCanExecuteChanged() = canExecuteChanged.Trigger(this, System.EventArgs())

    interface System.Windows.Input.ICommand with
        member __.CanExecute(x : obj) =
            match x with
            | null | :? 'a ->
                match getCurrent() with
                | Available current -> canExecute current (unbox x)
                | Processing _ -> false
            | _ -> failwithf "Invalid parameter value %A\nExpected a value of type %A" x typeof<'a>.FullName

        member __.Execute(x : obj) =
            match x with
            | null | :? 'a ->
                match getCurrent() with
                | Available current ->
                    match canExecute current (unbox x) with
                    | true -> action current (unbox x) |> finish
                    | false -> ()
                | Processing _ -> ()
            | _ -> failwithf "Invalid parameter value %A\nExpected a value of type %A" x typeof<'a>.FullName

        [<CLIEvent>]
        member __.CanExecuteChanged = canExecuteChanged.Publish


/// <summary>
/// An asynchronous command that allows operations on an immutable data type by
/// replacing a value rather than modifying it.
/// </summary>
/// <param name="action">
/// An async function which takes a "status update" function, a value of type
/// <typeparamref name="T" />, and an additional parameter, and asynchronously
/// returns an updated <typeparamref name="T" /> value.
/// </param>
/// <param name="canExecute">
/// A function which indicates whether the command can be executed with the
/// current <typeparamref name="T" /> and parameter values.
/// </param>
/// <param name="getCurrent">
/// A function which returns the current <typeparamref name="T" /> state.
/// </param>
/// <param name="updateCurrent">
/// A function which updates the <typeparamref name="T" /> value. This will be
/// used to initially flag the current value as "processing" when the command
/// starts running, and to set it to the finished value when the command
/// completes execution. Note that this function will be called asynchronously,
/// and should perform any dispatching required to return to the proper thread.
/// </param>
/// <remarks>
/// The internals of this type assumes that the specified parameter value can be
/// passed as null. If your functionality expects a non-nullable type, such as a
/// record or union type, be sure to check for nulls with e.g.
/// <c>isNull (box x)</c>.
/// </remarks>
type ImmutableAsyncCommand<'T, 'a>(action : ('T -> unit) -> 'T -> 'a -> Async<'T>, canExecute : 'T -> 'a -> bool, getCurrent : unit -> State<'T>, updateCurrent : State<'T> -> unit) =
    let canExecuteChanged = new Event<_,_>()

    interface ICanExecuteChange with
        member this.RaiseCanExecuteChanged() = canExecuteChanged.Trigger(this, System.EventArgs())

    interface System.Windows.Input.ICommand with
        member __.CanExecute(x : obj) =
            match x with
            | null | :? 'a ->
                match getCurrent() with
                | Available current -> canExecute current (unbox x)
                | Processing _ -> false
            | _ -> failwithf "Invalid parameter value %A\nExpected a value of type %A" x typeof<'a>.FullName

        member __.Execute(x : obj) =
            match x with
            | null | :? 'a ->
                match getCurrent() with
                | Available current ->
                    match canExecute current (unbox x) with
                    | true ->
                        updateCurrent (Processing current)
                        let f x = updateCurrent (Processing x)
                        try
                            async {
                                let! result = action f current (unbox x)
                                updateCurrent (Available result)
                                } |> Async.Start
                        with
                        | _ ->
                            updateCurrent (Available current)
                            reraise()
                    | false -> ()
                | Processing _ -> ()
            | _ -> failwithf "Invalid parameter value %A\nExpected a value of type %A" x typeof<'a>.FullName

        [<CLIEvent>]
        member __.CanExecuteChanged = canExecuteChanged.Publish



/// <summary>
/// A base type for view models which act as a mutable view model while wrapping
/// around an immutable data type. This allows code to be written with the
/// safety of working with an immutable data type, while allowing a WPF view to
/// treat the data as mutable.
/// </summary>
/// <param name="makeDefault>
/// A function which returns a default value of type <typeparamref name="T" />.
/// </param>
/// <param name="fields>
/// A list of field names, and functions which take a value of type
/// <typeparamref name="T" /> and return the corresponding field value.
/// </param>
/// <remarks>
/// For safety, internal values cannot be modified by any thread other than the
/// thread which created the instance of <see cref="ImmutableViewModel" />.
/// Attempts to do so will result in a <see cref="System.InvalidOperation" />
/// exception being thrown. Please use an appropriate dispatcher to make changes
/// from your UI thread.
///
/// Commands which use an asynchronous function are able to bypass the
/// cross-threading limitation, but will cause the internal state to be marked
/// as processing changes. No additional changes can be made while the
/// asynchronous function is running.
/// </remarks>
type ImmutableViewModelBase<'T when 'T : equality and 'T : comparison>(makeDefault : unit -> 'T, fields : (string * ('T -> System.IComparable)) list) as self =
    let propertyChangedEvent = new Event<_,_>()
    let mutable internalValue = makeDefault() |> Available
    // For each observable collection, keep a function which takes the updated
    // list and applies it to that observable collection.
    let mutable observableCollections : Map<string, (obj -> unit)> = Map.empty
    // Same thing for observable maps.
    let mutable observableMaps : Map<string, (obj -> unit)> = Map.empty
    // For each command, keep a reference where we can say that its CanExecute
    // has changed.
    let mutable commands : Map<string, ICanExecuteChange> = Map.empty
    let mutable dependencies : Map<string, string list> = Map.empty

    static let logger = NLog.LogManager.GetCurrentClassLogger()

    // Iterates over all of the fields in the old/new values to see which, if
    // any, have changed, then raises INotifyPropertyChanged.PropertyChanged so
    // that e.g. WPF will reload those fields.
    let checkModifiedFields (oldValue : 'T) (newValue : 'T) : unit =
        fields
        |> Seq.iter
            (fun (name, getter) ->
                let x = getter oldValue
                let y = getter newValue
                if x = y then () else
                    propertyChangedEvent.Trigger(self, System.ComponentModel.PropertyChangedEventArgs(name))
                    // If the field name is a known observable collection,
                    // update its contents with the new value.
                    match observableCollections.TryFind name with
                    | None -> ()
                    | Some xf -> xf y
                    // If the field name is a known observable map, update its
                    // contents with the new value.
                    match observableMaps.TryFind name with
                    | None -> ()
                    | Some xf -> xf y
                    // If any fields have been marked as dependent on this one,
                    // process them as well.
                    match dependencies.TryFind name with
                    | None -> ()
                    | Some xs ->
                        xs |> Seq.iter
                            (fun x ->
                                // If this dependency is a command, indicate
                                // that its CanExecute has changed.
                                match commands.TryFind x with
                                | None -> ()
                                | Some x -> x.RaiseCanExecuteChanged()
                                propertyChangedEvent.Trigger(self, System.ComponentModel.PropertyChangedEventArgs(x))
                            )
            )

    let dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher
    #if TYPE_EXPANSION
    let dispatch (f : unit -> 'a) : 'a = f()
    #else
    let dispatch f =
        if dispatcher.CheckAccess() then f() else
        try
            dispatcher.Invoke(fun _ -> f())
        with
        | :? System.Threading.Tasks.TaskCanceledException -> ()
    #endif

    let updateCommandsCanExec() = commands |> Map.toSeq |> Seq.iter (fun (_, x) -> x.RaiseCanExecuteChanged())

    let updateField (updater : 'T -> 'T) =
        match dispatcher.CheckAccess() with
        | false ->
            logger.Warn("Asked to modify the internal state from a thread other than the one where this view model was created.")
        | true ->
            match internalValue with
            | Available x ->
                let newValue = updater x
                internalValue <- Available newValue
                if newValue = x then () else checkModifiedFields x newValue
            | Processing _ ->
                logger.Warn("Asked to modify the internal state while an asynchronous command is running.")

    let buildCommand name action canExec =
        let wrappedAction x y =
            let oldState = internalValue
            let oldValue =
                match oldState with
                | Available x -> x
                | Processing x -> x
            internalValue <- Processing oldValue
            let newValue = action x y
            internalValue <- Available oldValue
            newValue
        let command =
            ImmutableCommand(
                wrappedAction, canExec,
                (fun () -> internalValue),
                (fun x -> updateField (fun _ -> x))
                )
        commands <- commands |> Map.add name (command :> ICanExecuteChange)
        command :> System.Windows.Input.ICommand

    let buildAsyncCommand name action canExec =
        let mutable asyncUpdate : State<'T> -> unit = fun _ -> ()
        asyncUpdate <-
            fun newState ->
                match dispatcher.CheckAccess() with
                | false -> dispatch (fun _ -> asyncUpdate newState)
                | true ->
                    let oldState = internalValue
                    let newValue =
                        match newState with
                        | Available x -> x
                        | Processing x -> x
                    let oldValue =
                        match oldState with
                        | Available x -> x
                        | Processing x -> x
                    internalValue <- newState
                    if newValue = oldValue then () else checkModifiedFields oldValue newValue
                    match newState, oldState with
                    | Available _, Processing _ -> updateCommandsCanExec()
                    | Processing _, Available _ -> updateCommandsCanExec()
                    | _ -> ()
        let command =
            ImmutableAsyncCommand(
                action, canExec,
                (fun () -> internalValue),
                asyncUpdate
                )
        commands <- commands |> Map.add name (command :> ICanExecuteChange)
        command :> System.Windows.Input.ICommand

    let addDependency name dependsOn =
        let updated =
            match dependencies.TryFind dependsOn with
            | None -> [name]
            | Some xs -> name::xs
        dependencies <- dependencies |> Map.add dependsOn updated

    let rec quotationToName x =
        match x with
        | Microsoft.FSharp.Quotations.Patterns.PropertyGet (_, x, _) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.FieldGet    (_, x   ) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.Call        (_, x, _) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.Var         (   x   ) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.Lambda      (_, x   ) -> quotationToName x
        | _ -> failwithf "Could not get a name from %A" x

    interface IImmutableViewModelBase<'T> with
        member __.UpdateField updater = updateField updater

        member __.AsyncUpdate (updater : 'T -> Async<'T>) =
            match dispatcher.CheckAccess() with
            | false ->
                logger.Warn("Asked to start an asynchronous command from a thread other than the one where this view model was created.")
            | true ->
                match internalValue with
                | Available x ->
                    internalValue <- Processing x
                    updateCommandsCanExec()
                    Async.StartWithContinuations(
                        updater x,
                        (fun y ->
                            internalValue <- Available y
                            if x = y then () else
                                dispatch
                                    (fun () ->
                                        checkModifiedFields x y
                                        updateCommandsCanExec()
                                    )
                        ),
                        (fun ex ->
                            internalValue <- Available x
                            printfn "%A" ex
                        ),
                        (fun cancelledEx ->
                            internalValue <- Available x
                            printfn "%A" cancelledEx
                        ),
                        ((new System.Threading.CancellationTokenSource()).Token)
                        )
                | Processing _ ->
                    logger.Warn("Asked to start an asynchronous command while another one is already running.")

        member __.CurrentValue
            with get () =
                match internalValue with
                | Available x -> x
                | Processing x -> x

        member __.IsProcessing
            with get () =
                match internalValue with
                | Available _ -> false
                | Processing _ -> true

        member __.MakeObservableList<'a> (fieldName, startData) =
            let observable = System.Collections.ObjectModel.ObservableCollection<'a>(startData)
            let newF (x : obj) =
                match x with
                | :? ('a list) as xs ->
                    observable.Clear()
                    xs |> Seq.iter observable.Add
                | _ -> ()
            observableCollections <- observableCollections |> Map.add fieldName newF
            System.Collections.ObjectModel.ReadOnlyObservableCollection<'a>(observable)

        member __.MakeObservableList<'a, 'b when 'a : equality and 'b : equality> (fieldName, startData, getKey) =
            let _ : 'a -> 'b = getKey
            let observable = System.Collections.ObjectModel.ObservableCollection<'a>(startData)
            let newF (y : obj) =
                match y with
                | :? ('a list) as ys ->
                    if ys.IsEmpty then observable.Clear() else
                    observable
                    |> Seq.toArray
                    |> Seq.iter
                        (fun key ->
                            match ys |> List.tryFindIndex (fun y -> (getKey key) = (getKey y)) with
                            | None -> key |> observable.Remove |> ignore
                            | Some yi when ys.[yi] = key -> ()
                            | Some yi ->
                                let xi = observable |> Seq.findIndex (fun x -> (getKey x) = (getKey key))
                                observable.[xi] <- ys.[yi]
                        )
                    ys
                    |> Seq.iteri
                        (fun i y ->
                            if observable.[i] <> y then
                                observable.Insert(i, y)
                        )
                | _ -> ()
            observableCollections <- observableCollections |> Map.add fieldName newF
            System.Collections.ObjectModel.ReadOnlyObservableCollection<'a>(observable)


        member __.MakeObservableSet<'a when 'a : comparison> (fieldName, (startData : Set<'a>)) =
            let observable = System.Collections.ObjectModel.ObservableCollection<'a>(startData)
            let newF (x : obj) =
                match x with
                | :? (Set<'a>) as xs ->
                    if xs.IsEmpty then observable.Clear() else
                    if observable.Count = 0 then xs |> Seq.iter observable.Add else
                    let ys = Set.ofSeq observable
                    (ys - xs) |> Set.iter (observable.Remove >> ignore)
                    xs
                    |> Seq.iteri
                        (fun i x ->
                            if observable.[i] <> x
                            then observable.Insert(i, x)
                        )
                | _ -> ()
            observableCollections <- observableCollections |> Map.add fieldName newF
            System.Collections.ObjectModel.ReadOnlyObservableCollection<'a>(observable)


        member __.TrackObservableDictionary(name, updateValues) =
            observableMaps <- observableMaps |> Map.add name updateValues


        member __.MakeCommand((name : string), (behavior : 'T -> 'T)) =
            buildCommand name
                (fun x _ -> behavior x)
                (fun _ _ -> true)

        member __.MakeCommand((name : string), (behavior : 'T -> 'T), (canExecute : 'T -> bool)) =
            buildCommand name
                (fun x _ -> behavior x)
                (fun x _ -> canExecute x)

        member __.MakeCommand((name : string), (behavior : 'T -> 'a -> 'T)) =
            buildCommand name
                behavior
                (fun _ _ -> true)

        member __.MakeCommand((name : string), (behavior : 'T -> 'a -> 'T), (canExecute : 'T -> 'a -> bool)) =
            buildCommand name
                behavior
                canExecute

        member __.MakeAsyncCommand((name : string), (behavior : 'T -> Async<'T>)) =
            buildAsyncCommand name
                (fun _ x _ -> behavior x)
                (fun _ _ -> true)

        member __.MakeAsyncCommand((name : string), (behavior : 'T -> Async<'T>), (canExecute : 'T -> bool)) =
            buildAsyncCommand name
                (fun _ x _ -> behavior x)
                (fun x _ -> canExecute x)

        member __.MakeAsyncCommand((name : string), (behavior : 'T -> 'a -> Async<'T>)) =
            buildAsyncCommand name
                (fun _ x y -> behavior x y)
                (fun _ _ -> true)

        member __.MakeAsyncCommand((name : string), (behavior : 'T -> 'a -> Async<'T>), (canExecute : 'T -> 'a -> bool)) =
            buildAsyncCommand name
                (fun _ x y -> behavior x y)
                canExecute

        member __.MakeAsyncCommand((name : string), (behavior : ('T -> unit) -> 'T -> Async<'T>)) =
            buildAsyncCommand name
                (fun x y _ -> behavior x y)
                (fun _ _ -> true)

        member __.MakeAsyncCommand((name : string), (behavior : ('T -> unit) -> 'T -> 'a -> Async<'T>)) =
            buildAsyncCommand name
                behavior
                (fun _ _ -> true)

        member __.MakeAsyncCommand((name : string), (behavior : ('T -> unit) -> 'T -> Async<'T>), (canExecute : 'T -> bool)) =
            buildAsyncCommand name
                (fun x y _ -> behavior x y)
                (fun x _ -> canExecute x)

        member __.MakeAsyncCommand((name : string), (behavior : ('T -> unit) -> 'T -> 'a -> Async<'T>), (canExecute : 'T -> 'a -> bool)) =
            buildAsyncCommand name
                behavior
                canExecute

        member __.AddDependency(thing, dependsOn) = addDependency thing dependsOn

        member __.AddDependency(thing, dependsOn) =
            addDependency (quotationToName thing) (quotationToName dependsOn)

        member __.AddDependency(thing : Quotations.Expr<_>, dependsOn : Quotations.Expr<_>) =
            addDependency (quotationToName thing) (quotationToName dependsOn)

        member __.AddDependencies(thing, dependsOn) =
            dependsOn |> List.iter (fun x -> addDependency thing x)

        member __.AddDependencies(thing, dependsOn:Quotations.Expr list) =
            let thingName = quotationToName thing
            dependsOn |> List.iter (quotationToName >> (addDependency thingName))

        member __.AddDependencies(thing : Quotations.Expr<_>, dependsOn:Quotations.Expr<_> list) =
            let thingName = quotationToName thing
            dependsOn |> List.iter (quotationToName >> (addDependency thingName))


    interface System.ComponentModel.INotifyPropertyChanged with
        [<CLIEvent>]
        member __.PropertyChanged with get () = propertyChangedEvent.Publish
