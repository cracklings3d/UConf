module UConf

open Elmish.WPF
open ModernWpf.Controls

type NavPage =
  | Home
  | Code
  | CreateClass
  | CreateModule
  | Svn
  | Automation

type Model =
  {
    Count: int
    StepSize: int
    Address: NavPage
  }

let init () =
  {
    Count = 0
    StepSize = 1
    Address = Home
  }

type Msg =
  | Increment
  | Decrement
  | SetStepSize of int
  | Navigate of NavPage

let update msg m =
  match msg with
  | Increment -> { m with Count = m.Count + m.StepSize }
  | Decrement -> { m with Count = m.Count - m.StepSize }
  | SetStepSize x -> { m with StepSize = x }
  | Navigate p -> { m with Address = p }

let bindings () =
  [
    // Values
    "CounterValue"
    |> Binding.oneWay (fun m -> m.Count)
    "StepSize"
    |> Binding.twoWay ((fun m -> float m.StepSize), (fun newVal m -> int newVal |> SetStepSize))
    "Address"
    |> Binding.oneWay (fun m -> m.Address.ToString())

    // Events
    "Increment" |> Binding.cmd (fun m -> Increment)
    "Decrement" |> Binding.cmd (fun m -> Decrement)
    "Navigate"
    |> Binding.cmdParam (fun s m ->
      match ((s :?> NavigationView).SelectedItem :?>NavigationViewItem).Name with
      | "NavHome" -> Navigate Home
      | "NavCode" -> Navigate Code
      | "NavCreateClass" -> Navigate CreateClass
      | "NavCreateModule" -> Navigate CreateModule
      | "NavSvn" -> Navigate Svn
      | "NavAutomation" -> Navigate Automation
      | _ -> failwith "Invalid Navigation Address")
  ]


let main window =
  WpfProgram.mkSimple init update bindings
  |> WpfProgram.startElmishLoop window
