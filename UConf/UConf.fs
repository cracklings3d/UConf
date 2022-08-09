module Core

open System
open System.IO
open System.Windows
open System.Windows.Input
open Microsoft.Win32

open Elmish
open Elmish.WPF

type Model =
  {
    Count: int
    StepSize: int
    Log: string

    ProjectUri: Uri
    ProjectDescriptor: Project.Descriptor
  }

let init () =
  {
    Count = 0
    StepSize = 1
    Log = ""
    ProjectUri = Uri "C:/"
    ProjectDescriptor =
      {
        FileVersion = ""
        EngineAssociation = Version "5.0"
        Category = ""
        Description = ""
        Modules = []
        Plugins = []
      }
  }

type Msg =
  | Increment
  | Decrement
  | SetStepSize of int
  | SetProjectDescriptor of Project.Descriptor
  | OpenProject
  | ExitProgram

let update msg m =
  match msg with
  | Increment -> { m with Count = m.Count + m.StepSize }
  | Decrement -> { m with Count = m.Count - m.StepSize }
  | SetStepSize x -> { m with StepSize = x }
  | SetProjectDescriptor d -> { m with ProjectDescriptor = d }
  | OpenProject ->
    let ofd = OpenFileDialog()
    ofd.Filter <- "Unreal Project File|*.uproject"
    ofd.Multiselect <- false
    ofd.Title <- "Open Unreal project"

    if ofd.ShowDialog().HasValue then
      let uri_str = ofd.FileName

      match ofd.FileName.Length with
      | 0 -> m
      | _ ->
        let uri = Uri uri_str

        { m with
            ProjectUri = uri
            ProjectDescriptor = Project.parseDescriptor uri
        }
    else
      m
  | ExitProgram ->
    Application.Current.Shutdown 0
    m

let bindings () =
  [
    "CounterValue"
    |> Binding.oneWay (fun m -> m.Count)
    "StepSize"
    |> Binding.twoWay ((fun m -> float m.StepSize), (fun newVal m -> int newVal |> SetStepSize))
    "Log" |> Binding.oneWay (fun m -> m.Log)

    // Project Descriptor
    "ProjectDescriptor"
    |> Binding.twoWay ((fun m -> m.ProjectDescriptor), (fun d m -> d |> SetProjectDescriptor))

    "VisRequireProject"
    |> Binding.oneWay (fun m ->
      let uri_is_valid = m.ProjectUri.AbsoluteUri.EndsWith ".uproject"

      match uri_is_valid with
      | true -> Visibility.Visible
      | _ -> Visibility.Collapsed)
    "VisRequireNoProject"
    |> Binding.oneWay (fun m ->
      let uri_is_valid = m.ProjectUri.AbsoluteUri.EndsWith ".uproject"

      match uri_is_valid with
      | true -> Visibility.Collapsed
      | _ -> Visibility.Visible)
    "ProjectDir"
    |> Binding.oneWay (fun m ->
      if m.ProjectUri.IsFile then
        let p = m.ProjectUri.LocalPath
        Path.GetDirectoryName p
      else
        "")
    "WindowTitle"
    |> Binding.oneWay (fun m ->
      if m.ProjectUri.IsFile then
        let p = m.ProjectUri.LocalPath
        Path.GetFileNameWithoutExtension p
      else
        "")

    // Events
    "Increment" |> Binding.cmd (fun m -> Increment)
    "Decrement" |> Binding.cmd (fun m -> Decrement)
    "OpenProject"
    |> Binding.cmd (fun m -> OpenProject)
    "ExitProgram" |> Binding.cmd ExitProgram
  ]

let main window =
  WpfProgram.mkSimple init update bindings
  |> WpfProgram.startElmishLoop window
