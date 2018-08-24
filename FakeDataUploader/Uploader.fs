module Uploader

open System

type Status = 
    Job.JobStatusProvider.Status

let rec pushData' generateJson
                  (send : string -> Async<Status>)
                  pollUntilDone
                  getResult
                  results
                  total =
    async {
        let (count, payload) =
            generateJson total
                
        let! jobStatus =
            payload |>
            send

        let! finalStatus =
            pollUntilDone jobStatus.Id (TimeSpan.FromMinutes(2.))

        let! report =
            getResult jobStatus.Id

        let allResults = 
            report :: results

        return! match (total - count, finalStatus) with
                | (0, "Success")    -> async.Return (Ok allResults)
                | (toDo, "Success") -> pushData' generateJson
                                                 send
                                                 pollUntilDone
                                                 getResult
                                                 results
                                                 toDo
                | _                 -> async.Return (Error (finalStatus, allResults))
    }

let pushData generateJson send pollUntilDone getResult total =
    pushData' generateJson send pollUntilDone getResult [] total