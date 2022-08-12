module Plugin

open System

open FSharp.Data
open FSharp.Data.JsonExtensions

type Descriptor() =
  member val Name: String = "" with get, set
  member val Enabled: bool = true with get, set
  member val MarketplaceURL: String Option = None with get, set

let parseDescriptor (j: JsonValue) =
  let d = Descriptor()

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
