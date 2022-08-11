module User

open System
open FSharp.Data
open FSharp.Data.JsonExtensions



type EngineInstall =
  {
    Version: Version
    RootPath: Uri
    Plugins: PluginInstall seq
  }

type UserConf =
  {
    EngineInstalls: EngineInstall seq
    SvnExecutable: Uri
  }
