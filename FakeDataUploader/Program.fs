open Concurrency
open FSharpx.Collections
open Job
open ContentGenerator
open Api
open Result

let user             = "user"
let password         = "password"
let endpoint         = "https://endpoin.t"
let objectsCount     = 120000
let parallelism      = 1
let maxRequestLength = 15360.<KB>

[<EntryPoint>]
let main _ =
    // Real API
    let credentials =
        {
            Endpoint = endpoint
            User = user
            Password = password
        }
    let send =
        Api.send credentials 
    let pollUntilDone =
        Api.pollUntilDone credentials
    let getReport =
        Api.getResults credentials

    // Fakes
    let send _ =
        async.Return (JobStatusProvider.Status(id = "", status = "Finished"))
    let pollUntilDone _ _ =
        async.Return "Success"
    let getReport _ =
        async.Return "All successfully loaded"

    let generateJsons count =
        getItemsJson Item.createJson
                     maxRequestLength
                     "id"
                     count
                     BogusFaker.interpret

    let pushData =
        Uploader.pushData generateJsons
                          send 
                          pollUntilDone 
                          getReport

    objectsCount |>
    partitions parallelism |>
    List.map pushData |>
    Async.Parallel |>
    Async.RunSynchronously |>
    Array.fold resultsFolder (Ok []) |>
    function
    | Ok report              -> printf "%A" report
                                0
    | Error (status, report) -> printf "%A - %A" status report
                                1