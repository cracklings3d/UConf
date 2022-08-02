module UConf

open System

open Elmish
open Elmish.WPF

type Model =
  {
    Count: int
    StepSize: int
    Log: string

    ProjectInfo: ProjectInfo.Model
  }

let init () =
  {
    Count = 0
    StepSize = 1
    Log = ""
    ProjectInfo = { Dir = Uri "C:\\" }
  }

type Msg =
  | Increment
  | Decrement
  | SetStepSize of int

let update msg m =
  match msg with
  | Increment -> { m with Count = m.Count + m.StepSize }
  | Decrement -> { m with Count = m.Count - m.StepSize }
  | SetStepSize x -> { m with StepSize = x }

let bindings () =
  [
    // Values
    "CounterValue"
    |> Binding.oneWay (fun m -> m.Count)
    "StepSize"
    |> Binding.twoWay ((fun m -> float m.StepSize), (fun newVal m -> int newVal |> SetStepSize))
    "Address" |> Binding.oneWay (fun m -> m.Log)

    // Events
    "Increment" |> Binding.cmd (fun m -> Increment)
    "Decrement" |> Binding.cmd (fun m -> Decrement)
  ]


let main window =
  WpfProgram.mkSimple init update bindings
  |> WpfProgram.startElmishLoop window
