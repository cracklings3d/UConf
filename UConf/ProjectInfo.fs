module ProjectInfo

open System

open Elmish
open Elmish.WPF
open Microsoft.Win32

type Model = { Dir: Uri }

type Msg = | Open

let init () = { Dir = Uri "" }

let update msg m =
  match msg with
  | Open ->
    let ofd = OpenFileDialog()
    ofd.Filter <- "Unreal Project File|*.uproject"
    ofd.Multiselect <- false
    ofd.Title <- "Open Unreal project"

    if ofd.ShowDialog().HasValue then
      let dir = ofd.FileName
      { m with Dir = Uri dir }
    else
      m

let bindings () : Binding<Model, Msg> list =
  [
    // Event
    "Open" |> Binding.cmd Open

    // Data
    "Dir"
    |> Binding.oneWay (fun m -> m.Dir.ToString())
  ]
