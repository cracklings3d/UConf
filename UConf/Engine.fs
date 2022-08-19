module Engine

open System


type Install () =
  member val Version : Version = Version "5.0" with get, set
  member val RootPath : Uri = Uri "file:///" with get, set
  member val Plugins : Plugin.Descriptor List = [] with get, set
