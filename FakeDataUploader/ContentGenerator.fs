module ContentGenerator

open FSharpx.Collections

let minRequestSize createJson contactIdentifier =
    createJson contactIdentifier |>
    MinimunDataFaker.interpret |>
    getSize

let countSizeFolder (total, strings) value =
    (value |> getSize |> (+) total, value :: strings)

let isMaxLength minJson contactIdentifier maxRequestLength (total, _) =
    (total + (minRequestSize minJson contactIdentifier)) > maxRequestLength

let getItemsJson createJson maxRequestLength id count fakerInterpreter =
    let isMaxLength =
        isMaxLength createJson id maxRequestLength

    id |>
    LazyList.repeat |>
    LazyList.take count |>
    LazyList.map (createJson >> fakerInterpreter) |>
    LazyList.folde countSizeFolder isMaxLength (0.0<KB>, List.empty<string>) |>
    snd |>
    (fun jsons -> (jsons.Length, jsons |> String.concat "," |> sprintf "[%s]"))