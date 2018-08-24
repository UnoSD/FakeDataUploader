module Job

open FSharp.Data

[<Literal>]
let private sampleJson = @"{""id"":""x"",""status"":""x""}"

type JobStatusProvider =
    JsonProvider<sampleJson, RootName="Status">

let parseStatus json = 
    JobStatusProvider.Parse json