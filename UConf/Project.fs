module Project

open System
open FSharp.Data
open FSharp.Data.JsonExtensions

[<Literal>]
let projectFileExtension = ".uproject"

type Descriptor () =
  member val FileVersion : String = "3" with get, set
  member val EngineAssociation : Version = Version "5.0" with get, set
  member val Category : String = "" with get, set
  member val Description : String = "" with get, set
  member val Modules : Module.Ref seq = [] with get, set
  member val Plugins : Plugin.Ref seq = [] with get, set

let isValidProjectDescriptor (file : Uri) = file.IsFile && file.AbsoluteUri.EndsWith projectFileExtension

let parseDescriptor (file : Uri) =
  assert (file |> isValidProjectDescriptor)

  let j_pd =
    file.LocalPath |> System.IO.File.ReadAllText |> JsonValue.Parse

  let mutable d = Descriptor ()
  d.FileVersion <- j_pd?FileVersion |> JsonExtensions.AsString
  d.EngineAssociation <- j_pd?EngineAssociation |> JsonExtensions.AsString |> Version.Parse
  d.Category <- j_pd?Category |> JsonExtensions.AsString
  d.Description <- j_pd?Description |> JsonExtensions.AsString
  d.Modules <- j_pd?Modules |> JsonExtensions.AsArray |> Seq.map Module.parseRef
  d.Plugins <- j_pd?Plugins |> JsonExtensions.AsArray |> Seq.map Plugin.parseRef

  d
