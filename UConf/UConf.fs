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
    Log: string

    mutable UserConf: User.Conf

    ProjectUri: Uri
    ProjectDescriptor: Project.Descriptor
  }

let init () =
  {
    Log = ""

    UserConf = User.LoadConf()

    ProjectUri = Uri "C:/"
    ProjectDescriptor = Project.Descriptor()
  }

type Msg =
  | SetUserConf of User.Conf
  | SaveUserConf of User.Conf
  | SetProjectDescriptor of Project.Descriptor
  | OpenProject
  | ExitProgram

let update msg m =
  match msg with
  | SetUserConf u -> { m with UserConf = u }
  | SaveUserConf u ->
    User.SaveConf u
    { m with UserConf = u }
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
    "Log"
    |> Binding.oneWay (fun m -> m.Log)

    "UserConf"
    |> Binding.twoWay ((fun m -> m.UserConf), (fun u m -> SetUserConf u))

    // Project Descriptor
    "ProjectDescriptor"
    |> Binding.twoWay ((fun m -> m.ProjectDescriptor), (fun d m -> SetProjectDescriptor d))

    "VisRequireProject"
    |> Binding.oneWay (fun m ->
      let uri_is_valid =
        m.ProjectUri.AbsoluteUri.EndsWith ".uproject"

      match uri_is_valid with
      | true -> Visibility.Visible
      | _ -> Visibility.Collapsed)
    "VisRequireNoProject"
    |> Binding.oneWay (fun m ->
      let uri_is_valid =
        m.ProjectUri.AbsoluteUri.EndsWith ".uproject"

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
    "OpenProject"
    |> Binding.cmd (fun m -> OpenProject)
    "SaveUserConf"
    |> Binding.cmd (fun m -> SaveUserConf m.UserConf)
    "ExitProgram"
    |> Binding.cmd ExitProgram
  ]

let main window =
  WpfProgram.mkSimple init update bindings
  |> WpfProgram.startElmishLoop window
