module FreeFaker

type Range<'a> =
    {
        From: 'a
        To: 'a
    }

type FakerInstruction<'a> =
    | Integer     of Range<int>     * (int     -> FakerInstruction<'a>)
    | Decimal     of Range<decimal> * (decimal -> FakerInstruction<'a>)
    | OneOf       of string list    * (string  -> FakerInstruction<'a>)
    | Guid        of (string -> FakerInstruction<'a>)
    | Url         of (string -> FakerInstruction<'a>)
    | ImageUrl    of (string -> FakerInstruction<'a>)
    | Pure        of 'a
    
type FakerBuilder() =
    member this.Bind (x, f) =
        match x with
        | Integer (range, next) -> Integer     (range,   next >> (this.Bind |> flipt) f)
        | Decimal (range, next) -> Decimal     (range,   next >> (this.Bind |> flipt) f)
        | OneOf (strings, next) -> OneOf       (strings, next >> (this.Bind |> flipt) f)
        | Guid next             -> Guid        (next >> (this.Bind |> flipt) f)
        | Url next              -> Url         (next >> (this.Bind |> flipt) f)
        | ImageUrl next         -> ImageUrl    (next >> (this.Bind |> flipt) f)
        | Pure x                -> f x
    member __.Return x = 
        Pure x
    member __.ReturnFrom x =
        x
    member __.Zero () = 
        Pure ()

let faker = 
    new FakerBuilder()

[<RequireQualifiedAccess>]
module FreeFaker =
    let integer range = Integer     (range, Pure)
    let decimal range = Decimal     (range, Pure)
    let oneOf strings = OneOf       (strings, Pure)
    let guid          = Guid        Pure
    let url           = Url         Pure
    let imageUrl      = ImageUrl    Pure

    let positiveInteger = Integer ({ From = 0; To = System.Int32.MaxValue }, Pure)