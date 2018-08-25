module ContentGenerator

open FSharpx.Collections

let private minRequestSize createJson contactIdentifier =
    createJson contactIdentifier |>
    MinimunDataFaker.interpret |>
    getSize

let private isMaxLength minJson contactIdentifier maxRequestLength total =
    (total + (minRequestSize minJson contactIdentifier)) > maxRequestLength

let getItemsJson createJson maxRequestLength id count fakerInterpreter =
    let isMaxLength =
        isMaxLength createJson id maxRequestLength

    let unfolder (total, strings) =
        match strings with
        | LazyList.Nil             -> None
        | _ when isMaxLength total -> None
        | LazyList.Cons (x, xs)    -> Some (x, (x |> getSize |> (+) total, xs))

    id |>
    LazyList.repeat |>
    LazyList.take count |>
    LazyList.map (createJson >> fakerInterpreter) |>
    (fun json -> (0.0<KB>, json)) |>
    LazyList.unfold unfolder |>
    (fun jsons -> (jsons.Length(), jsons |> String.concat "," |> sprintf "[%s]"))