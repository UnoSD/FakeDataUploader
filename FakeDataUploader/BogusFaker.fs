module BogusFaker

open FreeFaker
open Bogus
open System

let private faker = Faker()

let private imageUrl (faker : Faker) =
    sprintf "https://picsum.photos/%i/%i?image=%i" (faker.Random.Int(100, 1024))
                                                   (faker.Random.Int(100, 768))
                                                   (faker.Random.Int(0, 1084))

let private decimal range (faker : Faker) =
    faker.Random.Decimal(range.From, range.To) |>
    (fun x -> Math.Round(x, 2))
    
let rec interpret = 
    function
    | Integer (range, next) -> faker.Random.Int(range.From, range.To) |> next |> interpret
    | Decimal (range, next) -> faker |> decimal range                 |> next |> interpret
    | OneOf (strings, next) -> faker.PickRandom(strings)              |> next |> interpret
    | Guid next             -> faker.Random.Uuid().ToString("n")      |> next |> interpret
    | Url next              -> faker.Internet.Url()                   |> next |> interpret
    | ImageUrl next         -> faker |> imageUrl                      |> next |> interpret
    | Pure x                -> x