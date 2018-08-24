[<RequireQualifiedAccess>]
module Item

open FSharp.Data
open FreeFaker

[<Literal>]
let private sampleJson = @"
{
	""systemId"" : ""string"",
	""id""       : ""string"",
	""custom""   : {
        ""luckyNumber"" : 0,
        ""status""      : ""string"",
        ""image""       : ""string"",
        ""total""       : 0.0,
        ""url""         : ""string""
    }
}
"

let private minifyJson json =
    // Surely this is not efficient, replace with a proper implementation
    Newtonsoft.Json.Linq.JObject.Parse(json).ToString(Newtonsoft.Json.Formatting.None)

let private minifiedTokenizedSampleJson =
    sampleJson |>
    minifyJson |>
    (fun json -> json.Replace("string", "%s")
                     .Replace("0.0",    "%M")
                     .Replace("0",      "%i"))

let createJson baseId =
    faker {
        let! systemId    = FreeFaker.guid
        let! id          = FreeFaker.guid
        let! luckyNumber = FreeFaker.positiveInteger
        let! status      = FreeFaker.oneOf           [ "New"; "Used" ]
        let! image       = FreeFaker.imageUrl
        let! total       = FreeFaker.decimal         { From = 1.0m; To = 1000.0m }
        let! url         = FreeFaker.url

        return
            (minifiedTokenizedSampleJson |>
             Printf.StringFormat<string->string->int->string->string->decimal->string->string> |>
             sprintf)
                systemId   
                (baseId + id)         
                luckyNumber
                status     
                image      
                total      
                url        
    }

type ItemsProvider = 
    JsonProvider<Sample=sampleJson, RootName="Item">

let create id =
    faker {
        let! json = createJson id

        return json |> JsonValue.Parse |> ItemsProvider.Item
    }