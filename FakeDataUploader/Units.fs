[<AutoOpen>]
module Units

open System.Text

[<Measure>]
type KB

let getSize (value : string) =
    value |>
    Encoding.UTF8.GetByteCount |>
    float |>
    (*) 0.001<KB>