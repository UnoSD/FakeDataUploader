module Concurrency

open System

let partitions (count : int) total =
    match Math.DivRem(total, count) with
    | (x, 0) -> List.replicate count x
    | (x, y) -> List.replicate count x @ [y]