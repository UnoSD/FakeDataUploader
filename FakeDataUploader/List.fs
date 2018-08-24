module LazyList

open FSharpx.Collections

let rec folde folder over state (source : LazyList<'a>) =
    match source with
    | LazyList.Nil          -> state
    | LazyList.Cons (x, xs) -> match folder state x with
                               | state when not (state |> over) -> folde folder
                                                                         over
                                                                         state
                                                                         xs
                               | state                          -> state