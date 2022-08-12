module User

open System
open System.Text.Json
open System.IO

open FSharp.Data
open FSharp.Data.JsonExtensions
open FSharp.Json

[<Literal>]
let ConfFileName = "app.conf"

let ConfFileDir =
  Path.Combine (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "UConf")

let ConfFilePath =
  Path.Combine (ConfFileDir, ConfFileName)

type PluginInstall () =
  member val Name : String = "" with get, set
  member val ReadableName : String = "" with get, set
  member val Description : String = "" with get, set
  member val Category : String = "" with get, set
  member val Author : String = "" with get, set
  member val Version : Version = Version "0.0" with get, set
  member val SvnAddress : Uri option = None with get, set
  member val MarketplaceAddress : Uri option = None with get, set
  member val Modules : Module.Descriptor seq = [] with get, set

let parsePluginInstall (j : JsonValue) =
  let v = PluginInstall ()
  v.Author <- j?Author |> JsonExtensions.AsString
  v.Category <- j?Category |> JsonExtensions.AsString
  v.Description <- j?Description |> JsonExtensions.AsString
  v.Modules <- j?Plugins |> JsonExtensions.AsArray |> Seq.map Module.parseDescriptor
  v.Name <- j?Name |> JsonExtensions.AsString
  v.Version <- j?Version |> JsonExtensions.AsString |> Version
  v.ReadableName <- j?ReadableName |> JsonExtensions.AsString

  v.MarketplaceAddress <-
    match j?MarketplaceAddress with
    | JsonValue.Null -> None
    | s -> Some <| (s |> JsonExtensions.AsString |> Uri)

  v.SvnAddress <-
    match j?SvnAddress with
    | JsonValue.Null -> None
    | s -> Some <| (s |> JsonExtensions.AsString |> Uri)

  v

type EngineInstall () =
  member val Version : Version = Version "5.0" with get, set
  member val RootPath : Uri = Uri "file:///" with get, set
  member val Plugins : PluginInstall seq = [] with get, set

let parseEngineInstall (j : JsonValue) =
  let v = EngineInstall ()
  v.Plugins <- j?Plugins |> JsonExtensions.AsArray |> Seq.map parsePluginInstall
  v.Version <- j?Version |> JsonExtensions.AsString |> Version
  v.RootPath <- j?RootPath |> JsonExtensions.AsString |> Uri
  v

type Conf () =
  member val EngineInstalls : EngineInstall seq = [] with get, set
  // SvnExecutable: Uri
  member val SvnExecutable : Uri = Uri "file://" with get, set

let parseConf (j : JsonValue) =
  let mutable c = Conf ()
  c.SvnExecutable <- j?SvnExecutable |> JsonExtensions.AsString |> Uri
  c.EngineInstalls <- j?EngineInstalls |> JsonExtensions.AsArray |> Seq.map parseEngineInstall
  c

let LoadConf () =
  Directory.CreateDirectory ConfFileDir |> ignore

  match File.Exists ConfFilePath with
  | false -> Conf ()
  | _ ->
    let conf_str = File.ReadAllText ConfFilePath
    JsonSerializer.Deserialize<Conf> conf_str
    // TODO: replace every parser with deserialize

let SaveConf (conf : Conf) =
  let str = JsonSerializer.Serialize conf
  File.WriteAllText (ConfFilePath, str)
  ()
