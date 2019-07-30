#if INTERACTIVE
// Load the TypeExpansion.Attributes library from one of these two relative
// paths in fsi and Visual Studio's script editor; note that error FS0211 warns
// us that one of the specified paths does not exist, which should always be the
// case for one of the two paths specified.
#nowarn "0211"
#I "../../packages/Amazingant.FSharp.TypeExpansion/lib/net45"
#I "../../../Amazingant.FSharp.TypeExpansion/lib/net45"
#I "../../Amazingant.FSharp.TypeExpansion.Templates/packages/Amazingant.FSharp.TypeExpansion/lib/net45"
#r "Amazingant.FSharp.TypeExpansion.Attributes.dll"

#load "ImmutableViewModel_Base.fsx"
#endif

namespace Amazingant.FSharp.TypeExpansion.Templates.ImmutableViewModel

open Amazingant.FSharp.TypeExpansion.Attributes


module Expander =
    let getFieldNamesAndFunctions (props : System.Reflection.PropertyInfo array) : string =
        match props with
        | [||] -> ""
        | xs ->
            xs
            |> Seq.map
                (fun x ->
                    sprintf "\t\t\t(%A, (fun x -> x.``%s`` :> System.IComparable));"
                        x.Name x.Name
                )
            |> fun xs -> System.String.Join("\n", xs)

    let hasViewModel (t : System.Type) =
        let xs = t.GetCustomAttributes(typeof<ExpandableTypeAttribute>, true)
        match xs with
        | [| x |] ->
            match x with
            | :? ExpandableTypeAttribute as x -> x.CanUseTemplate(Some "ImmutableViewModel", true)
            | _ -> false
        | _ -> false

    let getKeyField (t : System.Type) =
        t.GetProperties()
        |> Seq.filter
            (fun x ->
                x.GetCustomAttributes(typeof<ObservableCollectionKeyAttribute>, true)
                |> Seq.tryHead
                |> Option.isSome
            )
        |> Seq.tryHead

    let getTypeName (t : System.Type) =
        match t.DeclaringType with
        | null -> t.Namespace + "." + t.Name
        | _ -> t.DeclaringType.FullName + "." + t.Name

    let getTypeNameWithViewModel (t : System.Type) =
        let baseName = getTypeName t
        match hasViewModel t with
        | true  -> baseName + "ViewModel"
        | false -> baseName

    let handleObservableMap (x : System.Reflection.PropertyInfo) : string =
        let rec makeNonObservableType (t : System.Type) =
            match t.IsGenericType with
            | true ->
                let generic = t.GetGenericTypeDefinition()
                if generic = typeof<Map<int, int>>.GetGenericTypeDefinition() then
                    let args = t.GetGenericArguments()
                    let x = args.[0]
                    let y = args.[1]
                    sprintf "Map<%s, %s>"
                        (makeNonObservableType x) (makeNonObservableType y)
                elif generic = typeof<int list>.GetGenericTypeDefinition() then
                    t.GetGenericArguments()
                    |> Array.head
                    |> makeNonObservableType
                    |> sprintf "list<%s>"
                else
                    t.GetGenericArguments()
                    |> Seq.map makeNonObservableType
                    |> fun xs -> System.String.Join(", ", xs)
                    |> sprintf "%s<%s>" (getTypeName t)

            | _ -> getTypeName t

        let rec makeObservableType (t : System.Type) =
            match t.IsGenericType with
            | true ->
                let generic = t.GetGenericTypeDefinition()
                if generic = typeof<Map<int, int>>.GetGenericTypeDefinition() then
                    let args = t.GetGenericArguments()
                    let x = args.[0]
                    let y = args.[1]
                    let nestedY = makeObservableType y
                    sprintf "DrWPF.Windows.Data.ObservableDictionary<%s, %s>"
                        (makeNonObservableType x) nestedY
                elif generic = typeof<int list>.GetGenericTypeDefinition() then
                    let args = t.GetGenericArguments()
                    let x = args.[0]
                    sprintf "System.Collections.ObjectModel.ObservableCollection<%s>"
                        (makeObservableType x)
                else
                    t.GetGenericArguments()
                    |> Seq.map makeNonObservableType
                    |> fun xs -> System.String.Join(", ", xs)
                    |> sprintf "%s<%s>" (getTypeName t)

            | false -> getTypeNameWithViewModel t


        let rec removeValues (t : System.Type) depth =
            match t.IsGenericType with
            | true ->
                let generic = t.GetGenericTypeDefinition()
                if generic = typeof<Map<int, int>>.GetGenericTypeDefinition() then
                    let nestedType = Array.last(t.GetGenericArguments())
                    let baseIndent = System.String('\t', depth * 3)
                    sprintf "xs.Keys\n%s|> Seq.toArray\n%s|> Seq.iter\n%s\t(fun key ->\n%s\t\tif ys.IsEmpty then xs.Clear()\n%s\t\telif not (ys.ContainsKey key)\n%s\t\tthen key |> xs.Remove |> ignore\n%s\t\telse\n%s\t\t\tlet _oldXs = xs\n%s\t\t\tlet xs = xs.[key]\n%s\t\t\tlet ys = ys.[key]\n%s\t\t\t%s\n%s\t)"
                        baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent (removeValues nestedType (depth + 1)) baseIndent
                elif generic = typeof<int list>.GetGenericTypeDefinition() then
                    let baseIndent = System.String('\t', depth * 3)
                    let nestedType = Array.last(t.GetGenericArguments())
                    match hasViewModel nestedType, getKeyField nestedType with
                    | true, Some p ->
                        sprintf "xs\n%s|> Seq.toArray\n%s|> Seq.iter\n%s\t(fun key ->\n%s\t\tif ys.IsEmpty then xs.Clear() else\n%s\t\tmatch ys |> List.tryFindIndex (fun y -> y.%s = key.%s) with\n%s\t\t| None -> key |> xs.Remove |> ignore\n%s\t\t| Some yi when ys.[yi] = key.InternalState_CurrentValue -> ()\n%s\t\t| Some yi ->\n%s\t\t\tlet xi = xs |> Seq.findIndex (fun x -> x.%s = key.%s)\n%s\t\t\t(xs.[xi] :> Amazingant.FSharp.TypeExpansion.Templates.ImmutableViewModel.IImmutableViewModelBase<_>).UpdateField(fun _ -> ys.[yi])\n%s\t)"
                            baseIndent baseIndent baseIndent baseIndent baseIndent p.Name p.Name baseIndent baseIndent baseIndent baseIndent p.Name p.Name baseIndent baseIndent
                    | _ ->
                        sprintf "xs\n%s|> Seq.toArray\n%s|> Seq.iter\n%s\t(fun key ->\n%s\t\tif not (List.contains key ys)\n%s\t\tthen key |> xs.Remove |> ignore\n%s\t)"
                            baseIndent baseIndent baseIndent baseIndent baseIndent baseIndent
                else "if xs <> ys then key |> _oldXs.Remove |> ignore"
            | _ ->
                if not (hasViewModel t) then
                    "if xs <> ys then key |> _oldXs.Remove |> ignore"
                else
                    match getKeyField t with
                    | Some p ->
                        let baseIndent = System.String('\t', depth * 3)
                        sprintf "// Since a \"key\" field has been flagged, check it. If it differs, update the view model's state, rather than replacing the entire view model.\n%sif xs.%s <> ys.%s then\n%s\t(xs :> Amazingant.FSharp.TypeExpansion.Templates.ImmutableViewModel.IImmutableViewModelBase<_>).UpdateField(fun _ -> ys)"
                            baseIndent p.Name p.Name baseIndent
                    | None -> "if xs.InternalState_CurrentValue <> ys then key |> _oldXs.Remove |> ignore"

        let rec addValues (t : System.Type) depth =
            match t.IsGenericType with
            | true ->
                let generic = t.GetGenericTypeDefinition()
                if generic = typeof<Map<int, int>>.GetGenericTypeDefinition() then
                    let baseIndent = System.String('\t', (depth * 2) + 1)
                    let nestedType = t.GetGenericArguments().[1]
                    let valueCtor =
                        match nestedType.IsGenericType with
                        | true ->
                            let g = nestedType.GetGenericTypeDefinition()
                            if g = typeof<Map<int, int>>.GetGenericTypeDefinition() then "DrWPF.Windows.Data.ObservableDictionary()"
                            elif g = typeof<int list>.GetGenericTypeDefinition() then "System.Collections.ObjectModel.ObservableCollection()"
                            else "value"
                        | false ->
                            if hasViewModel nestedType
                            then (getTypeNameWithViewModel nestedType) + "(value)"
                            else "value"
                    sprintf "value\n%s|> Map.iter\n%s\t(fun key value ->\n%s\t\tif not (_xs.ContainsKey key) then\n%s\t\t\t_xs.Add(key, %s)\n%s\t\tlet _xs = _xs.[key]\n%s\t\t%s\n%s\t)"
                        baseIndent baseIndent baseIndent baseIndent valueCtor baseIndent baseIndent (addValues nestedType (depth + 1)) baseIndent
                elif generic = typeof<int list>.GetGenericTypeDefinition() then
                    let baseIndent = System.String('\t', (depth * 2) + 1)
                    let nestedType = Array.last(t.GetGenericArguments())
                    match hasViewModel nestedType, getKeyField nestedType with
                    | true, Some p ->
                        let nestedTypeName =
                            match nestedType.DeclaringType with
                            | null -> nestedType.Namespace + "." + nestedType.Name + "ViewModel"
                            | _ -> nestedType.DeclaringType.FullName + "." + nestedType.Name + "ViewModel"
                        sprintf "value\n%s|> Seq.iter\n%s\t(fun x ->\n%s\t\tif not (_xs |> Seq.exists (fun y -> y.%s = x.%s)) then\n%s\t\t\tx |> %s |> _xs.Add\n%s\t)"
                            baseIndent baseIndent baseIndent p.Name p.Name baseIndent nestedTypeName baseIndent
                    | _ ->
                        sprintf "value\n%s|> Seq.iter\n%s\t(fun x ->\n%s\t\tif not (_xs.Contains x) then\n%s\t\t\txs.Add x\n%s\t)"
                            baseIndent baseIndent baseIndent baseIndent baseIndent
                else "()"
            | _ -> "()"


        let observableType = makeObservableType x.PropertyType
        let mapType = makeNonObservableType x.PropertyType
        sprintf "\tlet ``%s_immutable_view_model_observable_map`` =\n\t\tlet xs = %s()\n\t\tlet _xs = xs\n\t\tlet removeValues (ys : %s) =\n\t\t\t%s\n\n\t\tlet addValues (ys : %s) =\n\t\t\tlet value = ys\n\t\t\t%s\n\n\t\tlet updateData (x : obj) =\n\t\t\tx |> unbox |> removeValues\n\t\t\tx |> unbox |> addValues\n\t\tupdateData ivm.CurrentValue.``%s``\n\t\tivm.TrackObservableDictionary(\"%s\", updateData)\n\t\txs"
            x.Name observableType mapType (removeValues x.PropertyType 1) mapType (addValues x.PropertyType 1) x.Name x.Name

    let getObservableLists (props : System.Reflection.PropertyInfo array) : string =
        match props with
        | [||] -> ""
        | xs ->
            xs
            |> Seq.choose
                (fun x ->
                    match x.PropertyType.IsGenericType with
                    | false -> None
                    | true ->
                        let generic = x.PropertyType.GetGenericTypeDefinition()
                        let innerType = Array.head(x.PropertyType.GetGenericArguments())
                        if generic = typeof<int list>.GetGenericTypeDefinition() then
                            match getKeyField innerType with
                            | Some p ->
                                sprintf "\tlet ``%s_immutable_view_model_observable_list`` = ivm.MakeObservableList<%s,_>(\"%s\", ivm.CurrentValue.``%s``, fun x -> x.%s)"
                                    x.Name (getTypeName innerType) x.Name x.Name p.Name
                                |> Some
                            | None ->
                                sprintf "\tlet ``%s_immutable_view_model_observable_list`` = ivm.MakeObservableList<%s>(\"%s\", ivm.CurrentValue.``%s``)"
                                    x.Name (getTypeName innerType) x.Name x.Name
                                |> Some
                        elif generic = typeof<Set<int>>.GetGenericTypeDefinition() then
                            sprintf "\tlet ``%s_immutable_view_model_observable_set`` = ivm.MakeObservableSet<%s>(\"%s\", ivm.CurrentValue.``%s``)"
                                x.Name (getTypeName innerType) x.Name x.Name
                            |> Some
                        elif generic = typeof<Map<int, int>>.GetGenericTypeDefinition() then
                            handleObservableMap x |> Some
                        else None
                )
            |> fun xs -> System.String.Join("\n", xs)

    let getCommands (t : System.Type) : (string list * string) =
        let asyncT = typeof<Async<int>>.GetGenericTypeDefinition().MakeGenericType([|t|])
        let statusUpdater = typeof<int -> unit>.GetGenericTypeDefinition().MakeGenericType([|t;typeof<unit>|])
        t.Assembly.GetTypes()
        |> Seq.filter (fun x -> x.IsPublic || x.IsNestedPublic)
        |> Seq.filter
            (fun x ->
                x.GetCustomAttributes(typeof<HelperModuleAttribute>, true)
                |> Seq.exists (fun x -> (x :?> HelperModuleAttribute).TargetType = t )
            )
        |> Seq.collect
            (fun x ->
                x.GetMethods()
                |> Seq.filter (fun x -> x.IsStatic)
                |> Seq.filter (fun x -> x.IsPublic)
                |> Seq.filter
                    (fun x ->
                        x.GetCustomAttributes(typeof<PropertyDependenciesAttribute>, true)
                        |> Seq.isEmpty
                    )
                |> Seq.choose
                    (fun exec ->
                        let nameLowered = exec.Name.ToLowerInvariant()
                        let f (x : System.Reflection.MethodInfo) = x.GetParameters() |> Seq.map (fun x -> x.ParameterType) |> Seq.toList
                        let execPs = f exec
                        let canExec = exec.DeclaringType.GetMethod(exec.Name + "_CanExec") |> Option.ofObj
                        let canExecRet = canExec |> Option.map (fun x -> x.ReturnType)
                        let canExecPs  = canExec |> Option.map f
                        match canExecPs, canExecRet, execPs, exec.ReturnType with
                        // When this function does not have a CanExec, and the
                        // function signature is either:
                        //     'T -> 'T
                        // or
                        //     'T -> 'a -> 'T
                        | _, None, [x], y | _, None, [x;_], y when x = t && y = t ->
                            let call =
                                sprintf "\tlet ``%s_immutable_view_model_command`` = ivm.MakeCommand(\"%s\", %s.``%s``)"
                                    nameLowered
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                            Some (exec.Name, call)

                        // When this function does have a CanExec, and the
                        // function signatures are:
                        //     'T -> 'T
                        // and
                        //     'T -> bool
                        | Some [cP], Some cR, [eP], eR when cP = t && eP = t && eR = t && cR = typeof<bool> ->
                            let call =
                                sprintf "\tlet ``%s_immutable_view_model_command`` = ivm.MakeCommand(\"%s\", %s.``%s``, %s.``%s_CanExec``)"
                                    nameLowered
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                            Some (exec.Name, call)

                        // When this function does have a CanExec, and the
                        // function signatures are:
                        //     'T -> 'a -> 'T
                        // and
                        //     'T -> 'a -> bool
                        | Some [cP;cP'], Some cR, [eP; eP'], eR when cP = t && eP = t && eR = t && cR = typeof<bool> && cP' = eP' ->
                            let call =
                                sprintf "\tlet ``%s_immutable_view_model_command`` = ivm.MakeCommand(\"%s\", %s.``%s``, %s.``%s_CanExec``)"
                                    nameLowered
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                            Some (exec.Name, call)

                        // When this function has a CanExec, but they take
                        // different argument types.
                        | Some [_;cP], _, [_;eP], _ when cP <> eP ->
                            failwithf "Found '%s.%s' and '%s.%s_CanExec' which should go together, but have conflicting parameter types. '%s' expects a %s, while '%s_CanExec' expects a %s."
                                exec.DeclaringType.FullName exec.Name
                                exec.DeclaringType.FullName exec.Name
                                exec.Name eP.FullName
                                exec.Name cP.FullName

                        // When this function has a CanExec, but they take
                        // different argument counts.
                        | Some cPs, _, ePs, _ when cPs.Length <> ePs.Length ->
                            failwithf "Found '%s.%s' and '%s.%s_CanExec' which should go together, but have conflicting parameter counts."
                                exec.DeclaringType.FullName exec.Name
                                exec.DeclaringType.FullName exec.Name

                        // When this function does not have a CanExec, and the
                        // function signature is either:
                        //     ('T -> unit) -> 'T -> Async<'T>
                        // or
                        //     ('T -> unit) -> 'T -> 'a -> Async<'T>
                        | _, None, [u;x], y | _, None, [u;x;_], y when x = t && y = asyncT && u = statusUpdater ->
                            let call =
                                sprintf "\tlet ``%s_immutable_view_model_command`` = ivm.MakeAsyncCommand(\"%s\", %s.``%s``)"
                                    nameLowered
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                            Some (exec.Name, call)

                        // When this function does not have a CanExec, and the
                        // function signature is either:
                        //     'T -> Async<'T>
                        // or
                        //     'T -> 'a -> Async<'T>
                        | _, None, [x], y | _, None, [x;_], y when x = t && y = asyncT ->
                            let call =
                                sprintf "\tlet ``%s_immutable_view_model_command`` = ivm.MakeAsyncCommand(\"%s\", %s.``%s``, %s.``%s_CanExec``)"
                                    nameLowered
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                                    exec.DeclaringType.FullName
                                    exec.Name
                            Some (exec.Name, call)

                        | _ -> None
                            //match canExecPs, canExecRet, execPs, exec.ReturnType with
                            //failwithf "canExecPs = %A; canExecRet = %A; execPs = %A; exec.ReturnType = %A"
                            //    canExecPs canExecRet execPs exec.ReturnType
                    )
            )
        |> Seq.toList
        |> fun xs ->
            let names = xs |> List.map fst
            let builders = System.String.Join("\n", xs |> List.map snd)
            (names, builders)

    let getProperties (t : System.Type) (props : System.Reflection.PropertyInfo array) (commands : string list) : string =
        let pNames = props |> Seq.map (fun x -> x.Name) |> Seq.groupBy id |> Seq.map (fun (x, xs) -> x, (xs |> Seq.toList)) |> Seq.toList
        let cNames = commands |> List.groupBy id
        let allNames = pNames |> List.map snd |> List.collect id |> List.append commands |> List.groupBy id
        match pNames.Length, cNames.Length, allNames.Length with
        | x, _, _ when x <> props.Length ->
            pNames
            |> List.filter (fun (_, xs) -> xs.Length > 1)
            |> List.map fst
            |> failwithf "Target type '%s' contains multiple fields with the same name(s). The affected fields: %A"
                t.FullName
        | _, x, _ when x <> commands.Length ->
            cNames
            |> List.filter (fun (_, xs) -> xs.Length > 1)
            |> List.map fst
            |> failwithf "Modules containing functions for the target type '%s' contained multiple functions with the same name(s). The affected functions: %A"
                t.FullName
        | _, _, x when x <> (props.Length + commands.Length) ->
            allNames
            |> List.filter (fun (_, xs) -> xs.Length > 1)
            |> List.map fst
            |> failwithf "Modules containing functions for the target type '%s' contained functions with the same names as fileds on the target type. The affected functions/fields: %A"
                t.FullName
        | _ -> ()

        let fieldProperties =
            props
            |> Seq.map
                (fun x ->
                    let isReadOnly =
                        let bindingFlags =
                            System.Reflection.BindingFlags.NonPublic |||
                            System.Reflection.BindingFlags.Instance
                        let field = x.DeclaringType.GetField(x.Name + "@", bindingFlags)
                        match field with
                        | null -> false
                        | _ ->
                            field.GetCustomAttributes(typeof<ReadOnlyAttribute>, true)
                            |> Array.tryHead
                            |> Option.isSome
                    let doReadOnly (f : string -> string) writer =
                        match isReadOnly with
                        | true -> writer
                        | false -> writer + (f x.Name)
                    let f() =
                        sprintf "\tmember __.``%s``\n\t\twith get () = ivm.CurrentValue.``%s``"
                            x.Name x.Name
                        |> doReadOnly (sprintf "\n\t\tand set value = ivm.UpdateField (fun x -> { x with ``%s`` = value })")
                    match x.PropertyType.IsGenericType with
                    | false -> f()
                    | true ->
                        let generic = x.PropertyType.GetGenericTypeDefinition()
                        if generic = typeof<int option>.GetGenericTypeDefinition() then
                            let optionWrapped =
                                sprintf "\tmember __.``%s``\n\t\twith get () = ivm.CurrentValue.``%s``"
                                    x.Name x.Name
                                |> doReadOnly (sprintf "\n\t\tand set value = ivm.UpdateField (fun x -> { x with ``%s`` = value })")
                            let wpfWritable =
                                sprintf "\tmember __.``%s_Bindable``\n\t\twith get () = ivm.CurrentValue.``%s`` |> optionToNull"
                                    x.Name x.Name
                                |> doReadOnly (sprintf "\n\t\tand set value = ivm.UpdateField (fun x -> { x with ``%s`` = (optionOfNull value) })")
                            optionWrapped + "\n" + wpfWritable
                        elif generic = typeof<Map<int, int>>.GetGenericTypeDefinition() then
                            sprintf "%s\n\tmember __.``%s_Bindable``\n\t\twith get () = ``%s_immutable_view_model_observable_map``"
                                (f()) x.Name x.Name
                        elif generic = typeof<Set<int>>.GetGenericTypeDefinition() then
                            sprintf "%s\n\tmember __.``%s_Bindable``\n\t\twith get () = ``%s_immutable_view_model_observable_set``"
                                (f()) x.Name x.Name
                        elif generic = typeof<int list>.GetGenericTypeDefinition() then
                            sprintf "%s\n\tmember __.``%s_Bindable`` with get () = ``%s_immutable_view_model_observable_list``"
                                (f()) x.Name x.Name
                        else f()
                )
        let commandProperties =
            commands
            |> Seq.map
                (fun x ->
                    sprintf "\tmember __.``%s`` with get () = ``%s_immutable_view_model_command``"
                        x (x.ToLowerInvariant())
                )

        fieldProperties
        |> Seq.append commandProperties
        |> fun xs -> System.String.Join("\n", xs)


    let getUserProperties (t : System.Type) (props : System.Reflection.PropertyInfo array) =
        props
        |> Seq.filter (fun x -> x.CanRead)
        |> Seq.map
            (fun x ->
                sprintf "\tmember __.``%s`` with get () = ivm.CurrentValue.``%s``"
                    x.Name x.Name
            )
        |> fun xs -> System.String.Join("\n", xs)


    let rec quotationToName x =
        match x with
        | Microsoft.FSharp.Quotations.Patterns.PropertyGet (_, x, _) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.FieldGet    (_, x   ) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.Call        (_, x, _) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.Var         (   x   ) -> x.Name
        | Microsoft.FSharp.Quotations.Patterns.Lambda      (_, x   ) -> quotationToName x
        | _ -> failwithf "Could not get a name from %A" x


    let getDependencies (t : System.Type) =
        t.Assembly.GetTypes()
        |> Seq.filter (fun x -> x.IsPublic || x.IsNestedPublic)
        |> Seq.filter
            (fun x ->
                x.GetCustomAttributes(typeof<HelperModuleAttribute>, true)
                |> Seq.exists (fun x -> (x :?> HelperModuleAttribute).TargetType = t )
            )
        |> Seq.collect
            (fun x ->
                x.GetMethods()
                |> Seq.filter (fun x -> x.IsStatic)
                |> Seq.filter (fun x -> x.IsPublic)
                |> Seq.filter
                    (fun x ->
                        x.GetCustomAttributes(typeof<PropertyDependenciesAttribute>, true)
                        |> Seq.tryHead
                        |> Option.isSome
                    )
                |> Seq.collect
                    (fun x ->
                        let ps = x.GetParameters()
                        match ps with
                        | [| y |] when y.ParameterType = t && x.ReturnType = typeof<Quotations.Expr * Quotations.Expr> ->
                            let (x, y) = x.Invoke(null, [| null |]) :?> (Quotations.Expr * Quotations.Expr)
                            sprintf "\t\tivm.AddDependency(\"%s\", \"%s\")"
                                (quotationToName x)
                                (quotationToName y)
                            |> Seq.singleton

                        | [| y |] when y.ParameterType = t && x.ReturnType = typeof<(Quotations.Expr * Quotations.Expr) list> ->
                            x.Invoke(null, [| null |]) :?> ((Quotations.Expr * Quotations.Expr) list)
                            |> Seq.map
                                (fun (x, y) ->
                                    sprintf "\t\tivm.AddDependency(\"%s\", \"%s\")"
                                        (quotationToName x)
                                        (quotationToName y)
                                )

                        | [| y |] when y.ParameterType = t && x.ReturnType = typeof<Quotations.Expr * (Quotations.Expr list)> ->
                            let (y, ys) = x.Invoke(null, [| null |]) :?> (Quotations.Expr * (Quotations.Expr list))
                            let z = quotationToName y
                            ys
                            |> Seq.map
                                (fun y ->
                                    sprintf "\t\tivm.AddDependency(\"%s\", \"%s\")"
                                        z (quotationToName y)
                                )

                        | _ -> Seq.empty
                    )
            )
        |> fun xs -> System.String.Join("\n", xs)


    [<TypeExpander("ImmutableViewModel")>]
    let ImmutableViewModel (t : System.Type) =
        match Microsoft.FSharp.Reflection.FSharpType.IsRecord(t) with
        | true -> ()
        | false -> failwithf "Target type %A is not a record type. The ImmutableViewModel expander can only be run against record types." t.FullName

        let fieldProperties = Microsoft.FSharp.Reflection.FSharpType.GetRecordFields(t)
        let userProvidedProperties =
            System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.Public
            |> t.GetProperties
            |> Array.except fieldProperties

        let (commandNames, commandInits) = getCommands t

        let propertyDependencies = getDependencies t

        sprintf
            "namespace %s
type ``%sViewModel``(initialData) as self =
    inherit Amazingant.FSharp.TypeExpansion.Templates.ImmutableViewModel.ImmutableViewModelBase<%s.``%s``>(
        (fun () -> initialData),
        [
%s
        ])

    let ivm = (self :> Amazingant.FSharp.TypeExpansion.Templates.ImmutableViewModel.IImmutableViewModelBase<_>)

    // Helpers for dealing with Option<'T> to/from null in places where that
    // would not type-check, i.e. 'T is a record or union.
    let optionToNull (x : 'T option) =
        match x with
        | Some x -> x
        | None -> Unchecked.defaultof<'T>
    let optionOfNull (x : 'T) =
        match box x with
        | null -> None
        | _ -> Some x


    // Observable lists
%s


    // Commands
%s


    do
        // Dependencies
%s
        ()

    // With no parameters provided, this ctor uses the static \"Default\"
    // property as the initial value.
    new () = ``%sViewModel``(%s.``%s``.Default)


    member __.InternalState_CurrentValue
        with get () =
            ivm.CurrentValue
    member __.InternalState_IsProcessing
        with get () =
            ivm.IsProcessing

    // Properties
%s
    // User-provided properties
%s
"
            t.Namespace        // "namespace %s"
                        t.Name // "%s.%sViewModel"
            t.Namespace t.Name // "..ViewModelBase<%s.``%s``>"
            (getFieldNamesAndFunctions fieldProperties)
            (getObservableLists fieldProperties)
            commandInits
            propertyDependencies
            t.Name t.Namespace t.Name // ``..ViewModel``("%s.``%s``.Default)"
            (getProperties t fieldProperties commandNames)
            (getUserProperties t userProvidedProperties)

        |> fun x -> x.Replace("\t", "    ").Replace("\r", "").TrimEnd()
