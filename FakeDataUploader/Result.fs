module Result

let resultsFolder x y =
    match (x, y) with
    | (Ok x,          Ok y)         -> Ok    (x @ y)
    | (Error (ls, x), Error (s, y)) -> Error (s :: ls, x @ y)
    | (Ok x,          Error (s, y)) -> Error ([s]    , x @ y)
    | (Error (ls, x), Ok s)         -> Error (ls     , x @ s)