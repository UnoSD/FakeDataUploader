module MinimunDataFaker

open FreeFaker

let rec interpret = 
    function
    | Integer (_, next) -> 0   |> next |> interpret
    | Decimal (_, next) -> 0.m |> next |> interpret
    | OneOf (_, next)  
    | Guid next        
    | Url next         
    | ImageUrl next     -> ""  |> next |> interpret
    | Pure x            -> x