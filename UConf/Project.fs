module Project

open System
open FSharp.Data
open FSharp.Data.JsonExtensions

type ModuleType = | Runtime

let parseModuleType str =
  match str with
  | "Runtime" -> Runtime
  | _ -> failwith "Invalid Module Type"

type LoadingPhase = | Default

let parseLoadingPhase str =
  match str with
  | "Default" -> Default
  | _ -> failwith "Invalid Loading Phase"

type ModuleDescriptor =
  {
    Name: string
    Type: ModuleType
    LoadingPhase: LoadingPhase
    AdditionalDependencies: string seq
  }

type PluginDescriptor =
  {
    Name: string
    Enabled: bool
    MarketplaceURL: string
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
    Name =
      j?Name
      |> JsonExtensions.AsString
    Type =
      j?Type
      |> JsonExtensions.AsString
      |> parseModuleType
    LoadingPhase =
      j?Type
      |> JsonExtensions.AsString
      |> parseLoadingPhase
    AdditionalDependencies =
      j?AdditionalDependencies
      |> JsonExtensions.AsArray
      |> Seq.map JsonExtensions.AsString
  }

let parsePluginDescriptor (j: JsonValue) =
  {
    Name =
      j?Name
      |> JsonExtensions.AsString
    Enabled =
      j?Enabled
      |> JsonExtensions.AsBoolean
    MarketplaceURL =
      j?MarketplaceURL
      |> JsonExtensions.AsString
  }

let parseDescriptor (file: Uri) =
  assert
    (file
     |> isValidProjectDescriptor)

  let j_pd =
    file.LocalPath
    |> System.IO.File.ReadAllText
    |> JsonValue.Parse

  {
    FileVersion =
      j_pd?FileVersion
      |> JsonExtensions.AsString
    EngineAssociation =
      j_pd?EngineAssociation
      |> JsonExtensions.AsString
      |> Version.Parse
    Category =
      j_pd?Category
      |> JsonExtensions.AsString
    Description =
      j_pd?Description
      |> JsonExtensions.AsString
    Modules =
      j_pd?Modules
      |> JsonExtensions.AsArray
      |> Seq.map parseModuleDescriptor
    Plugins =
      j_pd?Plugins
      |> JsonExtensions.AsArray
      |> Seq.map parsePluginDescriptor
  }
