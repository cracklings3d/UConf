module User

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.FSharp

[<Literal>]
let ConfFileName = "app.conf"

let ConfFileDir =
  Path.Combine (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "UConf")

let ConfFilePath = Path.Combine (ConfFileDir, ConfFileName)

type Conf () =
  member val EngineInstalls : Engine.Install List = [] with get, set
  member val SvnExecutable : Uri = Uri "file://" with get, set

let LoadConf () =
  Directory.CreateDirectory ConfFileDir |> ignore

  match File.Exists ConfFilePath with
  | false -> Conf ()
  | _ ->
    let conf_str = File.ReadAllText ConfFilePath
    let converters : JsonConverter [] = [| new ListConverter () |]
    JsonConvert.DeserializeObject<Conf> (conf_str, converters)

let SaveConf (conf : Conf) =
  let str = JsonConvert.SerializeObject conf
  File.WriteAllText (ConfFilePath, str)
  ()
