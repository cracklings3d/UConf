module Plugin

open System

open FSharp.Data
open FSharp.Data.JsonExtensions

type Ref() =
  member val Name: String = "" with get, set
  member val Enabled: bool = true with get, set
  member val MarketplaceURL: String Option = None with get, set

let parseRef (j: JsonValue) =
  let d = Ref()

  d.Name <-
    j?Name
    |> JsonExtensions.AsString

  d.Enabled <-
    j?Enabled
    |> JsonExtensions.AsBoolean

  d.MarketplaceURL <-
    match j.TryGetProperty "MarketplaceURL" with
    | None -> None
    | Some v ->
      v
      |> JsonExtensions.AsString
      |> Some

  d
  
type Descriptor () =
  member val Name : String = "" with get, set
  member val ReadableName : String = "" with get, set
  member val Description : String = "" with get, set
  member val Category : String = "" with get, set
  member val Author : String = "" with get, set
  member val Version : Version = Version "0.0" with get, set
  member val SvnAddress : Uri option = None with get, set
  member val MarketplaceAddress : Uri option = None with get, set
  member val Modules : Module.Ref seq = [] with get, set
  
let parseDescriptor (j : JsonValue) =
  let v = Descriptor ()
  v.Author <- j?Author |> JsonExtensions.AsString
  v.Category <- j?Category |> JsonExtensions.AsString
  v.Description <- j?Description |> JsonExtensions.AsString
  v.Modules <- j?Plugins |> JsonExtensions.AsArray |> Seq.map Module.parseRef
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
