module Api

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System.Text

type Credentials =
    {
        Endpoint : string
        User  : string
        Password : string
    }

let private getBody =
    function
    | Text text -> text
    | _         -> failwith "TODO"

let private getStatus =
    getBody >> Job.parseStatus

let send credentials body =
    async {
        let! response =
            Http.AsyncRequest(url        = credentials.Endpoint, 
                              headers    = [ ContentType     HttpContentTypes.Json
                                             ContentEncoding Encoding.UTF8.HeaderName
                                             BasicAuth       credentials.User 
                                                             credentials.Password     ],
                              httpMethod = HttpMethod.Post,
                              body       = TextRequest body)

        return response.Body |> getStatus
    }

// TODO: Timeout
let rec pollUntilDone credentials id timeout =
    async {
        let! response =
            Http.AsyncRequest(credentials.Endpoint + id,
                              headers = [ BasicAuth credentials.User
                                                    credentials.Password ])

        let jobStatus = 
            response.Body |>
            getBody |>
            Job.parseStatus

        match jobStatus.Status with
        | "Starting"   
        | "InProgress"  -> do! Async.Sleep(500)
                           return! pollUntilDone credentials id timeout
        | "Success"
        | "Error"
        | _             -> return jobStatus.Status
    }

let getResults credentials id =
    async {
        let! response =
            Http.AsyncRequest(credentials.Endpoint + id + "/results",
                              headers = [ BasicAuth credentials.User
                                                    credentials.Password ])
            
        return response.Body |> getStatus
    }