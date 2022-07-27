module UConf

open Elmish.WPF

type ConfSection =
  | None
  | CPP

type Model =
  {
    Count: int
    StepSize: int
    ConfSection: ConfSection
  }

let init () = {
  Count = 0
  StepSize = 1
  ConfSection = None
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

    // Events
    "Increment" |> Binding.cmd (fun m -> Increment)
    "Decrement" |> Binding.cmd (fun m -> Decrement)
  ]

let main window =
  WpfProgram.mkSimple init update bindings
  |> WpfProgram.startElmishLoop window
