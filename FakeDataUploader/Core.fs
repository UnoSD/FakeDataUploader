[<AutoOpen>]
module Core

let flipt f x y =
    f (y, x)

let flip f x y =
    f y x