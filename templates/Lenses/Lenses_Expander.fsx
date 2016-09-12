#if INTERACTIVE
// Load the TypeExpansion.Attributes library from one of these two relative
// paths in fsi and Visual Studio's script editor
#I "../../packages/Amazingant.FSharp.TypeExpansion/lib/net45"
#I "../../../Amazingant.FSharp.TypeExpansion/lib/net45"
#r "Amazingant.FSharp.TypeExpansion.Attributes.dll"
#r "Amazingant.FSharp.TypeExpansion.dll"

// Load the base file to get access to the attribute
#load "Lenses_Base.fsx"
#endif

namespace Amazingant.FSharp.TypeExpansion.Templates.Lenses


open System
open System.Reflection
open Amazingant.FSharp.TypeExpansion.Attributes

module LensExpansion =
    let getTypeName = Amazingant.FSharp.TypeExpansion.TypeIntrospection.GetTemplateFriendlyName
    let notEmpty = Seq.isEmpty >> not
    let joinLines    (x : string seq) = String.Join("\n"  , x)
    // Not sure if this is the best way to detect this, but it works?
    let isUnionField (p : PropertyInfo) = p.GetCustomAttributes(typeof<CompilationMappingAttribute>, false) |> notEmpty
    let hasAttribute (a : Type) = a.GetCustomAttributes(typeof<LensAttribute>, false) |> notEmpty

    // Builds two lenses (one static, one not) for the given type
    let getBasicPropsForT (t : Type) =
        let buildForProp (p : PropertyInfo) =
            let s =
                sprintf "
            static member %s_StaticLens : StaticLens<%s, %s> =
                MakeStaticLens
                    (fun x -> x.%s)
                    (fun v x -> { x with %s = v })"
                    p.Name t.Name (getTypeName p.PropertyType) p.Name p.Name
            let m =
                sprintf "            member self.%s_Lens =
                Lens.FromStatic self
                    %s.%s_StaticLens"
                    p.Name p.ReflectedType.Name p.Name
            [s;m]

        t.GetProperties()
        |> Seq.filter isUnionField
        |> Seq.collect buildForProp


    // Builds two lenses (one static, one not) for all of the (potential) nested
    // lenses, and composes them as needed
    let getNestedProps (t : Type) =
        let rec doNested (nestings : string) (p : PropertyInfo) =
            let sLens = sprintf "%s%s_StaticLens" nestings p.Name
            let sProp =
                sprintf "
            static member %s =
                Compose
                    %s.%sStaticLens
                    %s.%s_StaticLens"
                    sLens
                    t.Name
                    nestings
                    p.ReflectedType.Name
                    p.Name
            let mProp =
                sprintf "            member self.%s%s_Lens =
                Lens.FromStatic self
                    %s.%s"
                    nestings
                    p.Name
                    t.Name
                    sLens
            let nested =
                if p.PropertyType |> hasAttribute |> not then
                    Seq.empty
                else
                    p.PropertyType.GetProperties()
                    |> Seq.filter isUnionField
                    |> Seq.collect (fun x -> doNested (sprintf "%s%s_" nestings p.Name) x)

            Seq.append [sProp;mProp] nested

        t.GetProperties()
        |> Seq.filter (fun x -> x.PropertyType |> hasAttribute)
        |> Seq.filter isUnionField
        |> Seq.collect
            (fun x ->
                x.PropertyType.GetProperties()
                |> Seq.filter isUnionField
                |> Seq.collect (doNested (x.Name + "_"))
            )


    [<TypeExpander("Lenses")>]
    let f (t : Type) =
        let props = Seq.append (getBasicPropsForT t) (getNestedProps t) |> Seq.toArray
        if props.Length = 0 then
            ""
        else
            sprintf "namespace %s
    [<AutoOpen>]
    module %s_Lenses_Extensions =
        type %s with
%s
"
                t.Namespace
                t.Name
                t.Name
                (props |> joinLines)
