module Project

open System
open FSharp.Data
open FSharp.Data.JsonExtensions

type ModuleType =
  | Runtime
  | Editor
  | Unspecified

let parseModuleType str =
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

type ModuleDescriptor =
  {
    Name: string
    Type: ModuleType
    LoadingPhase: LoadingPhase
    AdditionalDependencies: string seq option
  }

type PluginDescriptor =
  {
    Name: string
    Enabled: bool
    MarketplaceURL: string option
  }

type Descriptor =
  {
    FileVersion: string
    EngineAssociation: Version
    Category: string
    Description: string
    Modules: ModuleDescriptor seq
    Plugins: PluginDescriptor seq
  }

let projectFileExtension = ".uproject"

let isValidProjectDescriptor (file: Uri) =
  file.IsFile
  && file.AbsoluteUri.EndsWith projectFileExtension

let parseModuleDescriptor (j: JsonValue) =
  {
    Name = j?Name |> JsonExtensions.AsString
    Type =
      j?Type
      |> JsonExtensions.AsString
      |> parseModuleType
    LoadingPhase =
      j?LoadingPhase
      |> JsonExtensions.AsString
      |> parseLoadingPhase
    AdditionalDependencies =
      match j.TryGetProperty "AdditionalDependencies" with
      | None -> None
      | Some v ->
        v
        |> JsonExtensions.AsArray
        |> Seq.map JsonExtensions.AsString
        |> Some
  }

let parsePluginDescriptor (j: JsonValue) =
  {
    Name = j?Name |> JsonExtensions.AsString
    Enabled = j?Enabled |> JsonExtensions.AsBoolean
    MarketplaceURL =
      match j.TryGetProperty "MarketplaceURL" with
      | None -> None
      | Some v -> v |> JsonExtensions.AsString |> Some
  }

let parseDescriptor (file: Uri) =
  assert (file |> isValidProjectDescriptor)

  let j_pd =
    file.LocalPath
    |> System.IO.File.ReadAllText
    |> JsonValue.Parse

  {
    FileVersion = j_pd?FileVersion |> JsonExtensions.AsString
    EngineAssociation =
      j_pd?EngineAssociation
      |> JsonExtensions.AsString
      |> Version.Parse
    Category = j_pd?Category |> JsonExtensions.AsString
    Description = j_pd?Description |> JsonExtensions.AsString
    Modules =
      j_pd?Modules
      |> JsonExtensions.AsArray
      |> Seq.map parseModuleDescriptor
    Plugins =
      j_pd?Plugins
      |> JsonExtensions.AsArray
      |> Seq.map parsePluginDescriptor
  }
