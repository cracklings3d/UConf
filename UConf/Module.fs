module Module

open System

open FSharp.Data
open FSharp.Data.JsonExtensions

type Type =
  | Runtime
  | Editor
  | Unspecified

let parseType str =
  match str with
  | "Runtime" -> Runtime
  | "Editor" -> Editor
  | "" -> Unspecified
  | _ -> failwith "Invalid Module Type"

type LoadingPhase =
  | Default
  | Unspecified

let parseLoadingPhase str =
  match str with
  | "Default" -> Default
  | "" -> Unspecified
  | _ -> failwith "Invalid Loading Phase"

type Ref() =
  member val Name: String = "" with get, set
  member val Type: Type = Type.Unspecified with get, set
  member val LoadingPhase: LoadingPhase = LoadingPhase.Unspecified with get, set
  member val AdditionalDependencies: String seq Option = None with get, set

let parseRef (j: JsonValue) =
  let d = Ref()

  d.Name <-
    j?Name
    |> JsonExtensions.AsString

  d.Type <-
    j?Type
    |> JsonExtensions.AsString
    |> parseType

  d.LoadingPhase <-
    j?LoadingPhase
    |> JsonExtensions.AsString
    |> parseLoadingPhase

  d.AdditionalDependencies <-
    match j.TryGetProperty "AdditionalDependencies" with
    | None -> None
    | Some v ->
      v
      |> JsonExtensions.AsArray
      |> Seq.map JsonExtensions.AsString
      |> Some

  d
